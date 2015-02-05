using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace VAPPCT.DA
{
    /// <summary>
    /// Summary description for CSession
    /// </summary>
    public class CData
    {
        /// <summary>
        /// constructor for the base data class that takes a loaded CDATA
        /// </summary>
        public CData(CData dataInit)
        {
            DBConn = dataInit.DBConn;
            UserID = dataInit.UserID;
            ClientIP = dataInit.ClientIP;
            SessionID = dataInit.SessionID;
            MDWSTransfer = dataInit.MDWSTransfer;
            WebSession = dataInit.WebSession;
        }

        /// <summary>
        /// constructor for the base CData class
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="strClientIP"></param>
        /// <param name="lUserID"></param>
        /// <param name="strSessionID"></param>
        /// <param name="bMDWSTransfer"></param>
        public CData(CDataConnection conn,
                     string strClientIP,
                     long lUserID,
                     string strSessionID,
                     System.Web.SessionState.HttpSessionState SessionState,
                     bool bMDWSTransfer)
        {
            DBConn = conn;
            UserID = lUserID;
            ClientIP = strClientIP;
            SessionID = strSessionID;
            WebSession = SessionState;
            MDWSTransfer = bMDWSTransfer;
        }

        private bool m_bMDWSTransfer;

        /// <summary>
        /// is MDWS Transfer On?
        /// </summary>
        public bool MDWSTransfer
        {
            set
            {
                m_bMDWSTransfer = value;
            }
            get
            {
                return m_bMDWSTransfer;
            }
        }

        private System.Web.SessionState.HttpSessionState m_WebSession;

        /// <summary>
        /// Session passed in from caller
        /// </summary>
        public System.Web.SessionState.HttpSessionState WebSession
        {
            set
            {
                m_WebSession = value;
            }
            get
            {
                return m_WebSession;
            }
        }

        private CDataConnection m_DBConn;
        /// <summary>
        /// database connection
        /// </summary>
        public CDataConnection DBConn
        {
            set
            {
                m_DBConn = value;
            }
            get
            {
                return m_DBConn;
            }
        }

        private string m_strClientIP;
        /// <summary>
        /// get/set client ip
        /// </summary>
        public string ClientIP
        {
            set
            {
                m_strClientIP = value;

            }
            get
            {
                return m_strClientIP;
            }
        }

        private long m_lUserID;
        /// <summary>
        /// user id of the user currently logged in
        /// </summary>
        public long UserID
        {
            set
            {
                m_lUserID = value;
            }
            get
            {
                return m_lUserID;
            }
        }

        private string m_strSessionID;
        /// <summary>
        /// session id
        /// </summary>
        public string SessionID
        {
            get
            {
                return m_strSessionID;
            }
            set
            {
                m_strSessionID = value;
            }
        }

        /// <summary>
        /// check for a valid connection
        /// </summary>
        /// <returns></returns>
        public CStatus DBConnValid()
        {
            CStatus status = new CStatus();
            status.StatusCode = k_STATUS_CODE.Success;
            status.Status = true;
            status.StatusComment = String.Empty;

            if (DBConn == null)
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = "Database connection is null!";
            }

            return status;
        }
    }
}