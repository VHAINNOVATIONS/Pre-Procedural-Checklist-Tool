using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;
using System.Security;
using System.Configuration;

//MDWS web service
using VAPPCT.Data.MDWSEmrSvc;

/// <summary>
/// Summary description for CAppUser
/// </summary>
public class CAppUser : CData
{
    /// <summary>
    /// Constructor for CAppUSer class
    /// </summary>
    /// <param name="BaseMaster"></param>
    public CAppUser(CData Data)
        : base(Data)
    {
        //constructors are not inherited in c#!
    }
    
    /// <summary>
    /// audits page access and determines whether the user is allowed access
    /// </summary>
    /// <param name="strPageName"></param>
    /// <returns></returns>
    public bool AuditPageAccess(string strPageName)
    {
        CStatus status = new CStatus();
        CUserData ud = new CUserData(this);
        status = ud.AuditPageAccess(strPageName);
        return status.Status;
    }

    /// <summary>
    /// does the user have a specific key by name?
    /// </summary>
    /// <param name="strKeyName"></param>
    /// <returns></returns>
    public bool HasSecurityKey(string strKeyName)
    {
        CSecurityKeyData keyData = new CSecurityKeyData(this);
        return keyData.HasSecurityKey(UserID, strKeyName);
    }
        
    EmrSvcSoapClient m_EmrSvcSoapClient = null;
    /// <summary>
    /// gets the soap client so we can talk to mdws
    /// </summary>
    /// <returns></returns>
    EmrSvcSoapClient GetMDWSSOAPClient()
    {
        //this is used from communicator also and we 
        //have no "Session" in communicator
        if (WebSession != null)
        {
            if (WebSession["EmrSvcSoapClient"] == null)
            {
                m_EmrSvcSoapClient = new EmrSvcSoapClient("EmrSvcSoap");
                WebSession["EmrSvcSoapClient"] = m_EmrSvcSoapClient;
            }

            return (EmrSvcSoapClient)WebSession["EmrSvcSoapClient"];
        }
        else
        {
            if (m_EmrSvcSoapClient == null)
            {
                m_EmrSvcSoapClient = new EmrSvcSoapClient("EmrSvcSoap");
            }

            return m_EmrSvcSoapClient;
        }
    }
    /// <summary>
    /// US:866 US:840
    /// Log off the checklist application, disconnects from MDWS and clears session
    /// </summary>
    public void LogOff()
    {
        //clear the session
        //todo: remove commented out section for final production release
        //CUserData cud = new CUserData(this);
        //CStatus status = cud.ClearFXSession();
        
        //disconnect from MDWS if we are connected tos MDWS
        if (MDWSTransfer)
        {
            GetMDWSSOAPClient().disconnect();
        }

        //clear properties
        UserID = 0;
        UserLastName = string.Empty;
        UserRoleID = 0;
        UserFirstName = string.Empty;

        //clear the permissions
        IsAdministrator = false;
        IsNurse = false;
        IsDoctor = false;

        //not logged in
        LoggedIn = false;
    }

    bool m_bLoggedIn = false;
    /// <summary>
    /// Property to determine if the user is logged in
    /// </summary>
    public bool LoggedIn
    {
        set
        {
            m_bLoggedIn = value;
            WebSession["USR_LOGGEDIN"] = m_bLoggedIn;
        }
        get
        {
            //no user id we are not logged in
            //LogOff will clean up connections etc...
            if (UserID < 1)
            {
                LogOff();

                m_bLoggedIn = false;
                return m_bLoggedIn;
            }

            if (WebSession["USR_LOGGEDIN"] == null)
            {
                m_bLoggedIn = false;
                WebSession["USR_LOGGEDIN"] = m_bLoggedIn;
            }
            else
            {
                m_bLoggedIn = (bool)WebSession["USR_LOGGEDIN"];
            }

            return m_bLoggedIn;
        }
    }

    /// <summary>
    /// Property to hold the user id of the user currently logged in
    /// </summary>
    new public long UserID
    {
        get
        {
            if (WebSession["USR_ID"] == null)
            {
                return 0;
            }

            long lUserID = CDataUtils.ToLong(WebSession["USR_ID"].ToString());
            base.UserID = lUserID;
            
            return lUserID;
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_ID"] = value;
            base.UserID = value;
        }
    }

    /// <summary>
    /// US:866 Property to hold whether or not the user has admin privs
    /// </summary>
    public bool IsAdministrator
    {
        get
        {
            if (WebSession["USR_IS_ADMIN"] == null)
            {
                return false;
            }

            long lValue = CDataUtils.ToLong(WebSession["USR_IS_ADMIN"].ToString());
            if (lValue > 0)
            {
                return true;
            }

            return false;
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_IS_ADMIN"] = 0;
            if (value)
            {
                WebSession["USR_IS_ADMIN"] = 1;
            }
        }
    }

