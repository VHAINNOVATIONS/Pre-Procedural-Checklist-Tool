using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Text;
using System.Resources;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// code behind for the master page
/// </summary>
public partial class MasterPage : CBaseMaster
{
    /// <summary>
    /// are we in an async postback?
    /// </summary>
    public bool IsInAsyncPostBack
    {
        get { return tsmMASTER.IsInAsyncPostBack; }
    }

    /// <summary>
    /// sets the title displayed in the top right of 
    /// the master page
    /// </summary>
    public string PageTitle
    {
        set
        {
            divTitle.InnerHtml = value + "&nbsp;&nbsp;";
        }
    }

    /// <summary>
    /// page load, check login info and build menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //pass the mpe and basemaster to the warning banner control
        app_ucTimeout.MPE = mpeTimeout;
        app_ucTimeout.BaseMstr = this;
       
        tmrLogoffWarning.Interval = TimoutWarningInMiliseconds;
        
        //return if we are partially rendering
        if (tsmMASTER.IsInAsyncPostBack)
        {
            //just return, nothing more to do
            return;
        }
        
        if (!IsPostBack)
        {
            if (IsLoggedIn())
            {
                pnlNav.Visible = true;
                CAppMenu mnu = new CAppMenu();
                CStatus status = mnu.LoadMainMenu(this, mnuMain);
                if (!status.Status)
                {
                    ShowStatusInfo(status);
                }

                //show the users login info
                StringBuilder sbHTML = new StringBuilder();

                sbHTML.Append("<div style=\"font-size: xx-small;font-family:verdana,arial;\">" + UserFirstName + " " + UserLastName + ",");
                sbHTML.Append("<br />");
                sbHTML.Append("Logged In: " + CDataUtils.GetDateTimeAsString(UserLoginDateTime) + "</div>");

                divLoginInfo.InnerHtml = sbHTML.ToString();

                btnLogoff.Visible = true;
            }
            else
            {
                mnuMain.Items.Clear();
                btnLogoff.Visible = false;
            }
        }
    }

    /// <summary>
    /// user clicked the logoff button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        LogOff();
    }

    /// <summary>
    /// show status information
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    public void ShowStatusInfo(k_STATUS_CODE lStatusCode, string strStatusComment)
    {
        ShowStatusInfo(divMasterStatus, lStatusCode, strStatusComment);
    }

    /// <summary>
    /// Show Status Info
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    public void ShowStatusInfo(CStatus status)
    {
        ShowStatusInfo(divMasterStatus, status.StatusCode, status.StatusComment);
    }

    /// <summary>
    /// Show Status Info
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    public void ShowStatusInfo(k_STATUS_CODE lStatusCode, CParameterList pList)
    {
        ShowStatusInfo(divMasterStatus, lStatusCode, pList);
    }

    /// <summary>
    /// timer to show the user that they will be logged off if they do not
    /// take action
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void tmrLogoffWaring_Tick(object sender, EventArgs e)
    {
        if (AppUser.LoggedIn)
        {
            //load the timeout control
            app_ucTimeout.Timeout = TimoutAfterWarningInMiliseconds;//TimoutWarningInMiliseconds;
            app_ucTimeout.LoadControl(k_EDIT_MODE.INITIALIZE);
            app_ucTimeout.ShowMPE();

            //set the logoff warning to something greater so it does not fire again
            tmrLogoffWarning.Interval = TimeOutInMiliseconds;
        }
    }
}
