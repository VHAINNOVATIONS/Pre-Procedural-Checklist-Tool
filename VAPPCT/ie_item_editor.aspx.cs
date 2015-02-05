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

/// <summary>
/// code behind for the item editor page
/// </summary>
public partial class ie_item_editor : System.Web.UI.Page
{
    /// <summary>
    /// property
    /// stores the selected item id
    /// </summary>
    protected long ItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ItemID"] = value; }
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
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvItems.DataSource = ucItemLookup.ItemDataTable;
        gvItems.DataBind();

        CGridView.SetSelectedRow(gvItems, ItemID);
        CGridView.SetSelectedLinkButtonForeColor(gvItems, "lnkSelect", Color.White);
    }

    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucItemEditor.BaseMstr = Master;
        ucItemEditor.MPE = mpeItemEditor;

        ucItemLookup.BaseMstr = Master;

        if (!IsPostBack)
        {
            ucItemLookup.Activefilter = k_ACTIVE_ID.All;

            Master.PageTitle = "Item Editor";

            CStatus status = ucItemLookup.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
            }
        }
    }

    protected void OnSelIndexChangedItem(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in gvItems.Rows)
        {
            LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
            if (lnkSelect == null)
            {
                return;
            }

            lnkSelect.ForeColor = Color.Blue;
        }

        DataKey dk = gvItems.SelectedDataKey;
        if (dk != null)
        {
            ItemID = Convert.ToInt64(dk.Value);
            CGridView.SetSelectedLinkButtonForeColor(gvItems, "lnkSelect", Color.White);
            btnEdit.Enabled = true;
        }
        else
        {
            ItemID = -1;
            btnEdit.Enabled = false;
        }
    }

    protected void OnRowDataBoundItem(Object sender, GridViewRowEventArgs e)
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
        Label lblType = (Label)gvr.FindControl("lblType");
        Label lblGroup = (Label)gvr.FindControl("lblGroup");
        Label lblDescription = (Label)gvr.FindControl("lblDescription");
        Label lblLookbackTime = (Label)gvr.FindControl("lblLookbackTime");
        Label lblActive = (Label)gvr.FindControl("lblActive");

        if (lnkSelect == null
            || lblType == null
            || lblGroup == null
            || lblDescription == null
            || lblLookbackTime == null
            || lblActive == null)
        {
            return;
        }
        
        lnkSelect.Text = dr["ITEM_LABEL"].ToString();
        lblType.Text = dr["ITEM_TYPE_LABEL"].ToString();
        lblGroup.Text = dr["ITEM_GROUP_LABEL"].ToString();
        lblDescription.Text = dr["ITEM_DESCRIPTION"].ToString();
        lblLookbackTime.Text = dr["LOOKBACK_TIME"].ToString();
        lblActive.Text = dr["ACTIVE_LABEL"].ToString();
    }

    /// <summary>
    /// event
    /// displays the add dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickAdd(object sender, EventArgs e)
    {
        CStatus status = ucItemEditor.LoadControl(k_EDIT_MODE.INSERT);
        if(!status.Status)
        {
            Master.ShowStatusInfo(status);
        }
        ucItemEditor.ShowMPE();
    }

    /// <summary>
    /// event
    /// displays the edit dialog for the selected item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickEdit(object sender, EventArgs e)
    {
        if (ItemID < 1)
        {
            return;
        }

        ucItemEditor.LongID = ItemID;
        CStatus status = ucItemEditor.LoadControl(k_EDIT_MODE.UPDATE);
        if(!status.Status)
        {
            Master.ShowStatusInfo(status);
        }
        ucItemEditor.ShowMPE();
    }

    /// <summary>
    /// event
    /// loads the gridview with the search results
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSearchItems(object sender, EventArgs e)
    {
        gvItems.PageIndex = 0;
        gvItems.SelectedIndex = -1;
        gvItems.EmptyDataText = "No result(s) found.";
        gvItems.DataSource = ucItemLookup.ItemDataTable;
        gvItems.DataBind();
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
    protected void OnSortingItem(object sender, GridViewSortEventArgs e)
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

        DataView dv = ucItemLookup.ItemDataTable.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        ucItemLookup.ItemDataTable = dv.ToTable();

        RebindAndSelect();
    }

    protected void OnFiltersCollapse(object sender, CFiltersCollapseEventArgs e)
    {
        pnlGridView.Height = (e.Collapsed) ? 484 : 302;
    }
}
