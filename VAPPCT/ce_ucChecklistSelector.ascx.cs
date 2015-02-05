using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ce_ucChecklistSelector : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _Select;
    /// <summary>
    /// property
    /// adds/removes event handlers to the select event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> Select
    {
        add { _Select += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Select -= value; }
    }

    /// <summary>
    /// cache the complete dataset for paging
    /// </summary>
    public DataTable ChecklistDataTable
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistDataTable"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "ChecklistDataTable"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a bool used to filter to checklist
    /// true = active checklists only; false = all checklists
    /// </summary>
    public bool ActiveChecklistsOnly
    {
        get
        {
            object obj = ViewState[ClientID + "ActiveChecklistsOnly"];
            return (obj != null) ? Convert.ToBoolean(obj) : false;
        }
        set { ViewState[ClientID + "ActiveChecklistsOnly"] = value; }
    }

    /// <summary>
    /// property
    /// stores the checklist id of the selected grid view row
    /// </summary>
    public long ChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistID"] = value; }
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
    /// initializes checklist selector dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterJavaScript();

        if (!IsPostBack)
        {
            Title = "Checklist Selector";
            
            chkFilterByCLService.Checked = false;
            ddlFilterByService.Enabled = false;

            chkFilterByCLName.Checked = false;
            txtFilterByCLName.Enabled = false;

            gvCL.DataSource = null;
            gvCL.DataBind();
        }
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvCL.DataSource = ChecklistDataTable;
        gvCL.DataBind();

        CGridView.SetSelectedRow(gvCL, ChecklistID);
        CGridView.SetSelectedLinkButtonForeColor(gvCL, "lnkSelect", Color.White);
    }

    /// <summary>
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedCL(object sender, EventArgs e)
    {
        ShowMPE();

        foreach (GridViewRow gvr in gvCL.Rows)
        {
            LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
            if (lnkSelect == null)
            {
                return;
            }

            lnkSelect.ForeColor = Color.Blue;
        }

        DataKey dk = gvCL.SelectedDataKey;
        if (dk != null)
        {
            ChecklistID = Convert.ToInt64(dk.Value);
            CGridView.SetSelectedLinkButtonForeColor(gvCL, "lnkSelect", Color.White);
        }
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundCL(Object sender, GridViewRowEventArgs e)
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
        Label lblService = (Label)gvr.FindControl("lblService");

        if (lnkSelect == null || lblService == null)
        {
            return;
        }

        lnkSelect.Text = dr["CHECKLIST_LABEL"].ToString();
        lblService.Text = dr["SERVICE_LABEL"].ToString();
    }

    /// <summary>
    /// method
    /// loads the service drop down list
    /// </summary>
    /// <returns></returns>
    private CStatus LoadServiceDDL()
    {
        CStatus status = CService.LoadServiceDDL(
                BaseMstr.BaseData,
                k_ACTIVE_ID.All,
                ddlFilterByService);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads the checklist grid view with the search results
    /// </summary>
    /// <param name="strChecklistName"></param>
    /// <param name="lServiceID"></param>
    /// <returns></returns>
    private CStatus LoadChecklistSearch(string strChecklistName, long lServiceID)
    {
        //get the data
        DataSet ds = null;
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        CStatus status = cld.GetCheckListSearchDS(strChecklistName, lServiceID, ActiveChecklistsOnly, out ds);
        if(!status.Status)
        {
            return status;
        }

        ChecklistDataTable = ds.Tables[0];

        gvCL.PageIndex = 0;
        gvCL.SelectedIndex = -1;
        gvCL.EmptyDataText = "No result(s) found.";
        gvCL.DataSource = ChecklistDataTable;
        gvCL.DataBind();

        btnSearch.Focus();

        return new CStatus();
    }

    /// <summary>
    /// override
    /// loads drop downs and sets focus
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        CStatus status = LoadServiceDDL();
        if (!status.Status)
        {
            return status;
        }

        chkFilterByCLName.Focus(); 
        return new CStatus();
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
    /// raises an event with the selected checklist id
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSelect(object sender, EventArgs e)
    {
        if (ChecklistID < 1)
        {
            ShowMPE();
            return;
        }

        if (_Select != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                ChecklistID.ToString());

            _Select(this, args);
        }

        ShowParentMPE();
    }

    /// <summary>
    /// event
    /// does nothing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {
        ShowParentMPE();
    }

   /// <summary>
   /// event
   /// calls the checklist search method with the user's filters
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    protected void OnClickSearch(object sender, EventArgs e)
    {
        ShowMPE();
        string strFilterByName = string.Empty;
        long lServiceID = -1;

        if (chkFilterByCLName.Checked)
        {
            strFilterByName = txtFilterByCLName.Text;
        }

        if (chkFilterByCLService.Checked)
        {
            lServiceID = Convert.ToInt64(ddlFilterByService.SelectedValue);
        }

        CStatus status = LoadChecklistSearch(strFilterByName, lServiceID);
        if (!status.Status)
        {
            ShowStatusInfo(status);
        }
    }

    /// <summary>
    /// event
    /// toggles the enabled state of name control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnCheckedChangedCLName(object sender, EventArgs e)
    {
        ShowMPE();
        txtFilterByCLName.Enabled = chkFilterByCLName.Checked;
        chkFilterByCLName.Focus();
    }

    /// <summary>
    /// event
    /// toggles the enabled state of the service control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnCheckedChangedCLService(object sender, EventArgs e)
    {
        ShowMPE();
        ddlFilterByService.Enabled = chkFilterByCLService.Checked;
        chkFilterByCLService.Focus();
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
    protected void OnSortingCL(object sender, GridViewSortEventArgs e)
    {
        ShowMPE();
        if (SortExpression == e.SortExpression)
        {
            SortDirection = (SortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
        }
        else
        {
            SortExpression = e.SortExpression;
            SortDirection = SortDirection.Ascending;
        }

        DataView dv = ChecklistDataTable.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        ChecklistDataTable = dv.ToTable();

        RebindAndSelect();
    }

    protected void OnClickCollapseFilters(object sender, EventArgs e)
    {
        ShowMPE();
        pnlFilters.Visible = !pnlFilters.Visible;
        if (pnlFilters.Visible)
        {
            btnCollapseFilters.Text = "-";
            pnlCLgv.Height = 250;
        }
        else
        {
            btnCollapseFilters.Text = "+";
            pnlCLgv.Height = 381;
        }
    }
}
