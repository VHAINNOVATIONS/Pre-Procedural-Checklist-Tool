using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Drawing;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ce_ucItemSelector : CAppUserControlPopup
{
    private event EventHandler<CAppUserControlArgs> _Select;
    /// <summary>
    /// event
    /// fires when an item is selected
    /// </summary>
    public event EventHandler<CAppUserControlArgs> Select
    {
        add { _Select += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Select -= value; }
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
    /// property
    /// stores the selected item id
    /// </summary>
    private long ItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ItemID"] = value; }
    }

    /// <summary>
    /// event
    /// sets the title for the dialog
    /// initializes the dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucItemLookup.BaseMstr = BaseMstr;
        ucItemLookup.MPE = MPE;

        if (!IsPostBack)
        {
            ucItemLookup.Activefilter = k_ACTIVE_ID.Active;

            Title = "Item Selector";

            gvItems.DataSource = null;
            gvItems.DataBind();
        }
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
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedItem(object sender, EventArgs e)
    {
        ShowMPE();

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
        }
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
    /// override
    /// loads the item lookup user control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;
        CStatus status = ucItemLookup.LoadControl(EditMode);
        if (!status.Status)
        {
            return status;
        }

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
    /// raises an event with the selected item id
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSelect(object sender, EventArgs e)
    {
        if (ItemID < 1)
        {
            return;
        }

        if (_Select != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                ItemID.ToString());

            _Select(this, args);
        }
    }

    /// <summary>
    /// event
    /// raises an event stating nothing was selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {
        ShowParentMPE();
    }

    /// <summary>
    /// event
    /// loads the gridview with the search results
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSearchItems(object sender, EventArgs e)
    {
        ShowMPE();
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

        DataView dv = ucItemLookup.ItemDataTable.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        ucItemLookup.ItemDataTable = dv.ToTable();

        RebindAndSelect();
    }

    protected void OnFiltersCollapse(object sender, CFiltersCollapseEventArgs e)
    {
        pnlItemSelector.Height = (e.Collapsed) ? 432 : 250;
    }
}
