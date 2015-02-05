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
    /// This class is used to stord and retrieve a paramater
    /// Paramaters are used to pass values to stored procedures and 
    /// throughout the application.
    /// </summary>
    public class CParameter
    {
        //private parameter name and value
        private string m_strParameterName;
        private string m_strParameterValue;

        private ParameterDirection m_Direction;
        private long m_lParameterValue;
        private k_DATA_PARAMETER_TYPE m_nParameterType;
        private DateTime m_dtDateValue;
        private int m_nParameterValue;
        private bool m_bParamaterValue;
        private double m_dblParamaterValue;
        private string m_strCLOBValue;

        /// <summary>
        /// constructor
        /// </summary>
        public CParameter()
        {
            m_strParameterName = String.Empty;
            m_strParameterValue = String.Empty;
            m_lParameterValue = -1;
            m_nParameterValue = -1;

            //default to string parameter
            m_nParameterType = k_DATA_PARAMETER_TYPE.StringParameter;
            
            m_dblParamaterValue = -1;
            m_bParamaterValue = false;
            m_strCLOBValue = String.Empty;
        }

        /// <summary>
        /// Converts and returns the parameter as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (m_nParameterType == k_DATA_PARAMETER_TYPE.StringParameter)
            {
                return m_strParameterValue;
            }
            else if (m_nParameterType == k_DATA_PARAMETER_TYPE.LongParameter)
            {
                return Convert.ToString(m_lParameterValue);
            }
            else if (m_nParameterType == k_DATA_PARAMETER_TYPE.IntParameter)
            {
                return Convert.ToString(m_nParameterValue);
            }
            else if (m_nParameterType == k_DATA_PARAMETER_TYPE.DateParameter)
            {
                return Convert.ToString(m_dtDateValue);
            }
            else if (m_nParameterType == k_DATA_PARAMETER_TYPE.BoolParameter)
            {
                return Convert.ToString(m_bParamaterValue);
            }
            else if (m_nParameterType == k_DATA_PARAMETER_TYPE.DoubleParameter)
            {
                return Convert.ToString(m_dblParamaterValue);
            }
            else if (m_nParameterType == k_DATA_PARAMETER_TYPE.CLOBParameter)
            {
                return Convert.ToString(m_strCLOBValue);
            }
            else
            {
                return m_strParameterValue;
            }
        }

        /// <summary>
        /// paramater type
        /// </summary>
        public k_DATA_PARAMETER_TYPE ParameterType
        {
            get
            {
                return m_nParameterType;
            }
            set
            {
                m_nParameterType = value;
            }
        }

        /// <summary>
        /// paramater direction
        /// </summary>
        public ParameterDirection Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                m_Direction = value;
            }
        }

        /// <summary>
        /// Parameter Name
        /// </summary>
        public string ParameterName
        {
            get
            {
                return m_strParameterName;
            }
            set
            {
                m_strParameterName = value;
            }
        }

        /// <summary>
        /// Parameter value - long
        /// </summary>
        public long LongParameterValue
        {
            get
            {
                return m_lParameterValue;
            }
            set
            {
                m_lParameterValue = value;
            }
        }

        /// <summary>
        /// parameter value - double
        /// </summary>
        public double DoubleParameterValue
        {
            get
            {
                return m_dblParamaterValue;
            }
            set
            {
                m_dblParamaterValue = value;
            }
        }        

        /// <summary>
        /// Parameter value - long
        /// </summary>
        public bool BoolParameterValue
        {
            get
            {
                return m_bParamaterValue;
            }
            set
            {
                m_bParamaterValue = value;
            }
        }

        /// <summary>
        /// Parameter value - int
        /// </summary>
        public int IntParameterValue
        {
            get
            {
                return m_nParameterValue;
            }
            set
            {
                m_nParameterValue = value;
            }
        }

        /// <summary>
        /// Parameter value - string
        /// </summary>
        public string StringParameterValue
        {
            get
            {
                return m_strParameterValue;
            }
            set
            {
                m_strParameterValue = value;
            }
        }

        /// <summary>
        /// Parameter value - date
        /// </summary>
        public DateTime DateParameterValue
        {
            get
            {
                return m_dtDateValue;
            }
            set
            {
                m_dtDateValue = value;
            }
        }

        /// <summary>
        /// Parameter value - CLOB
        /// </summary>
        public string CLOBParameterValue
        {
            get
            {
                return m_strCLOBValue;
            }
            set
            {
                m_strCLOBValue = value;
            }
        }
    }
}
