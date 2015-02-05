using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.Web.SessionState;

//our data access class library
using VAPPCT.DA;

//
//This is our base master page, all other master pages will derive from this
//master page. This page will handle db connection and cleanup etc...
//
//By defining the BaseMasterPage class as abstract, it can't be 
//created directly and can only serve as the base for another class.
//
//sequence of events for reference...
//Master page controls Init event
//Content controls Init event
//
//Master page Init event
//Content page Init event
//
//Content page Load event
//Master page Load event
//
//Content page PreRender event
//Master page PreRender event
//
//Master page controls PreRender event
//Content controls PreRender event
//

/// <summary>
/// This class is a base class for all master pages
/// </summary>
public abstract class CBaseMaster : System.Web.UI.MasterPage
{
    public CAppUser AppUser;
    public CData BaseData;

    /// <summary>
    /// session object for use from other places
    /// </summary>
    public HttpSessionState UserSession
    {
        get { return Session; }
    }

    /// <summary>
    /// US:1882 application timeout in minutes
    /// </summary>
    public int TimeOutInMinutes
    {
        get { return 20; }
        //get { return 5; }
    }

    /// <summary>
    /// US:1882 application timeout in miliseconds
    /// </summary>
    public int TimeOutInMiliseconds
    {
        //1 minutes = 60,000 milliseconds
        get { return TimeOutInMinutes * 60000; }
    }

    /// <summary>
    /// US:1882 timeout for warning the user they are about to be logged off
    /// </summary>
    public int TimoutWarningInMiliseconds 
    {
        //3 minutes before timeout
        get { return TimeOutInMiliseconds - (3 * 60000); }
    }

    /// <summary>
    /// final timout after warning timeout
    /// </summary>
    public int TimoutAfterWarningInMiliseconds
    {
        //3 minutes before timeout
        get { return TimeOutInMiliseconds - (TimeOutInMiliseconds - (3 * 60000)); }
    }

    private bool m_bMDWSTransfer = false;
    
    /// <summary>
    /// is MDWS on
    /// </summary>
    public bool MDWSTransfer
    {
        get { return m_bMDWSTransfer; }
        set { m_bMDWSTransfer = value; }
    }
        
    private CAppDBConn m_DBConn;
    
    /// <summary>
    /// database connection
    /// </summary>
    public CAppDBConn DBConn
    {
        get
        {
            if (m_DBConn != null)
            {
                return m_DBConn;
            }

            return null;
        }
    }
    
    /// <summary>
    /// constructor
    /// </summary>
	public CBaseMaster()
	{
		//create a new dabase connection object
        m_DBConn = new CAppDBConn();
   	}

    /// <summary>
    /// override page load
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        // csrf protection on post backs only
        //one and only appuser holds data in session state
        AppUser = new CAppUser(BaseData);

        //check token if we are logged in
        if (AppUser.LoggedIn)
        {
            //using a session token to prevent against CSRF attacks
            //
            //if no token yet create one
            if (Session["CSRF_TOKEN"] == null)
            {
                Session["CSRF_TOKEN"] = Guid.NewGuid().ToString();
            }

            if (!IsPostBack)
            {
                //not a postback so put the token in view state
                ViewState["CSRF_TOKEN"] = Session["CSRF_TOKEN"].ToString();
            }
            else
            {
                //post back so check the token
                //
                //logoff if null
                if (ViewState["CSRF_TOKEN"] == null)
                {
                    LogOff();
                    return;
                }

                //logoff if session token and viewstate totken do not match
                string strSessionCSRF = Session["CSRF_TOKEN"].ToString();
                string strViewStateCSRF = ViewState["CSRF_TOKEN"].ToString();
                if (strSessionCSRF != strViewStateCSRF)
                {
                    LogOff();
                    return;
                }
            }
        }

