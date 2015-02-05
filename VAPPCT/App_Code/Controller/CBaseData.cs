using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using VAPPCT.DA;

/// <summary>
/// Summary description for CBaseData
/// </summary>
public class CBaseData
{
    /// <summary>
    /// database connection property
    /// </summary>
    CAppDBConn m_conn;
    public CAppDBConn DBConn
    {
        get { return m_conn; }
        set { m_conn = value; }
    }

    /// <summary>
    /// session id property
    /// </summary>
    string m_strSessionID;
    public string SessionID
    {
        get { return m_strSessionID; }
        set { m_strSessionID = value; }
    }
    
    /// <summary>
    /// client ip property
    /// </summary>
    string m_strClientIP;
    public string ClientIP
    {
        get { return m_strClientIP; }
        set { m_strClientIP = value; }
    }
    
    /// <summary>
    /// user id property
    /// </summary>
    long m_lUserID;
    public long UserID
    {
        get { return m_lUserID; }
        set { m_lUserID = value; }
    }

    /// <summary>
    /// base master property
    /// </summary>
    CBaseMaster m_BaseMster;
    public CBaseMaster BaseMster
    {
        get { return m_BaseMster; }
        set { m_BaseMster = value; }
    }

    /// <summary>
    /// 1 and only constructor
    /// </summary>
    /// <param name="BaseMaster"></param>
	public CBaseData(CBaseMaster BaseMaster)
	{
        BaseMster = BaseMaster;
        DBConn = BaseMster.DBConn;
        SessionID = BaseMster.SessionID;
        ClientIP = BaseMster.ClientIP;
        UserID = BaseMster.UserID;        
	}

    //check for a valid connection
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
            status.StatusComment = Resources.ErrorMessages.ERROR_CONN_NULL;
        }

        return status;
    }
}
