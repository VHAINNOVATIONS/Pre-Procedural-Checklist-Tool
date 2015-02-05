using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace VAPPCT.DA
{
    /// <summary>
    /// This class is used to stord and retrieve a list of paramaters
    /// Paramaters are used to pass values to stored procedures and 
    /// throughout the application.
    /// </summary>
    public class CParameterList: ArrayList
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public CParameterList()
        {
            //set the lists initial capacity
            this.Capacity = 25;
        }

        /// <summary>
        /// constructor that adds standard SP params
        /// </summary>
        public CParameterList( string strSessionID,
                               string strClientIP,
                               long lUserID)
        {
            //set the lists initial capacity
            this.Capacity = 25;

            this.AddStandardParams(strSessionID, strClientIP, lUserID);
        }

        /// <summary>
        /// helper to add the status code params to the SP call
        /// </summary>
        protected void AddStatusCodeParams()
        {
            long lStatusCode = 0;
            string strStatusComment = String.Empty;

            AddOutputParameter("po_nStatusCode", lStatusCode);
            AddOutputParameter("po_vStatusComment", strStatusComment);
        }

        /// <summary>
        /// helper to add the status code params to the SP call
        /// </summary>
        public void AddStandardParams( string strSessionID,
                                       string strClientIP,
                                       long lUserID)
        {
            AddInputParameter("pi_vSessionID", strSessionID);
            AddInputParameter("pi_vSessionClientIP", strClientIP);
            AddInputParameter("pi_nUserID", lUserID);
        }

         /// <summary>
        /// add a date parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="dtValue"></param>
        /// <returns></returns>
        public bool AddInputParameter(string strName, DateTime dtValue)
        {
            return AddParameter(strName, dtValue, ParameterDirection.Input);
        }

        /// <summary>
        /// add a string parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public bool AddInputParameter(string strName, string strValue)
        {
            return AddParameter(strName, strValue, ParameterDirection.Input);
        }

        /// <summary>
        /// add a string parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public bool AddInputParameterCLOB(string strName, string strValue)
        {
            return AddCLOBParameter(strName, strValue, ParameterDirection.Input);
        }

        /// <summary>
        /// add a long parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="lValue"></param>
        /// <returns></returns>
        public bool AddInputParameter(string strName, long lValue)
        {
            return AddParameter(strName, lValue, ParameterDirection.Input);
        }

        /// <summary>
        /// add a bool parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="bValue"></param>
        /// <returns></returns>
        public bool AddInputParameter(string strName, bool bValue)
        {
            return AddParameter(strName, bValue, ParameterDirection.Input);
        }

        /// <summary>
        /// add a double parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="dblValue"></param>
        /// <returns></returns>
        public bool AddInputParameter(string strName, double dblValue)
        {
            return AddParameter(strName, dblValue, ParameterDirection.Input);
        }

        /// <summary>
        /// add a date parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="dtValue"></param>
        /// <returns></returns>
        public bool AddOutputParameter(string strName, DateTime dtValue)
        {
            return AddParameter(strName, dtValue, ParameterDirection.Output);
        }

        /// <summary>
        /// add a string parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public bool AddOutputParameter(string strName, string strValue)
        {
            return AddParameter(strName, strValue, ParameterDirection.Output);
        }

        /// <summary>
        /// add a long parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="lValue"></param>
        /// <returns></returns>
        public bool AddOutputParameter(string strName, long lValue)
        {
            return AddParameter(strName, lValue, ParameterDirection.Output);
        }

        /// <summary>
        /// add a bool parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="bValue"></param>
        /// <returns></returns>
        public bool AddOutputParameter(string strName, bool bValue)
        {
            return AddParameter(strName, bValue, ParameterDirection.Output);
        }

        /// <summary>
        /// add a double parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="dblValue"></param>
        /// <returns></returns>
        public bool AddOutputParameter(string strName, double dblValue)
        {
            return AddParameter(strName, dblValue, ParameterDirection.Output);
        }

        /// <summary>
        /// add a bool parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="bValue"></param>
        /// <param name="pmDirection"></param>
        /// <returns></returns>
        private bool AddParameter(string strName, bool bValue, ParameterDirection pmDirection)
        {
            CParameter p = new CParameter();
            p.ParameterName = strName;

            p.ParameterType = k_DATA_PARAMETER_TYPE.BoolParameter;
            p.BoolParameterValue = bValue;
            p.Direction = pmDirection;
            base.Add(p);

            return true;
        }

        /// <summary>
        /// add a double parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="dblValue"></param>
        /// <param name="pmDirection"></param>
        /// <returns></returns>
        private bool AddParameter(string strName, double dblValue, ParameterDirection pmDirection)
        {
            CParameter p = new CParameter();
            p.ParameterName = strName;

            p.ParameterType = k_DATA_PARAMETER_TYPE.DoubleParameter;
            p.DoubleParameterValue = dblValue;
            p.Direction = pmDirection;
            base.Add(p);

            return true;
        }

        /// <summary>
        /// add a date parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="dtValue"></param>
        /// <param name="pmDirection"></param>
        /// <returns></returns>
        private bool AddParameter(string strName, DateTime dtValue, ParameterDirection pmDirection)
        {   
            CParameter p = new CParameter();
            p.ParameterName = strName;
            
            if (dtValue.Year < 1800)
            {
                dtValue = new System.DateTime(0);
            }
           
            p.DateParameterValue = dtValue;

            p.ParameterType = k_DATA_PARAMETER_TYPE.DateParameter;
            p.Direction = pmDirection;
            base.Add(p);

            return true;
        }

        /// <summary>
        /// add a string parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        /// <param name="pmDirection"></param>
        /// <returns></returns>
        private bool AddParameter(string strName, string strValue, ParameterDirection pmDirection)
        {
            CParameter p = new CParameter();
            p.ParameterName = strName;
            p.StringParameterValue = strValue;
            p.ParameterType = k_DATA_PARAMETER_TYPE.StringParameter;
            p.Direction = pmDirection;
            base.Add(p);

            return true;
        }

        /// <summary>
        /// helper to add a CLOB parameter 
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        /// <param name="pmDirection"></param>
        /// <returns></returns>
        private bool AddCLOBParameter(string strName, string strValue, ParameterDirection pmDirection)
        {
            CParameter p = new CParameter();
            p.ParameterName = strName;
            p.CLOBParameterValue = strValue;
            p.ParameterType = k_DATA_PARAMETER_TYPE.CLOBParameter;
            p.Direction = pmDirection;
            base.Add(p);

            return true;
        }

        /// <summary>
        /// add a long parameter to the list
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="lValue"></param>
        /// <param name="pmDirection"></param>
        /// <returns></returns>
        private bool AddParameter(string strName, long lValue, ParameterDirection pmDirection)
        {
            CParameter p = new CParameter();
            p.ParameterName = strName;
            p.LongParameterValue = lValue;
            p.ParameterType = k_DATA_PARAMETER_TYPE.LongParameter;
            p.Direction = pmDirection;
            base.Add(p);

            return true;
        }

        /// <summary>
        /// get an item by name
        /// </summary>
        /// <param name="strParameterName"></param>
        /// <returns></returns>
        public CParameter GetItemByName(string strParameterName)
        {
            foreach (CParameter parameter in this)
            {
                if (parameter.ParameterName.Equals(strParameterName))
                {
                    return parameter;
                }
            }

            return null;
        }

        
        /// <summary>
        /// returns the string value of a paramater by name
        /// </summary>
        /// <param name="strParameterName"></param>
        /// <returns></returns>
        public string GetParamStringValue(string strParameterName)
        {
            CParameter param = GetItemByName(strParameterName);
            if (param != null)
            {
                return param.StringParameterValue;
            }

            return String.Empty;
        }

        /// <summary>
        /// returns a long paramater value by name
        /// </summary>
        /// <param name="strParameterName"></param>
        /// <returns></returns>
        public long GetParamLongValue(string strParameterName)
        {
            CParameter param = GetItemByName(strParameterName);
            if (param != null)
            {
                return param.LongParameterValue;
            }

            return -1;
        }

        /// <summary>
        /// returns and int paramater by name
        /// </summary>
        /// <param name="strParameterName"></param>
        /// <returns></returns>
        public int GetParamIntValue(string strParameterName)
        {
            CParameter param = GetItemByName(strParameterName);
            if (param != null)
            {
                return param.IntParameterValue;
            }

            return -1;
        }
        
        /// <summary>
        /// get a paramater item by index
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public CParameter GetItemByIndex(int nIndex)
        {
            return (CParameter)this[nIndex];
        }

        /// <summary>
        /// add a parameter to the list
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Add(CParameter parameter)
        {
            if (parameter != null)
            {
                base.Add(parameter);
            }

            return true;
        }

        /// <summary>
        /// get the status code
        /// </summary>
        /// <returns></returns>
        public k_STATUS_CODE GetStatusCode()
        {
            CParameter pStatusCode = GetItemByName("po_nStatusCode");
            if (pStatusCode != null)
            {
                if( (k_STATUS_CODE)pStatusCode.LongParameterValue == k_STATUS_CODE.Failed ||
                    (k_STATUS_CODE)pStatusCode.LongParameterValue == k_STATUS_CODE.Success ||
                    (k_STATUS_CODE)pStatusCode.LongParameterValue == k_STATUS_CODE.Unknown)
                {
                    return (k_STATUS_CODE)pStatusCode.LongParameterValue;
                }
            }

            return k_STATUS_CODE.Unknown;
        }

        /// <summary>
        /// get the status comment
        /// </summary>
        /// <returns></returns>
        public string GetStatusComment()
        {
            CParameter pStatusComment = GetItemByName("po_vStatusComment");
            if (pStatusComment != null)
            {
                return pStatusComment.StringParameterValue;
            }

            return String.Empty;
        }
    }
}
