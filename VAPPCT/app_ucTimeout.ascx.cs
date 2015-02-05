using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.Data;

public partial class app_ucTimeout : CAppUserControlPopup
{
    public Timer AppTimeoutTimer;

    /// <summary>
    /// timeout
    /// </summary>
    public int Timeout
    {
        get
        {
            object obj = ViewState[ClientID + "Timeout"];
            return (obj != null) ? Convert.ToInt32(obj) : -1;
        }
        set { ViewState[ClientID + "Timeout"] = value; }
    }

    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Title = "Application Timeout";
        }
    }

    /// <summary>
    /// timeout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void tmrucTimeout_Tick(object sender, EventArgs e)
    {
        BaseMstr.AppUser.LogOff();
        Response.Redirect("VAPPCTHome.aspx");
    }

    /// <summary>
    /// load control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        CStatus status = new CStatus();
        
        //set the timeout
        tmrucTimeout.Interval = Timeout;

        //build the warning text
        string strWarning = String.Empty;

        strWarning = "WARNING: You will be logged off the ";
        strWarning += "application in ";
        long lMinutes = Convert.ToInt32(Timeout/60000);
        strWarning += Convert.ToString(lMinutes);
        strWarning += " minutes if you do not take action!";

        lblWarning.Text = strWarning;

        return status;
    }

    /// <summary>
    /// validate user input
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CStatus status = new CStatus();

        return status;
    }

    /// <summary>
    /// save the control
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = new CStatus();
        return status;
    }

    protected void btnStayLoggedIn_Click(object sender, EventArgs e)
    {

    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        BaseMstr.AppUser.LogOff();
        Response.Redirect("VAPPCTHome.aspx");
    }
}
