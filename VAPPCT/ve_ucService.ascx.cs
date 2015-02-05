using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Drawing;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ve_ucService : CAppUserControl
{
    private long ServiceID
    {
        get
        {
            object obj = ViewState[ClientID + "ServiceID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ServiceID"] = value; }
    }

    private DataTable Services
    {
        get
        {
            object obj = ViewState[ClientID + "Services"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "Services"] = value; }
    }

    /// <summary>
    /// property
    /// US:931
    /// stores the last sort expression used to sort the page's gridview
    /// </summary>
    private string SortExpression
    {
        get
        {
            object obj = ViewState[ClientID + "SortExpression"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "SortExpression"] = value; }
    }

    /// <summary>
    /// property
    /// US:931
    /// stores the last sort direction used to sort the page's gridview
    /// </summary>
    private SortDirection SortDirection
    {
        get
        {
            object obj = ViewState[ClientID + "SortDirection"];
            return (obj != null) ? (SortDirection)obj : SortDirection.Ascending;
        }
        set { ViewState[ClientID + "SortDirection"] = value; }
    }

    /// <summary>
    /// method
    /// registers the javascript required by the page
    /// </summary>
    private void RegisterJavaScript()
    {
        ClientScriptManager csm = Page.ClientScript;
        if (!csm.IsStartupScriptRegistered("InitPagers"))
        {
            StringBuilder sbInitPagers = new StringBuilder();
            sbInitPagers.Append("var prm = Sys.WebForms.PageRequestManager.getInstance();");
            sbInitPagers.Append("prm.add_pageLoaded(initPagers);");

            csm.RegisterStartupScript(GetType(), "InitPagers", sbInitPagers.ToString(), true);
        }
    }

    /// <summary>
    /// event
    /// enables the edit button if a service is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        btnEdit.Enabled = (ServiceID < 0) ? false : true;
    }

    /// <summary>
    /// event
    /// initializes the edit service dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucServiceEdit.BaseMstr = BaseMstr;
        ucServiceEdit.MPE = mpeServiceEdit;
        ucServiceEdit.GView = gvService;

        RegisterJavaScript();

        Page.LoadComplete += new EventHandler(Page_LoadComplete);
    }

    /// <summary>
    /// method
    /// loads the services into the service grid view
    /// </summary>
    /// <returns></returns>
    private CStatus LoadServices()
    {
        CServiceData service = new CServiceData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = service.GetServiceDS(k_ACTIVE_ID.All, out ds);
        if (!status.Status)
        {
            return status;
        }

        Services = ds.Tables[0];
        if (Services.Rows.Count == 0)
        {
            gvService.Width = 468;
        }
        gvService.DataSource = Services;
        gvService.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// override
    /// sets the edit mode and loads the services
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        CStatus status = LoadServices();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return status;
        }

        return status;
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
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// event
    /// initializes and displays the service edit control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickNew(object sender, EventArgs e)
    {
        CStatus status = ucServiceEdit.LoadControl(k_EDIT_MODE.INSERT);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucServiceEdit.ShowMPE();
    }

    /// <summary>
    /// event
    /// loads and displays the service edit control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickEdit(object sender, EventArgs e)
    {
        if (ServiceID < 1)
        {
            return;
        }

        ucServiceEdit.LongID = ServiceID;
        CStatus status = ucServiceEdit.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucServiceEdit.ShowMPE();
    }

    /// <summary>
    /// event
    /// reloads the service list and selects the changed item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSaveService(object sender, CAppUserControlArgs e)
    {
        CStatus status = LoadServices();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ServiceID = Convert.ToInt64(e.EventData);
        CGridView.SetSelectedRow(gvService, e.EventData);
        CGridView.SetSelectedLinkButtonForeColor(gvService, "lnkSelect", Color.White);
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvService.DataSource = Services;
        gvService.DataBind();

        CGridView.SetSelectedRow(gvService, ServiceID);
        CGridView.SetSelectedLinkButtonForeColor(gvService, "lnkSelect", Color.White);
    }

    /// <summary>
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedService(object sender, EventArgs e)
    {
        ShowMPE();

        foreach (GridViewRow gvr in gvService.Rows)
        {
            LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
            if (lnkSelect == null)
            {
                return;
            }

            lnkSelect.ForeColor = Color.Blue;
        }

        DataKey dk = gvService.SelectedDataKey;
        if (dk != null)
        {
            ServiceID = Convert.ToInt64(dk.Value);
            CGridView.SetSelectedLinkButtonForeColor(gvService, "lnkSelect", Color.White);
        }
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundService(Object sender, GridViewRowEventArgs e)
    {
        GridViewRow gvr = (GridViewRow)e.Row;
        if (gvr == null || gvr.RowType != DataControlRowType.DataRow)
        {
            return;
        }

        DataRowView drv = (DataRowView)gvr.DataItem;
        if (drv == null)
        {
            return;
        }

        DataRow dr = drv.Row;
        if (dr == null)
        {
            return;
        }

        LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
        Label lblActive = (Label)gvr.FindControl("lblActive");

        if (lnkSelect == null || lblActive == null)
        {
            return;
        }

        lnkSelect.Text = dr["service_label"].ToString();
        lblActive.Text = dr["active_label"].ToString();
    }

    /// <summary>
    /// event
    /// US:931
    /// sorts the page's gridview with respect to the clicked column in asc/desc order
    /// the first time a column is clicked the gridview is sorted in asc order
    /// if the column is clicked twice in a row the gridview is sorted in desc order
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSortingService(object sender, GridViewSortEventArgs e)
    {
        if (SortExpression == e.SortExpression)
        {
            SortDirection = (SortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
        }
        else
        {
            SortExpression = e.SortExpression;
            SortDirection = SortDirection.Ascending;
        }

        DataView dv = Services.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        Services = dv.ToTable();

        RebindAndSelect();
    }
}
