using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Data.Sql;
using System.Data.SqlClient;

namespace VAPPCT.DA
{
    /// <summary>
    /// This class contains methods/properties used to
    /// execute a stored procedure that returns a dataset.
    /// </summary>
    public class CDataSet
    {
        /// <summary>
        /// constructor
        /// </summary>
        public CDataSet()
        {
        }

        /// <summary>
        /// audit the query
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strSPName"></param>
        /// <param name="ParamList"></param>
        /// <param name="lInStatusCode"></param>
        /// <param name="strInStatue"></param>
        /// <param name="lAuditStatusCode"></param>
        /// <param name="strAuditStatus"></param>
        /// <returns></returns>
        public bool AuditTransaction(CDataConnection conn,
                                     string strSPName,
                                     CParameterList ParamList,
                                     k_STATUS_CODE lInStatusCode,
                                     string strInStatus,
                                     out k_STATUS_CODE lAuditStatusCode,
                                     out string strAuditStatus)
        {
            lAuditStatusCode = k_STATUS_CODE.Success;
            strAuditStatus = String.Empty;

            if (conn.Audit)
            {
                return conn.AuditTransaction(strSPName,
                                              ParamList,
                                              lInStatusCode,
                                              strInStatus,
                                              out lAuditStatusCode,
                                              out strAuditStatus);
            }

            return true;
        }

        /// <summary>
        /// New call using CStatus, calls will be convertyed to this later
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strSPName"></param>
        /// <param name="ParamList"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public CStatus GetOracleDataSet(CDataConnection conn,
                                        string strSPName,
                                        CParameterList ParamList,
                                        out DataSet ds)
        {
            ds = null;

            k_STATUS_CODE lStatusCode = k_STATUS_CODE.Success;
            string strStatus = String.Empty;

            CStatus status = new CStatus();
            status.Status = GetOracleDataSet(conn,
                                             strSPName,
                                             ParamList,
                                             out ds,
                                             out lStatusCode,
                                             out strStatus);
            status.StatusCode = lStatusCode;
            status.StatusComment = strStatus;

            return status;
        }
       
