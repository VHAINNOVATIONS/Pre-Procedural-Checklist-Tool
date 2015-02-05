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

public partial class ve_ucItemGroup : CAppUserControl
{
    private long ItemGroupID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemGroupID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ItemGroupID"] = value; }
    }

    private DataTable ItemGroups
    {
        get
        {
            object obj = ViewState[ClientID + "ItemGroups"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "ItemGroups"] = value; }
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
    /// load item groups
    /// </summary>
    /// <returns></returns>
    private CStatus LoadItemGroups()
    {
        //get the data
        DataSet ds = null;
        CItemGroupData igd = new CItemGroupData(BaseMstr.BaseData);
        CStatus status = igd.GetItemGroupDS(k_ACTIVE_ID.All, out ds);
        if (!status.Status)
        {
            return status;
        }

        ItemGroups = ds.Tables[0];
        if (ItemGroups.Rows.Count == 0)
        {
            gvItemGroups.Width = 468;
        }
        gvItemGroups.DataSource = ItemGroups;
        gvItemGroups.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// all loading is done at this point, re select the 
    /// gridview rows and enable/disable buttons
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        btnPopupIGEdit.Enabled = (ItemGroupID < 1) ? false : true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ucItemGroupsEdit.BaseMstr = BaseMstr;
        ucItemGroupsEdit.MPE = mpeIGEdit;
        ucItemGroupsEdit.GView = gvItemGroups;

        RegisterJavaScript();

        Page.LoadComplete += new EventHandler(Page_LoadComplete);
    }

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        CStatus status = LoadItemGroups();
        if (!status.Status)
        {
            return status;
        }

        return status;
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// create a new item group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickNew(object sender, EventArgs e)
    {
        CStatus status = ucItemGroupsEdit.LoadControl(k_EDIT_MODE.INSERT);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucItemGroupsEdit.ShowMPE();
    }


    /// <summary>
    /// edit an item group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickEdit(object sender, EventArgs e)
    {
        if (ItemGroupID < 1)
        {
            return;
        }

        //pass the basemaster and popupextender to the contol 
        //and load it in "new" mode
        ucItemGroupsEdit.LongID = ItemGroupID;
        CStatus status = ucItemGroupsEdit.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucItemGroupsEdit.ShowMPE();
    }

    /// <summary>
    /// a new item group was inserted
    /// </summary>
    /// <param name="args"></param>
    protected void OnSaveItemGroup(object sender, CAppUserControlArgs e)
    {
        //reload the temporal states
        CStatus status = LoadItemGroups();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        //select the item we just updated
        ItemGroupID = Convert.ToInt64(e.EventData);
        CGridView.SetSelectedRow(gvItemGroups, e.EventData);
        CGridView.SetSelectedLinkButtonForeColor(gvItemGroups, "lnkSelect", Color.White);
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvItemGroups.DataSource = ItemGroups;
        gvItemGroups.DataBind();

        CGridView.SetSelectedRow(gvItemGroups, ItemGroupID);
        CGridView.SetSelectedLinkButtonForeColor(gvItemGroups, "lnkSelect", Color.White);
    }

    /// <summary>
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedItemGroup(object sender, EventArgs e)
    {
        ShowMPE();

        foreach (GridViewRow gvr in gvItemGroups.Rows)
        {
            LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
            if (lnkSelect == null)
            {
                return;
            }

            lnkSelect.ForeColor = Color.Blue;
        }

        DataKey dk = gvItemGroups.SelectedDataKey;
        if (dk != null)
        {
            ItemGroupID = Convert.ToInt64(dk.Value);
            CGridView.SetSelectedLinkButtonForeColor(gvItemGroups, "lnkSelect", Color.White);
        }
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundItemGroup(Object sender, GridViewRowEventArgs e)
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

        lnkSelect.Text = dr["item_group_label"].ToString();
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
    protected void OnSortingItemGroup(object sender, GridViewSortEventArgs e)
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

        DataView dv = ItemGroups.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        ItemGroups = dv.ToTable();

        RebindAndSelect();
    }
}
