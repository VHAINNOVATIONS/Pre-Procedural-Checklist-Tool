using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.Data;

public partial class ie_ucItemCollectionEditor : CAppUserControl
{
    public Unit Height
    {
        get { return pnlGridView.Height; }
        set { pnlGridView.Height = value; }
    }

    /// <summary>
    /// property
    /// count of the items in the collection
    /// </summary>
    public long Count
    {
        get { return CollectionItems.Rows.Count; }
    }

    /// <summary>
    /// property
    /// cache the complete dataset for paging
    /// </summary>
    public DataTable CollectionItems
    {
        get
        {
            object obj = ViewState[ClientID + "CollectionItems"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "CollectionItems"] = value; }
    }

    /// <summary>
    /// method
    /// initializes the collection item data table with the columns required by the grid view
    /// </summary>
    protected void InitializeDataTable()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("COLLECTION_ITEM_ID", Type.GetType("System.Int32"));
        dt.Columns.Add("ITEM_ID", Type.GetType("System.Int32"));
        dt.Columns.Add("ITEM_LABEL", Type.GetType("System.String"));
        dt.Columns.Add("SORT_ORDER", Type.GetType("System.Int32"));

        CollectionItems = dt;
    }

    /// <summary>
    /// method
    /// moves the values from the grid view to the data table
    /// </summary>
    protected void MoveValuesFromGVtoDT()
    {
        foreach (GridViewRow gvr in gvItemCollection.Rows)
        {
            TextBox tbSortOrder = (TextBox)gvr.FindControl("tbSortOrder");
            if (tbSortOrder == null)
            {
                return;
            }

            foreach (DataRow dr in CollectionItems.Rows)
            {
                string strItemID = dr["ITEM_ID"].ToString();
                if (gvItemCollection.DataKeys[gvr.RowIndex].Value.ToString() == strItemID)
                {
                    dr["SORT_ORDER"] = tbSortOrder.Text;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucItemSelector.BaseMstr = BaseMstr;
        ucItemSelector.ParentMPE = MPE;
        ucItemSelector.MPE = mpeAddItem;
    }

    /// <summary>
    /// US:1883
    /// event
    /// load the value from the ds to the grid view row
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

        Label lblItem = (Label)gvr.FindControl("lblItem");
        TextBox tbSortOrder = (TextBox)gvr.FindControl("tbSortOrder");

        if (lblItem == null || tbSortOrder == null)
        {
            return;
        }

        lblItem.Text = dr["ITEM_LABEL"].ToString();
        tbSortOrder.Text = dr["SORT_ORDER"].ToString();
    }

    /// <summary>
    /// US:1883
    /// method
    /// adds an item to the grid view
    /// </summary>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus AddItem(long lItemID)
    {
        if (CollectionItems.Select("ITEM_ID = " + lItemID.ToString()).Count() > 0)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        CItemData item = new CItemData(BaseMstr.BaseData);
        CItemDataItem di = null;
        CStatus status = item.GetItemDI(lItemID, out di);
        if (!status.Status)
        {
            return status;
        }

        DataRow dr = CollectionItems.NewRow();

        dr["COLLECTION_ITEM_ID"] = 0;
        dr["ITEM_ID"] = lItemID;
        dr["ITEM_LABEL"] = di.ItemLabel;
        dr["SORT_ORDER"] = CollectionItems.Rows.Count + 1;

        CollectionItems.Rows.Add(dr);

        gvItemCollection.DataSource = CollectionItems;
        gvItemCollection.DataBind();

        return new CStatus();
    }

    /// <summary>
    /// US:1883
    /// override
    /// loads the control with the items of the collection specified by the LongID property
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        if (EditMode == k_EDIT_MODE.UPDATE)
        {
            CItemCollectionData itemCollection = new CItemCollectionData(BaseMstr.BaseData);
            DataSet ds = null;
            CStatus status = itemCollection.GetItemCollectionDS(LongID, out ds);
            if (!status.Status)
            {
                return status;
            }

            CollectionItems = ds.Tables[0];
            gvItemCollection.DataSource = CollectionItems;
            gvItemCollection.DataBind();
        }
        else
        {
            LongID = 0;
            gvItemCollection.DataSource = null;
            gvItemCollection.DataBind();
            InitializeDataTable();
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883
    /// method
    /// clears all the values in the collection grid view
    /// </summary>
    public void ClearCollectionGridView()
    {
        gvItemCollection.DataSource = null;
        gvItemCollection.DataBind();
    }

    /// <summary>
    /// US:1883
    /// method
    /// deletes all the items in the collection that are not in the grid view
    /// </summary>
    /// <returns></returns>
    protected CStatus DeleteChildItems()
    {
        string strItemIDs = ",";
        foreach (DataRow dr in CollectionItems.Rows)
        {
            strItemIDs += dr["ITEM_ID"].ToString() + ",";
        }

        CItemData item = new CItemData(BaseMstr.BaseData);
        return item.DeleteChildren(LongID, (long)k_ITEM_TYPE_ID.Collection, string.Empty, strItemIDs);
    }

    /// <summary>
    /// US:1883
    /// override
    /// saves the item collection to the database
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = DeleteChildItems();
        if (!status.Status)
        {
            return status;
        }

        foreach (DataRow dr in CollectionItems.Rows)
        {
            CItemCollectionDataItem di = new CItemCollectionDataItem();
            di.CollectionItemID = Convert.ToInt64(dr["COLLECTION_ITEM_ID"]);
            di.ItemID = Convert.ToInt64(dr["ITEM_ID"]);
            di.SortOrder = Convert.ToInt64(dr["SORT_ORDER"]);

            CItemCollectionData itemCollection = new CItemCollectionData(BaseMstr.BaseData);
            if (di.CollectionItemID < 1)
            {
                di.CollectionItemID = LongID;
                status = itemCollection.InsertItemCollection(di);
                if (!status.Status)
                {
                    return status;
                }

                dr["COLLECTION_ITEM_ID"] = LongID;
            }
            else
            {
                status = itemCollection.UpdateItemCollection(di);
                if (!status.Status)
                {
                    return status;
                }
            }
        }

        

        return new CStatus();
    }

    /// <summary>
    /// method
    /// validates the user input for an item
    /// </summary>
    /// <param name="dr"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    protected CStatus ValidateItem(DataRow dr, out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();

        CStatus status = new CStatus();
        string strSortOrder = dr["SORT_ORDER"].ToString();
        if (string.IsNullOrEmpty(strSortOrder) || Convert.ToInt32(strSortOrder) < 1)
        {
            plistStatus.AddInputParameter("ERROR_IE_COLLECTION", "TODO");
        }

        if (plistStatus.Count > 0)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        return status;
    }

    /// <summary>
    /// override
    /// validates the items in the collection item data table
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        MoveValuesFromGVtoDT();

        plistStatus = null;

        foreach (DataRow dr in CollectionItems.Rows)
        {
            CStatus status = ValidateItem(dr, out plistStatus);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1945 US:1883 user clicked the add button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickAdd(object sender, EventArgs e)
    {
        ShowMPE();
        CStatus status = ucItemSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucItemSelector.ShowMPE();
    }

    /// <summary>
    /// US:1883 event
    /// adds the selected item to the item collection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected void OnSelectItem(object sender, CAppUserControlArgs e)
    {
        ShowMPE();
        if (string.IsNullOrEmpty(e.EventData))
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, "TODO");
            return;
        }

        CStatus status = AddItem(Convert.ToInt64(e.EventData));
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
    }
}