        /// <summary>
        /// retrieve an Oracle dataset from an SP call
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strSPName"></param>
        /// <param name="ParamList"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public bool GetOracleDataSet(CDataConnection conn,
                                        string strSPName,
                                        CParameterList ParamList,
                                        out DataSet ds,
                                        out k_STATUS_CODE lStatusCode,
                                        out string strStatus)
        {
            ds = null;
            lStatusCode = k_STATUS_CODE.Success;
            strStatus = String.Empty;

            k_STATUS_CODE lAuditStatusCode = k_STATUS_CODE.Success;
            string strAuditStatus = String.Empty;
           
            //return null if no conn
            if (conn == null)
            {
                ds = null;
                lStatusCode = k_STATUS_CODE.Failed;
                strStatus = "Unable to connect to data source, data connection is null!";

                //audit the error if audting is on
                AuditTransaction(conn,
                                 strSPName,
                                 ParamList,
                                 lStatusCode,
                                 strStatus,
                                 out lAuditStatusCode,
                                 out strAuditStatus);

                return false;
            }

            //create a new command object and set the command objects connection, text and type
            //must use OracleCommand or you cannot get back a ref cur out param which is how
            //we do things in medbase
            OracleCommand cmd = new OracleCommand(); // OleDbCommand();
            cmd.Connection = conn.Conn;
            cmd.CommandText = strSPName;
            cmd.CommandType = CommandType.StoredProcedure;

            //add the parameters from the parameter list to the command parameter list
            for (int i = 0; i < ParamList.Count; i++)
            {
                CParameter parameter = ParamList.GetItemByIndex(i);
                if (parameter != null)
                {
                    //create a new oledb param from our param and add it to the list
                    //this follows how we currently do it in medbase
                    //TODO: get direction, length etc from the parameter not hard coded below
                    OracleParameter oraParameter = new OracleParameter();
                    oraParameter.ParameterName = parameter.ParameterName;

                    //set the parameter value, default to string. 
                    if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.StringParameter)
                    {
                        oraParameter.Value = parameter.StringParameterValue;
                        oraParameter.OracleType = OracleType.VarChar;
                        oraParameter.Size = 4000;
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.LongParameter)
                    {
                        oraParameter.Value = parameter.LongParameterValue;
                        oraParameter.OracleType = OracleType.Int32;
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DoubleParameter)
                    {
                        oraParameter.Value = parameter.DoubleParameterValue;
                        oraParameter.OracleType = OracleType.Double;
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DateParameter)
                    {
                        if (!CDataUtils.IsDateNull(parameter.DateParameterValue))
                        {   
                            //if the date is not null then set the value
                            oraParameter.Value = parameter.DateParameterValue;
                        }
                        else
                        {
                            //set value to DBNull if date is null
                            oraParameter.Value = DBNull.Value;
                        }

                        oraParameter.OracleType = OracleType.DateTime;
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.CLOBParameter)
                    {
                        //You must begin a transaction before obtaining a temporary LOB. 
                        //Otherwise, the OracleDataReader may fail to obtain data later.
                        OracleTransaction transaction = conn.Conn.BeginTransaction();

                        //make a new command
                        OracleCommand command = conn.Conn.CreateCommand();
                        command.Connection = conn.Conn;
                        command.Transaction = transaction;
                        command.CommandText = "declare xx clob; begin dbms_lob.createtemporary(xx, false, 0); :tempclob := xx; end;";
                        command.Parameters.Add(new OracleParameter("tempclob", OracleType.Clob)).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();

                        //get a temp lob
                        OracleLob tempLob = (OracleLob)command.Parameters[0].Value;

                        //begin batch
                        tempLob.BeginBatch(OracleLobOpenMode.ReadWrite);

                        //convert string to byte array and write to lob,
                        //note:encoding must be set to match oracle encoding
                        //default encoding for oracle can be found by running
                        //SELECT value$ FROM sys.props$ WHERE name = 'NLS_CHARACTERSET' ;
                        System.Text.ASCIIEncoding encASCII = new System.Text.ASCIIEncoding();

                        //ascii chars
                        byte[] buffAsciiBytes = encASCII.GetBytes(parameter.CLOBParameterValue);

                        //destination to convert to. convert from ascii to unicode
                        UnicodeEncoding encDest = new UnicodeEncoding();
                        byte[] buffDest = Encoding.Convert(encASCII, encDest, buffAsciiBytes);

                        //write the converted data to the lob
                        tempLob.Write(buffDest,
                                      0,
                                      buffDest.Length);

                        //end batch
                        tempLob.EndBatch();

                        //set the value of the param =  lob
                        oraParameter.OracleType = OracleType.Clob;
                        //oraParameter.OracleType = OracleType.NClob;

                        oraParameter.Value = tempLob;

                        //all done so commit;
                        transaction.Commit();
                    }
                    else
                    {
                        oraParameter.Value = parameter.StringParameterValue;
                        oraParameter.OracleType = OracleType.VarChar;
                        oraParameter.Size = 4000;
                    }

                    oraParameter.Direction = parameter.Direction;
                    cmd.Parameters.Add(oraParameter);
                }
            }

            //add in out params for stored proc, all sp's will return a status 1 = good, 0 = bad
            //status
            ParamList.AddOutputParameter("po_nStatusCode", 0);
            OracleParameter oraStatusParameter = new OracleParameter("po_nStatusCode",
                                                                       OracleType.Int32);
            oraStatusParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(oraStatusParameter);
            //
            //comment
            ParamList.AddOutputParameter("po_vStatusComment", String.Empty);
            OracleParameter oraCommentParameter = new OracleParameter("po_vStatusComment",
                                                                        OracleType.VarChar,
                                                                        4000);
            oraCommentParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(oraCommentParameter);
            // 
            //now add an out parameter to hold the ref cursor to the commands parameter list
            //returned ref cursor must always be named "RS" because OracleClient binds these 
            //parameters by name, so you must name your parameter correctly
            //so the OracleParameter must be named the same thing.
            OracleParameter oraRSParameter = new OracleParameter("RS",
                                                                    OracleType.Cursor);
            //OracleType.Cursor
            oraRSParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(oraRSParameter);

            //create a new dataset to hold the conntent of the reference cursor
            ds = new DataSet();
                        
            //create an adapter and fill the dataset. 
            //datasets are completely disconnected and provide 
            //the most flexibility for later porting to a web service etc.
            try
            {
                OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd);
                dataAdapter.Fill(ds);
            }
            catch (InvalidOperationException e)
            {
                ds = null;
                lStatusCode = k_STATUS_CODE.Failed;
                strStatus = e.Message;

                //audit if auditing is turned on
                AuditTransaction(conn,
                                 strSPName,
                                 ParamList,
                                 lStatusCode,
                                 strStatus,
                                 out lAuditStatusCode,
                                 out strAuditStatus);

                return false;
            }
            catch (OracleException e)
            {
                ds = null;
                lStatusCode = k_STATUS_CODE.Failed;
                strStatus = e.Message;

                //audit if auditing is turned on
                AuditTransaction(conn,
                                 strSPName,
                                 ParamList,
                                 lStatusCode,
                                 strStatus,
                                 out lAuditStatusCode,
                                 out strAuditStatus);


                return false;
            }

            if (lStatusCode == k_STATUS_CODE.Success)
            {
                //now read back out params into our list
                for (int i = 0; i < ParamList.Count; i++)
                {
                    CParameter parameter = ParamList.GetItemByIndex(i);
                    if (parameter != null)
                    {
                        if (parameter.Direction == ParameterDirection.Output ||
                            parameter.Direction == ParameterDirection.InputOutput)
                        {
                            foreach (OracleParameter oP in cmd.Parameters)
                            {
                                if (oP.ParameterName.Equals(parameter.ParameterName))
                                {
                                    if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.StringParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            parameter.StringParameterValue = oP.Value.ToString();
                                        }
                                    }
                                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.LongParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            if (!oP.Value.ToString().Equals(String.Empty))
                                            {
                                                parameter.LongParameterValue = Convert.ToInt64(oP.Value);
                                            }
                                        }
                                    }
                                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DateParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            if (!oP.Value.ToString().Equals(String.Empty))
                                            {
                                                parameter.DateParameterValue = Convert.ToDateTime(oP.Value);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        parameter.StringParameterValue = oP.Value.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //set status code and text
            lStatusCode = ParamList.GetStatusCode();
            strStatus = ParamList.GetStatusComment();

            //audit if auditing is turned on
            if (!AuditTransaction(conn,
                             strSPName,
                             ParamList,
                             lStatusCode,
                             strStatus,
                             out lAuditStatusCode,
                             out strAuditStatus))
            {
                //original error overrules audit error
                if (lStatusCode == k_STATUS_CODE.Success)
                {
                    lStatusCode = lAuditStatusCode;
                    strStatus = strAuditStatus;
                }
            }

            if (lStatusCode != k_STATUS_CODE.Success)
            {
                return false;
            }

            return true;
        }        
    }
}
