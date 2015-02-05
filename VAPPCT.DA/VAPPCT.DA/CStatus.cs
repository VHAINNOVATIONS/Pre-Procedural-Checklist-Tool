using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAPPCT.DA
{
    public class CStatus
    {
        /// <summary>
        /// bool status
        /// </summary>
        bool m_bStatus;
        public bool Status
        {
            get { return m_bStatus; }
            set { m_bStatus = value; }
        }

        /// <summary>
        /// status comment
        /// </summary>
        string m_strStatusComment;
        public string StatusComment
        {
            get { return m_strStatusComment; }
            set { m_strStatusComment = value; }
        }

        /// <summary>
        /// status code
        /// </summary>
        k_STATUS_CODE m_lStatusCode;
        public k_STATUS_CODE StatusCode
        {
            get { return m_lStatusCode; }
            set { m_lStatusCode = value; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public CStatus()
        {
            //initialize values
            m_strStatusComment = String.Empty;
            m_lStatusCode = k_STATUS_CODE.Success;
            m_bStatus = true;
        }

        /// <summary>
        /// constructor
        /// initializes the members of the instance with the passed values
        /// </summary>
        /// <param name="bStatus"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatusComment"></param>
        public CStatus(bool bStatus, k_STATUS_CODE lStatusCode, string strStatusComment)
        {
            m_strStatusComment = strStatusComment;
            m_lStatusCode = lStatusCode;
            m_bStatus = bStatus;
        }
    }
}
