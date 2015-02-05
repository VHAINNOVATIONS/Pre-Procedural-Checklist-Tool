using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAPPCT.DA
{
    public class CStringStatus : CStatus
    {
        private string m_strValue;
        /// <summary>
        /// property
        /// gets/sets the value string
        /// </summary>
        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        /// <summary>
        /// constructor
        /// initializes the members of the instance with default values
        /// </summary>
        public CStringStatus() : base()
        {
            m_strValue = string.Empty;
        }

        /// <summary>
        /// constructor
        /// initializes the members of the instance with the passes values
        /// </summary>
        /// <param name="bStatus"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatusComment"></param>
        /// <param name="strValue"></param>
        public CStringStatus(bool bStatus, k_STATUS_CODE lStatusCode, string strStatusComment, string strValue)
            : base(bStatus, lStatusCode, strStatusComment)
        {
            m_strValue = strValue;
        }

        public CStringStatus(CStatus status, string strValue)
            :base(status.Status, status.StatusCode, status.StatusComment)
        {
            m_strValue = strValue;
        }
    }
}