        //call base to finish the load
        base.OnLoad(e);
    }

    /// <summary>
    /// get client ip
    /// </summary>
    public string ClientIP
    {
        get { return Context.Request.ServerVariables["REMOTE_ADDR"]; }
    }

    /// <summary>
    /// helper to get the current page name
    /// </summary>
    /// <returns></returns>
    public string GetPageName()
    {
        string strPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
        System.IO.FileInfo oInfo = new System.IO.FileInfo(strPath);

        return oInfo.Name.ToLower();
    }

    /// <summary>
    /// get the version of the application
    /// </summary>
    public string Version
    {
        get 
        { 
            return System.Reflection.Assembly.Load("App_Code").GetName().Version.ToString(); 
        }
    }

    /// <summary>
    /// proper place to close connections etc...
    /// </summary>
    public override void Dispose()
    {
        //close the database connection
        if (m_DBConn != null)
        {
            m_DBConn.Close();
        }

        base.Dispose();
    }

    /// <summary>
    /// get/set status code info
    /// </summary>
    private k_STATUS_CODE m_lStatusCode;
    public k_STATUS_CODE StatusCode
    {
        get 
        { 
            return m_lStatusCode; 
        }
        set
        { 
            Session["StatusCode"] = value;//todo
            m_lStatusCode = value;
        }
    }

    /// <summary>
    /// get/set status comment info
    /// </summary>
    private string m_strStatusComment;
    public string StatusComment
    {
        get 
        {
            return m_strStatusComment; 
        }
        set
        { 
            Session["StatusComment"] = value;//todo
            m_strStatusComment = value;
        }
    }

    /// <summary>
    /// clear the status, called by the masterpage after we 
    /// display status info
    /// </summary>
    public void ClearStatusInfo()
    {
        Session["StatusComment"] = string.Empty;//todo
        Session["StatusCode"] = 0;//todo

        m_strStatusComment = string.Empty;
        m_lStatusCode = k_STATUS_CODE.Success;

    }

    /// <summary>
    /// US:836 US:1882 this is the proper place to do initialization in a master page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        CStatus status = new CStatus();
        
        //Returns a string that can be used in a client 
        //event to cause postback to the server. 
        Page.ClientScript.GetPostBackEventReference(this, String.Empty);

        //set the character set, since all pages derive from basemaster
        //this will set the encoding for all pages...
        Response.ContentEncoding = Encoding.UTF8;

        //is MDWS on?
        MDWSTransfer = false;
        if (System.Configuration.ConfigurationManager.AppSettings["MDWSTransfer"] != null)
        {
            string strMDWS = System.Configuration.ConfigurationManager.AppSettings["MDWSTransfer"].ToString();
            if (strMDWS == "1")
            {
                MDWSTransfer = true;
            }
        }

        //connect to the data source
        if (m_DBConn != null)
        {
            status = m_DBConn.Connect();
            if (!status.Status)
            {
                //redirect to an error page
                Response.Redirect("ep_error_page.htm");
                Response.End();
            }
        }

        //one and only basedata, used as a base class for all
        //data items. allows us to share data classes from 
        //VAPPCT.Data
        BaseData = new CData(DBConn,
                             ClientIP,
                             UserID,
                             SessionID, 
                             Session,
                             MDWSTransfer);

        //one and only appuser holds data in session state
        AppUser = new CAppUser(BaseData);

        //because basedata gets set before appuser and 
        //app user id is stored in session state we must
        //set it here again to avoid a 0 user id if the 
        //user is logged in...
        BaseData.UserID = AppUser.UserID;
        
        //timeout for the application is 20 minutes
        Session.Timeout = TimeOutInMinutes;

        //check for a valid session
        if (AppUser.LoggedIn)
        {
            CUserData ud = new CUserData(BaseData);
            status = ud.CheckFXSession();
            if (!status.Status)
            {
                LogOff();
                return;
            }
        }

        //if we are not logged in and not on the home page
        //redirct to the home page...
        if (!AppUser.LoggedIn)
        {
            if (GetPageName().ToLower() != "vappcthome.aspx")
            {
                Response.Redirect("VAPPCTHome.aspx");
            }
        }
        else
        {   
            //if we are logged in but not connected to mdws 
            //then reconnect
            if (MDWSTransfer)
            {
                CStatus s = new CStatus();
                s = AppUser.CheckMDWSConnection();
            }
        }

        
        //if the user does not have access to this page
        //then logg them off
        if (GetPageName().ToLower() != "vappcthome.aspx")
        {
            if (!AppUser.AuditPageAccess(GetPageName()))
            {
                LogOff();
            }
        }
    }

    
    /// <summary>
    /// use this check to see if the user clicked the 
    /// applications main "Save" button if one exists...
    /// </summary>
    /// <returns></returns>
    public bool OnMasterSAVE()
    {
        //get the postback control
        string strPostBackControl = Request.Params["__EVENTTARGET"];
        if (strPostBackControl != null)
        {
            //did we do a patient lookup?
            if (strPostBackControl.Equals("MASTER_SAVE"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// is the user logged in. implemented as a function so that it 
    /// can be enhanced later.
    /// </summary>
    /// <returns></returns>
    public bool IsLoggedIn()
    {
        if (AppUser == null)
        {
            return false;
        }

        return AppUser.LoggedIn;
    }

    /// <summary>
    /// logoff the system
    /// </summary>
    /// <returns></returns>
    public bool LogOff()
    {
        if (AppUser != null)
        {
            AppUser.LogOff();
            AppUser = new CAppUser(this.BaseData);
        }
        
        Response.Redirect("VAPPCTHome.aspx");

        return true;
    }
    
    /// <summary>
    /// user id of the user currently logged in
    /// </summary>
    public long UserID
    {
        get
        {
            long lUserID = 0;
            if (AppUser != null)
            {
                lUserID = AppUser.UserID;
            }

            return lUserID;
        }
    }

    /// <summary>
    /// role id of the user logged in
    /// </summary>
    public long UserRoleID
    {
        get
        {
            long lRoleID = 0;
            if (AppUser != null)
            {
                lRoleID = AppUser.UserRoleID;
            }

            return lRoleID;
        }
    }

    /// <summary>
    /// first name of the user logged in
    /// </summary>
    public string UserFirstName
    {
        get
        {
            string strFirstName = String.Empty;
            if (AppUser != null)
            {
                strFirstName = AppUser.UserFirstName;
            }

            return strFirstName;
        }
    }

    /// <summary>
    /// date time the user logged in
    /// </summary>
    public DateTime UserLoginDateTime
    {
        get
        {
            DateTime dt = new DateTime();
            if (AppUser != null)
            {
                dt = AppUser.UserLoginDateTime;
            }
            return dt;
        }
    }

    /// <summary>
    /// last name of the user logged in
    /// </summary>
    public string UserLastName
    {
        get
        {
            string strLastName = String.Empty;
            if (AppUser != null)
            {
                strLastName = AppUser.UserLastName;
            }

            return strLastName;
        }
    }

    /// <summary>
    /// our session id is the same as the asp.net session id
    /// when the user logs in a session record is created in the 
    /// database.
    /// </summary>
    public string SessionID
    {
        get
        {
            return Session.SessionID;
        }
    }

    /// <summary>
    /// method
    /// shows status information
    /// </summary>
    /// <param name="divStatus"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    public void ShowStatusInfo(
        HtmlGenericControl divStatus,
        k_STATUS_CODE lStatusCode,
        string strStatusComment)
    {
        StatusCode = lStatusCode;
        StatusComment = strStatusComment;

        if (string.IsNullOrEmpty(strStatusComment))
        {
            divStatus.InnerHtml = string.Empty;
            return;
        }

        StringBuilder sbHTML = new StringBuilder();
        sbHTML.Append("<span style=\"font-family: verdana,arial; color: ");
        sbHTML.Append((lStatusCode == k_STATUS_CODE.Success) ? "darkgreen" : "darkred");
        sbHTML.Append(";\">");
        sbHTML.Append(Server.HtmlEncode(strStatusComment));
        sbHTML.Append("</span><br /><br />");

        if (divStatus != null)
        {
            divStatus.InnerHtml = sbHTML.ToString();
        }
    }

    /// <summary>
    /// method
    /// shows status information
    /// </summary>
    /// <param name="divStatus"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    public void ShowStatusInfo(
        HtmlGenericControl divStatus,
        k_STATUS_CODE lStatusCode,
        CParameterList plistStatus)
    {
        StatusCode = lStatusCode;
        StatusComment = string.Empty;

        if (plistStatus.Count < 1)
        {
            divStatus.InnerHtml = string.Empty;
            return;
        }

        StringBuilder sbHTML = new StringBuilder();
        sbHTML.Append("<table cellpadding=\"2\" width=\"99%\"><tr><td>");
        sbHTML.Append("<span style=\"font-family: verdana,arial; color: ");
        sbHTML.Append((lStatusCode == k_STATUS_CODE.Success) ? "darkgreen" : "darkred");
        sbHTML.Append(";\">");

        for (int i = 0; i < plistStatus.Count; i++)
        {
            sbHTML.Append(Server.HtmlEncode(plistStatus[i].ToString()));
            sbHTML.Append("<div class=\"app_horizontal_spacer\"></div>");
        }

        sbHTML.Append("</span></td></tr></table><div class=\"app_horizontal_spacer\"></div>");

        if (divStatus != null)
        {
            divStatus.InnerHtml = sbHTML.ToString();
        }
    }
}
