using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class app_ucLogin : CAppUserControlPopup
{
    /// <summary>
    /// event
    /// sets dialog title
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Login";
        txtUID.Focus();

        if (ddlSite.Items.Count < 1)
        {   
            DataSet dsRegion = null;
            DataSet dsSite = null;

            CSiteData siteData = new CSiteData(BaseMstr.BaseData);
            
            //get the regions
            CStatus status = siteData.GetRegionDS(out dsRegion);
            CDropDownList.RenderDataSet(dsRegion,
                                        ddlRegion,
                                        "REGION_NAME",
                                        "REGION_ID");

            //select the default region if set
            long lRegionID = 0;
            if (System.Configuration.ConfigurationManager.AppSettings["MDWSEmrSvcRegionID"] != null)
            {
                string strRegion = System.Configuration.ConfigurationManager.AppSettings["MDWSEmrSvcRegionID"].ToString();
                lRegionID = CDataUtils.ToLong(strRegion);
                CDropDownList.SelectItemByValue(ddlRegion,
                                                lRegionID);
            }

            //get all sites for this region
            status = siteData.GetSiteDS(lRegionID, out dsSite);
            CDropDownList.RenderDataSet(dsSite,
                                        ddlSite,
                                        "SITE_NAME",
                                        "SITE_ID");

            long lSiteID = 0;
            if (System.Configuration.ConfigurationManager.AppSettings["MDWSEmrSvcSiteList"] != null)
            {
                string strSite = System.Configuration.ConfigurationManager.AppSettings["MDWSEmrSvcSiteList"].ToString();
                lSiteID = CDataUtils.ToLong(strSite);
                CDropDownList.SelectItemByValue(ddlSite,
                                                lSiteID);
            }
        }
    }

    /// <summary>
    /// US:840
    /// event
    /// validates username and password fields
    /// redirects to the home page if successful
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOKLogin_Click(object sender, EventArgs e)
    {
        //login to the application through basemaster
        CStatus status = BaseMstr.AppUser.Login(txtUID.Text, 
                                                txtPWD.Text,
                                                CDataUtils.ToLong(ddlSite.SelectedValue));
        if (!status.Status)
        {
            //this error seems to occur if the user gets in a state where 
            //they are already logged in but try to login again. To avoid this
            //we disconnect and try one more time if we get this error
            if (status.StatusComment.ToLower().IndexOf("the remote procedure ") != -1)
            {
                BaseMstr.AppUser.LogOff();
                status = BaseMstr.AppUser.Login(txtUID.Text, 
                                                txtPWD.Text,
                                                CDataUtils.ToLong(ddlSite.SelectedValue));
            }

            //if we fail then show the error
            if (!status.Status)
            {
                ShowStatusInfo(status);
                ShowMPE();
                return;
            }
        }

        //login was successful so redirect
        Response.Redirect("VAPPCTHome.aspx");
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput( out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// selected region was changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            return;
        }

        if (ddlRegion.SelectedIndex != -1)
        {
            ddlSite.Items.Clear();

            DataSet dsSite = null;
            long lRegionID = CDataUtils.ToLong(ddlRegion.SelectedValue);
            CSiteData siteData = new CSiteData(BaseMstr.BaseData);

            //get all sites for this region
            siteData.GetSiteDS(lRegionID, out dsSite);
            CDropDownList.RenderDataSet(dsSite,
                                        ddlSite,
                                        "SITE_NAME",
                                        "SITE_ID");
        }

        ShowMPE();
    }
}