    /// <summary>
    /// US:866 Property to hold whether or not the user has doctor privs
    /// </summary>
    public bool IsDoctor
    {
        get
        {
            if (WebSession["USR_IS_DOC"] == null)
            {
                return false;
            }

            long lValue = CDataUtils.ToLong(WebSession["USR_IS_DOC"].ToString());
            if (lValue > 0)
            {
                return true;
            }

            return false;
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_IS_DOC"] = 0;
            if (value)
            {
                WebSession["USR_IS_DOC"] = 1;
            }
        }
    }

    /// <summary>
    /// US:866 Property to hold whether or not the user has nurse privs
    /// </summary>
    public bool IsNurse
    {
        get
        {
            if (WebSession["USR_IS_NURSE"] == null)
            {
                return false;
            }

            long lValue = CDataUtils.ToLong(WebSession["USR_IS_NURSE"].ToString());
            if (lValue > 0)
            {
                return true;
            }

            return false;
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_IS_NURSE"] = 0;
            if (value)
            {
                WebSession["USR_IS_NURSE"] = 1;
            }
        }
    }

    /// <summary>
    /// Property to hold the role id of the user logged in
    /// </summary>
    public long UserRoleID
    {
        get
        {
            if (WebSession["USR_ROLE_ID"] == null)
            {
                return 0;
            }

            return CDataUtils.ToLong(WebSession["USR_ROLE_ID"].ToString());
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_ROLE_ID"] = value;
        }
    }

    //holds the site id
    public long SiteID
    {
        get
        {
            if (WebSession["USR_SITE_ID"] == null)
            {
                return 0;
            }

            return CDataUtils.ToLong(WebSession["USR_SITE_ID"].ToString());
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_SITE_ID"] = value;
        }
    }

    //private member to hold the UID
    private string m_MDWSUID;
    
    /// <summary>
    /// US:1882 cached, encrypted MDWS UID for re-connecting to MDWS
    /// if we timeout. implemented this way per Joel.
    /// </summary>
    public string MDWSUID
    {
        get
        {
            if (WebSession["MDWSUID"] == null)
            {
                m_MDWSUID = null;
                return m_MDWSUID;
            }
            else
            {
                m_MDWSUID = Convert.ToString(WebSession["MDWSUID"]);
            }
          
            //decrypt the UID
            return CDataUtils.dec(m_MDWSUID,
                              ConfigurationSettings.AppSettings["KEY"],
                              ConfigurationSettings.AppSettings["IV"]); 
        }
        set
        {
            //encrypt the UID
            m_MDWSUID = CDataUtils.enc(value,
                                   ConfigurationSettings.AppSettings["KEY"],
                                   ConfigurationSettings.AppSettings["IV"]);

            //keep the encrypted UID in session state
            WebSession["MDWSUID"] = m_MDWSUID;
        }
    }

    //private member to hold the //private member to hold the UID
    private string m_MDWSPWD;
    
    /// <summary>
    /// US:1882 cached, encrypted MDWS UID for re-connecting to MDWS
    /// if we timeout. implemented this way per Joel.
    /// </summary>
    public string MDWSPWD
    {
        get
        {
            if (WebSession["MDWSPWD"] == null)
            {
                m_MDWSPWD = null;
                return m_MDWSPWD;
            }
            else
            {
                m_MDWSPWD = Convert.ToString(WebSession["MDWSPWD"]);
            }

            //decrypt the PWD
            return CDataUtils.dec(m_MDWSPWD,
                              ConfigurationSettings.AppSettings["KEY"],
                              ConfigurationSettings.AppSettings["IV"]); 
        }
        set
        {
            //encrypt the UID
            m_MDWSPWD = CDataUtils.enc(value,
                                   ConfigurationSettings.AppSettings["KEY"],
                                   ConfigurationSettings.AppSettings["IV"]);

            //keep the encrypted UID in session state
            WebSession["MDWSPWD"] = m_MDWSPWD;
        }
    }


    /// <summary>
    /// property to hold the first name of the user logged in
    /// </summary>
    public string UserFirstName
    {
        get
        {
            if (WebSession["USR_FIRST_NAME"] == null)
            {
                return String.Empty;
            }

            return Convert.ToString(WebSession["USR_FIRST_NAME"]);
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_FIRST_NAME"] = value;
        }
    }

    /// <summary>
    /// Property to hold the last name of the user logged in
    /// </summary>
    public string UserLastName
    {
        get
        {
            if (WebSession["USR_LAST_NAME"] == null)
            {
                return String.Empty;
            }

            return Convert.ToString(WebSession["USR_LAST_NAME"]);
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_LAST_NAME"] = value;
        }
    }

    /// <summary>
    /// Property to hold the date time the user logged in
    /// </summary>
    public DateTime UserLoginDateTime
    {
        get
        {
            if (WebSession["USR_LOGIN_DATE"] == null)
            {
                return DateTime.Now;
            }

            return Convert.ToDateTime(WebSession["USR_LOGIN_DATE"]);
        }

        //can only be set in this class via login so its private!
        private set
        {
            WebSession["USR_LOGIN_DATE"] = value;
        }
    }

