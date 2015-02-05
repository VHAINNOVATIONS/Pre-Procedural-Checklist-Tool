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
    /// This class contains methods/properties used to connect to an oracle
    /// database and execute stored procedures.
    /// </summary>
    public class CDataConnection
    {
        //are in "audit" mode or not
        private bool m_bAudit;
        
        //the underlying Oracle connection
        private OracleConnection m_OracleConnection;

        /// <summary>
        /// constructor
        /// </summary>
        public CDataConnection()
        {
            m_OracleConnection = null;
            m_bAudit = false;
        }

        /// <summary>
        /// readonly auditing property
        /// </summary>
        public bool Audit
        {
            get
            {
                return m_bAudit;
            }
        }
        
        /// <summary>
        /// oracle connection property
        /// </summary>
        public OracleConnection Conn
        {
            get
            {
                return m_OracleConnection;
            }
            set
            {
                m_OracleConnection = value;
            }
        }

        /// <summary>
        /// Connect to the Oracle database using the connection string
        /// passed in. if bAudit is true, all SP calls are audited 
        /// to the FX_AUDIT table
        /// </summary>
        /// <param name="strConnectionString"></param>
        /// <param name="bAudit"></param>
        /// <returns></returns>
        public CStatus Connect( string strConnectionString, 
                                 bool bAudit)
        {
            m_bAudit = bAudit;
            CStatus status = new CStatus();

            //close connection if it is open
            if (m_OracleConnection != null)
            {
                try
                {
                    m_OracleConnection.Close();
                    m_OracleConnection = null;
                }
                catch (OracleException e)
                {
                    m_OracleConnection = null;
                    status.StatusComment = e.Message;
                    status.StatusCode = k_STATUS_CODE.Failed;
                    status.Status = false;
                }
            }
            
            //make sure the string is valid
            if (String.IsNullOrEmpty(strConnectionString))
            {
                status.StatusComment = "Connectuion string is empty!";
                status.StatusCode = k_STATUS_CODE.Failed;
                status.Status = false;
                m_OracleConnection = null;

                return status;
            }

            //trim the connection string
            string strConnString = strConnectionString.Trim();
            try
            {
                m_OracleConnection = new OracleConnection(strConnString);
                m_OracleConnection.Open();

                strConnString = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
                return status;
            }
            catch (OracleException e)
            {
                status.StatusComment = e.Message;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.Status = false;
                m_OracleConnection = null;
              
                strConnString = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
                return status;
            }
            catch (ArgumentException ee)
            {
                status.StatusComment = ee.Message;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.Status = false;
                m_OracleConnection = null;

                strConnString = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
                return status;
            }
            catch (Exception eee)
            {
                status.StatusComment = eee.Message;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.Status = false;
                m_OracleConnection = null;

                strConnString = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
                return status;
            }
        }

        /// <summary>
        /// builds audit xml from the paramater list
        /// </summary>
        /// <param name="ParamList"></param>
        /// <returns></returns>
        private string GetAuditXML(string strSPName, CParameterList ParamList)
        {
            string strAuditXML = "<sp_name>" + strSPName + "</sp_name>";

            //add the parameters from the parameter list to the command parameter list
            for (int i = 0; i < ParamList.Count; i++)
            {
                CParameter parameter = ParamList.GetItemByIndex(i);
                if (parameter != null)
                {
                    strAuditXML += "<" + parameter.ParameterName + ">";

                    if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.StringParameter)
                    {
                        strAuditXML += parameter.StringParameterValue;
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.LongParameter)
                    {
                        strAuditXML += Convert.ToString(parameter.LongParameterValue);
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DoubleParameter)
                    {
                        strAuditXML += Convert.ToString(parameter.DoubleParameterValue);
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.IntParameter)
                    {
                        strAuditXML += Convert.ToString(parameter.IntParameterValue);
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DateParameter)
                    {
                        strAuditXML += CDataUtils.GetDateAsString(parameter.DateParameterValue);
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.CLOBParameter)
                    {
                        strAuditXML += parameter.CLOBParameterValue;
                    }
                    else
                    {
                        strAuditXML += parameter.StringParameterValue;
                    }

                    strAuditXML += "</" + parameter.ParameterName + ">";
                }
            }

            return strAuditXML;
        }

        /// <summary>
        /// writes to the FX_AUDIT table, used here and in CDataSet
        /// </summary>
        /// <param name="strSPName"></param>
        /// <param name="ParamList"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public bool AuditTransaction( string strSPName,
                                      CParameterList ParamList,
                                      k_STATUS_CODE lInStatusCode,
                                      string strInStatus,
                                      out k_STATUS_CODE lStatusCode,
                                      out string strStatus)
        {
            //System.Threading.Thread.Sleep(500);
            lStatusCode = k_STATUS_CODE.Success;
            strStatus = String.Empty;

            //get the xml to audit
            string strAuditXML = GetAuditXML(strSPName, ParamList);

            //add status info
            strAuditXML += "<status>" + strInStatus + "</status>";

            //build the paramater list
            CParameterList plistAudit = new CParameterList();
            plistAudit.AddInputParameter("pi_vSessionClientIP", ParamList.GetItemByName("pi_vSessionClientIP").StringParameterValue);
            plistAudit.AddInputParameter("pi_nUserID", ParamList.GetItemByName("pi_nUserID").LongParameterValue);
            plistAudit.AddInputParameter("pi_vSPName", strSPName);
            
            //audit data is a clob
            plistAudit.AddInputParameterCLOB("pi_clAuditXML", strAuditXML);
            
            //status is the status code
            plistAudit.AddInputParameter("pi_nStatus", (long)lInStatusCode);

            //execute sp and do not audit the audit
            bool bStatus = ExecuteOracleSP( false,
                                            "PCK_FX_SECURITY.AuditTransaction",
                                            plistAudit,
                                            out lStatusCode,
                                            out strStatus);
           
            return bStatus;

        }

        /// <summary>
        /// NEW execute with status info returned
        /// </summary>
        /// <param name="strSPName"></param>
        /// <param name="ParamList"></param>
        /// <returns></returns>
        public CStatus ExecuteOracleSP(string strSPName,
                                        CParameterList ParamList)
        {
            k_STATUS_CODE lStatusCode = k_STATUS_CODE.Success;
            string strStatus = String.Empty;

            CStatus status = new CStatus();
            status.Status = ExecuteOracleSP(strSPName,
                                            ParamList,
                                            out lStatusCode,
                                            out strStatus);
            status.StatusCode = lStatusCode;
            status.StatusComment = strStatus;

            return status;
        }

        /// <summary>
        /// Execute an oracle stored procedure using the 
        /// parameters passed in, using the connections audit property
        /// </summary>
        /// <param name="strSPName"></param>
        /// <param name="ParamList"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public bool ExecuteOracleSP(string strSPName,
                                    CParameterList ParamList,
                                    out k_STATUS_CODE lStatusCode,
                                    out string strStatus)
        {
           
            //execute the sp
            bool bStatus = ExecuteOracleSP( m_bAudit,
                                            strSPName,
                                            ParamList,
                                            out lStatusCode,
                                            out strStatus);
           
            //audit if auditing is turned on
            if (m_bAudit)
            {
                k_STATUS_CODE lAuditStatusCode = k_STATUS_CODE.Success;
                string strAuditStatus = String.Empty;

                if (!AuditTransaction(strSPName,
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

            }

            return bStatus;
        }

        /// <summary>
        /// Execute an oracle stored procedure using the 
        /// parameters passed in
        /// </summary>
        /// <param name="strSPName"></param>
        /// <param name="ParamList"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        private bool ExecuteOracleSP(bool bAudit,
                                     string strSPName,
                                     CParameterList ParamList,
                                     out k_STATUS_CODE lStatusCode,
                                     out string strStatus)
        {
            lStatusCode = k_STATUS_CODE.Success;
            strStatus = String.Empty;

            //return null if no conn
            if (m_OracleConnection == null)
            {
                lStatusCode = k_STATUS_CODE.Failed;
                strStatus = "Unable to connect to data source, Data Connection is null";

                return false;
            }

            //create a new command object and set the command objects connection, text and type
            //must use OracleCommand or you cannot get back a ref cur out param which is how
            //we do things in medbase
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = m_OracleConnection;
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
                    OracleParameter oraParameter = new OracleParameter();
                    oraParameter.ParameterName = parameter.ParameterName;

                    //set the parameter value, default to string. Probably a better way than the
                    //if then else, but this works and we can find it later,
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
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.IntParameter)
                    {
                        oraParameter.Value = parameter.IntParameterValue;
                        oraParameter.OracleType = OracleType.Int32;
                    }
                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DateParameter)
                    {
                        oraParameter.Value = parameter.DateParameterValue;

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
                        OracleTransaction transaction = m_OracleConnection.BeginTransaction();

                        //make a new command
                        OracleCommand command = m_OracleConnection.CreateCommand();
                        command.Connection = m_OracleConnection;
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

            //add in out params for stored proc, all sp's will return a status 0 = good, 1 = bad
            //
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
            try
            {
                //execute the stored proc and move the out param values back into 
                //our list
                cmd.ExecuteNonQuery();

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
                                            parameter.StringParameterValue = parameter.StringParameterValue.Trim();
                                        }
                                    }
                                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.LongParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            if (oP.Value.ToString() != String.Empty)
                                            {
                                                parameter.LongParameterValue = Convert.ToInt64(oP.Value);
                                            }
                                        }
                                    }
                                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DoubleParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            if (oP.Value.ToString() != String.Empty)
                                            {
                                                parameter.DoubleParameterValue = Convert.ToDouble(oP.Value);
                                            }
                                        }
                                    }
                                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.BoolParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            if (oP.Value.ToString() != String.Empty)
                                            {
                                                parameter.BoolParameterValue = Convert.ToBoolean(oP.Value);
                                            }
                                        }
                                    }
                                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.IntParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            if (oP.Value.ToString() != String.Empty)
                                            {
                                                parameter.IntParameterValue = Convert.ToInt32(oP.Value);
                                            }
                                        }
                                    }
                                    else if (parameter.ParameterType == k_DATA_PARAMETER_TYPE.DateParameter)
                                    {
                                        if (oP.Value != null)
                                        {
                                            if (oP.Value.ToString() != String.Empty)
                                            {
                                                if (!oP.Value.ToString().Equals(String.Empty))
                                                {
                                                    parameter.DateParameterValue = Convert.ToDateTime(oP.Value);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (oP.Value != null)
                                        {
                                            parameter.StringParameterValue = oP.Value.ToString();
                                            parameter.StringParameterValue = parameter.StringParameterValue.Trim();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                lStatusCode = ParamList.GetStatusCode();
                strStatus = ParamList.GetStatusComment();

                if (lStatusCode == k_STATUS_CODE.Success)
                {
                    return true;
                }

                return false;
            }
            catch (InvalidOperationException e)
            {
                strStatus = e.Message;
                lStatusCode = k_STATUS_CODE.Failed;
            }
            catch (OracleException e)
            {
                strStatus = e.Message;
                lStatusCode = k_STATUS_CODE.Failed;
            }

            return false;
        }

        /// <summary>
        /// desctructor
        /// </summary>
        ~CDataConnection()
        {
            //we should not really be closing here, the user must call close!!!!
            if (m_OracleConnection != null)
            {
                try
                {
                    m_OracleConnection.Close();
                    m_OracleConnection.Dispose();
                    m_OracleConnection = null;
                }
                catch (OracleException e)
                {
                    string strError = e.Message;
                    m_OracleConnection = null;
                }
                //catch (InvalidOperationException e)
                //{
                //    m_OracleConnection = null;
                //}
            }
        }

        /// <summary>
        ///close the underlying Oracle connection
        /// </summary>
        public void Close()
        {
            if (m_OracleConnection != null)
            {
                m_OracleConnection.Close();
                m_OracleConnection.Dispose();
                m_OracleConnection = null;
            }
        }
    }
}
