using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.Data;

public partial class app_ucWarningBanner : CAppUserControlPopup
{
    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Title = "Warning Banner";
        }

    }

    /// <summary>
    /// load control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        CStatus status = new CStatus();
        if (!status.Status)
        {
            return status;
        }

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



    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("app_warning_banner.htm");
    }
}