    /// <summary>
    /// US:866 Load the user data from the database
    /// </summary>
    /// <param name="lUserID"></param>
    /// <returns></returns>
    private CStatus LoadUserData(long lUserID)
    {
        DataSet ds = null;

        CUserData cud = new CUserData(this);
        CStatus status = cud.GetUserDS(lUserID, out ds);
        if (status.Status)
        {
            //cach the date the user logged in
            UserLoginDateTime = DateTime.Now;

            //cache the user id, role id, first name and last name
            UserID = lUserID;
            UserRoleID = CDataUtils.GetDSLongValue(ds, "user_role_id");
            UserFirstName = CDataUtils.GetDSStringValue(ds, "first_name");
            UserLastName = CDataUtils.GetDSStringValue(ds, "last_name");
        }
        else
        {
            return status;
        }

        //transfer user keys to our db
        if (MDWSTransfer)
        {
            CMDWSOps ops = new CMDWSOps(this);
            long lCount = 0;
            status = ops.GetMDWSSecurityKeys(lUserID, true, out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        //set the admin, doc and nurse privs for this user
        DataSet dsRoles = null;
        status = cud.GetUserRolesDS(lUserID, out dsRoles);
        if (status.Status)
        {
            foreach (DataTable table in dsRoles.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    long lRoleID = CDataUtils.GetDSLongValue(dr, "USER_ROLE_ID");
                    if (lRoleID == (long)k_USER_ROLE_ID.Administrator)
                    {
                        IsAdministrator = true;
                    }
                    else if (lRoleID == (long)k_USER_ROLE_ID.Doctor)
                    {
                        IsDoctor = true;
                    }
                    else if (lRoleID == (long)k_USER_ROLE_ID.Nurse)
                    {
                        IsNurse = true;
                    }
                }
           }
        }
            
        return status;
    }
    
    /// <summary>
    /// US:840 US:1882 US:836 US:866
    /// login to the checklist tool
    /// </summary>
    /// <param name="strUID"></param>
    /// <param name="strPWD"></param>
    /// <returns></returns>
    public CStatus Login(string strUID, 
                         string strPWD,
                         long lSiteID)
    {
        //status
        CStatus status = new CStatus();
        long lUserID = 0;
        LoggedIn = false;

        //login to mdws if we are connecting to mdws
        if (MDWSTransfer)
        {
            EmrSvcSoapClient mdwsSOAPClient = null;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.MDWSLogin(
                strUID,
                strPWD,
                lSiteID,
                out lUserID,
                out mdwsSOAPClient);

            if (status.Status)
            {
                //create the session record
                UserID = lUserID;
                CUserData ud = new CUserData(this);
                string strFXSessionID = String.Empty;
                status = ud.CreateFXSession(out strFXSessionID);
                if (!status.Status)
                {
                    return status;
                }

                //load the rest of the user data
                status = LoadUserData(lUserID);
                if (!status.Status)
                {
                    return status;
                }
                
                //we are loged in at this point
                LoggedIn = true;

                //cache the encrypted login credentials so that we can re-login if
                //we timeout from MDWS.
                MDWSUID = strUID;
                MDWSPWD = strPWD;
                SiteID = lSiteID;
            }

            return status;
        }

        //simple login 
        long lRoleID = 0;

        DataSet ds = null;
        CUserData cud = new CUserData(this);
        status = cud.GetLoginUserDS(
            strUID,
            strPWD,
            out ds,
            out lUserID,
            out lRoleID);
        if (status.Status)
        {
            //create the session record
            UserID = lUserID;
            CUserData ud = new CUserData(this);
            string strFXSessionID = String.Empty;
            status = ud.CreateFXSession(out strFXSessionID);
            if (!status.Status)
            {
                return status;
            }

            //load the rest of the user date
            status = LoadUserData(lUserID);
            if (!status.Status)
            {
                return status;
            }

            //we are loged in at this point
            LoggedIn = true;
        }

        return status;
    }

    /// <summary>
    /// US:1882 Check to see if we have a valid MDWS connection
    /// </summary>
    /// <returns></returns>
    public CStatus CheckMDWSConnection()
    {
        //performing a simple operation in MDWS 
        //to make sure we are still connected.

        CStatus status = new CStatus();
       
        //todo: forcing a disconnect for testing
        //GetMDWSSOAPClient().disconnect();

        CMDWSOps ops = new CMDWSOps(this);
        long lCount = 0;
        status = ops.GetMDWSSecurityKeys(UserID, false, out lCount);

        if (!status.Status)
        {
            long lUserID = 0;
            EmrSvcSoapClient mdwsSOAPClient = null;
            status = ops.MDWSLogin(MDWSUID.ToString(),
                                   MDWSPWD.ToString(),
                                   SiteID,
                                   out lUserID,
                                   out mdwsSOAPClient);
        }


        return status;
    }

}
