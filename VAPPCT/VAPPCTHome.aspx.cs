using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// code behind for the main Home page
/// </summary>
public partial class VAPPCTHome : System.Web.UI.Page
{
    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //pass the mpe and basemaster to the login control
        app_ucLogin.MPE = mpeLogin;
        app_ucLogin.BaseMstr = Master;

        //pass the mpe and basemaster to the warning banner control
        app_ucWarningBanner.MPE = mpeWarningBanner;
        app_ucWarningBanner.BaseMstr = Master;

        //hide the login button if we are not logged in
        if (Master.IsLoggedIn())
        {
            btnLogin.Visible = false;
        }

        //if this is the first time we access the page, 
        //then load the user control
        if (!IsPostBack)
        {
            Master.PageTitle = "Home";
            if (!Master.IsLoggedIn())
            {
                app_ucWarningBanner.ShowMPE();
            }
        }
    }

    /// <summary>
    /// user clicked the login button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickLogin(object sender, EventArgs e)
    {
        app_ucLogin.ShowMPE();
    }
}
