using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ie_ucItemComponentEditor : CAppUserControl
{
    private const int kItemComponentIDIndex = 0;
    private const int kICRangeIDIndex = 1;
    private const int kICStateIDIndex = 2;

    public Unit Height
    {
        get { return pnlGridView.Height; }
        set { pnlGridView.Height = value; }
    }

    /// <summary>
    /// property
    /// enables/disables the add button
    /// </summary>
    public bool AddEnabled
    {
        get { return btnAdd.Enabled; }
        set { btnAdd.Enabled = value; }
    }

    /// <summary>
    /// property
    /// returns the number of item components
    /// </summary>
    public long Count
    {
        get { return gvItemComponent.Rows.Count; }
    }

    /// <summary>
    /// item id property
    /// </summary>
    public long ItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ItemID"] = value; }
    }

    /// <summary>
    /// item type property
    /// </summary>
    public long ItemTypeID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemTypeID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ItemTypeID"] = value; }
    }

    /// <summary>
    /// property
    /// cache the complete dataset for paging
    /// </summary>
    public DataTable ItemComponents
    {
        get
        {
            object obj = ViewState[ClientID + "ItemComponents"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "ItemComponents"] = value; }
    }

    public void SourceAndBind()
    {
        gvItemComponent.DataSource = ItemComponents;
        gvItemComponent.DataBind();
    }

    public void Clear()
    {
        if(ItemComponents != null)
        {
            ItemComponents.Rows.Clear();
        }
        SourceAndBind();
    }

    /// <summary>
    /// method
    /// initializes the collection item data table with the columns required by the grid view
    /// </summary>
    protected void InitializeDataTable()
    {
        DataTable dt = new DataTable();

        // item component
        dt.Columns.Add("ITEM_ID", Type.GetType("System.Int32"));
        dt.Columns.Add("ITEM_COMPONENT_ID", Type.GetType("System.Int32"));
        dt.Columns.Add("ITEM_COMPONENT_LABEL", Type.GetType("System.String"));
        dt.Columns.Add("SORT_ORDER", Type.GetType("System.Int32"));
        dt.Columns.Add("ACTIVE_ID", Type.GetType("System.Int32"));

        // item component state
        dt.Columns.Add("IC_STATE_ID", Type.GetType("System.Int32"));
        dt.Columns.Add("STATE_ID", Type.GetType("System.Int32"));

        // item component range
        dt.Columns.Add("IC_RANGE_ID", Type.GetType("System.Int32"));
        dt.Columns.Add("UNITS", Type.GetType("System.String"));
        dt.Columns.Add("LEGAL_MIN", Type.GetType("System.Double"));
        dt.Columns.Add("CRITICAL_LOW", Type.GetType("System.Double"));
        dt.Columns.Add("LOW", Type.GetType("System.Double"));
        dt.Columns.Add("HIGH", Type.GetType("System.Double"));
        dt.Columns.Add("CRITICAL_HIGH", Type.GetType("System.Double"));
        dt.Columns.Add("LEGAL_MAX", Type.GetType("System.Double"));

        ItemComponents = dt;
    }

    /// <summary>
    /// load datarow from component control
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="dr"></param>
    protected CStatus LoadDRFromComp(GridViewRow gvr, DataRow dr)
    {
        TextBox tbComponentLabel = (TextBox)gvr.FindControl("tbComponentLabel");
        TextBox tbSortOrder = (TextBox)gvr.FindControl("tbSortOrder");
        CheckBox chkActive = (CheckBox)gvr.FindControl("chkActive");

        if (tbComponentLabel == null
            || tbSortOrder == null
            || chkActive == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        dr["ITEM_COMPONENT_LABEL"] = tbComponentLabel.Text;
        string strSortOrder = (!string.IsNullOrEmpty(tbSortOrder.Text)) ? tbSortOrder.Text : "0";
        dr["SORT_ORDER"] = strSortOrder;
        dr["ACTIVE_ID"] = Convert.ToInt32((chkActive.Checked) ? k_ACTIVE_ID.Active : k_ACTIVE_ID.Inactive);

        return new CStatus();
    }

    /// <summary>
    /// load datarow from range control
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="dr"></param>
    protected CStatus LoadDRFromRange(GridViewRow gvr, DataRow dr)
    {
        TextBox tbUnits = (TextBox)gvr.FindControl("tbUnits");
        TextBox tbLegalMin = (TextBox)gvr.FindControl("tbLegalMin");
        TextBox tbCriticalLow = (TextBox)gvr.FindControl("tbCriticalLow");
        TextBox tbLow = (TextBox)gvr.FindControl("tbLow");
        TextBox tbHigh = (TextBox)gvr.FindControl("tbHigh");
        TextBox tbCriticalHigh = (TextBox)gvr.FindControl("tbCriticalHigh");
        TextBox tbLegalMax = (TextBox)gvr.FindControl("tbLegalMax");

        if (tbUnits == null
            || tbLegalMin == null
            || tbCriticalLow == null
            || tbLow == null
            || tbHigh == null
            || tbCriticalHigh == null
            || tbLegalMax == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        dr["UNITS"] = tbUnits.Text;
        dr["LEGAL_MIN"] = tbLegalMin.Text;
        dr["CRITICAL_LOW"] = tbCriticalLow.Text;
        dr["LOW"] = tbLow.Text;
        dr["HIGH"] = tbHigh.Text;
        dr["CRITICAL_HIGH"] = tbCriticalHigh.Text;
        dr["LEGAL_MAX"] = tbLegalMax.Text;

        return new CStatus();
    }

    /// <summary>
    /// load datarow from selection control
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="dr"></param>
    protected CStatus LoadDRFromSelection(GridViewRow gvr, DataRow dr)
    {
        DropDownList ddlState = (DropDownList)gvr.FindControl("ddlState");

        if (ddlState == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        dr["STATE_ID"] = ddlState.SelectedValue;

        return new CStatus();
    }

    /// <summary>
    /// load dataset from gridview
    /// </summary>
    public void MoveValuesFromGVtoDT()
    {
        foreach (GridViewRow gvr in gvItemComponent.Rows)
        {
            foreach (DataRow dr in ItemComponents.Rows)
            {
                if (gvItemComponent.DataKeys[gvr.RowIndex].Values[kItemComponentIDIndex].ToString() == dr["ITEM_COMPONENT_ID"].ToString())
                {
                    CStatus status = LoadDRFromComp(gvr, dr);
                    if (!status.Status)
                    {
                        return;
                    }

                    switch (ItemTypeID)
                    {
                        case (long)k_ITEM_TYPE_ID.Laboratory:
                            status = LoadDRFromRange(gvr, dr);
                            if (!status.Status)
                            {
                                return;
                            }
                            break;
                        case (long)k_ITEM_TYPE_ID.QuestionSelection:
                            status = LoadDRFromSelection(gvr, dr);
                            if (!status.Status)
                            {
                                return;
                            }
                            break;
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// load gridview dropdown lists
    /// </summary>
    /// <param name="gvr"></param>
    protected CStatus LoadGridViewRowDDLs(GridViewRow gvr)
    {
        DropDownList ddlState = (DropDownList)gvr.FindControl("ddlState");
        if (ddlState == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        return CSTAT.LoadStateDDL(BaseMstr.BaseData, ddlState);
    }

    /// <summary>
    /// US:1945 hide gridview row tables
    /// </summary>
    /// <param name="gvr"></param>
    protected CStatus HideGridViewRowTables(GridViewRow gvr)
    {
        Label lblRange = (Label)gvr.FindControl("lblRange");
        Label lblSelection = (Label)gvr.FindControl("lblSelection");

        if (lblRange == null || lblSelection == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        switch (ItemTypeID)
        {
            case (long)k_ITEM_TYPE_ID.Laboratory:
                lblRange.Visible = true;
                lblSelection.Visible = false;
                break;
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                lblRange.Visible = false;
                lblSelection.Visible = true;
                break;
            case (long)k_ITEM_TYPE_ID.NoteTitle:
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
                lblRange.Visible = false;
                lblSelection.Visible = false;
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        return new CStatus();
    }

    /// <summary>
    /// laod gridview comp values
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="dr"></param>
    protected CStatus LoadGridViewCompValues(GridViewRow gvr, DataRow dr)
    {
        TextBox tbComponentLabel = (TextBox)gvr.FindControl("tbComponentLabel");
        TextBox tbSortOrder = (TextBox)gvr.FindControl("tbSortOrder");
        CheckBox chkActive = (CheckBox)gvr.FindControl("chkActive");

        if (tbComponentLabel == null
            || tbSortOrder == null
            || chkActive == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        tbComponentLabel.Text = dr["ITEM_COMPONENT_LABEL"].ToString();
        tbSortOrder.Text = dr["SORT_ORDER"].ToString();
        chkActive.Checked = (Convert.ToInt64(dr["ACTIVE_ID"]) == (long)k_ACTIVE_ID.Active) ? true : false;

        return new CStatus();
    }

    /// <summary>
    /// load gridview range values
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="dr"></param>
    protected CStatus LoadGridViewRangeValues(GridViewRow gvr, DataRow dr)
    {
        TextBox tbUnits = (TextBox)gvr.FindControl("tbUnits");
        TextBox tbLegalMin = (TextBox)gvr.FindControl("tbLegalMin");
        TextBox tbCriticalLow = (TextBox)gvr.FindControl("tbCriticalLow");
        TextBox tbLow = (TextBox)gvr.FindControl("tbLow");
        TextBox tbHigh = (TextBox)gvr.FindControl("tbHigh");
        TextBox tbCriticalHigh = (TextBox)gvr.FindControl("tbCriticalHigh");
        TextBox tbLegalMax = (TextBox)gvr.FindControl("tbLegalMax");

        if (tbUnits == null
            || tbLegalMin == null
            || tbCriticalLow == null
            || tbLow == null
            || tbHigh == null
            || tbCriticalHigh == null
            || tbLegalMax == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        tbUnits.Text = dr["UNITS"].ToString();
        tbLegalMin.Text = dr["LEGAL_MIN"].ToString();
        tbCriticalLow.Text = dr["CRITICAL_LOW"].ToString();
        tbLow.Text = dr["LOW"].ToString();
        tbHigh.Text = dr["HIGH"].ToString();
        tbCriticalHigh.Text = dr["CRITICAL_HIGH"].ToString();
        tbLegalMax.Text = dr["LEGAL_MAX"].ToString();

        return new CStatus();
    }

    /// <summary>
    /// load gridview selection values
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="dr"></param>
    protected CStatus LoadGridViewSelectionValues(GridViewRow gvr, DataRow dr)
    {
        DropDownList ddlState = (DropDownList)gvr.FindControl("ddlState");

        if (ddlState == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        ddlState.SelectedValue = dr["STATE_ID"].ToString();

        return new CStatus();
    }

    /// <summary>
    /// US:1945 load gridview row values
    /// </summary>
    /// <param name="gvr"></param>
    protected CStatus LoadGridViewRowValues(GridViewRow gvr)
    {
        DataRowView drv = (DataRowView)gvr.DataItem;
        if (drv == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        DataRow dr = drv.Row;
        if(dr == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        CStatus status = LoadGridViewCompValues(gvr, dr);
        if (!status.Status)
        {
            return status;
        }

        switch (ItemTypeID)
        {
            case (long)k_ITEM_TYPE_ID.Laboratory:
                status = LoadGridViewRangeValues(gvr, dr);
                if (!status.Status)
                {
                    return status;
                }
                break;
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                LoadGridViewSelectionValues(gvr, dr);
                if (!status.Status)
                {
                    return status;
                }
                break;
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
            case (long)k_ITEM_TYPE_ID.NoteTitle:
                // nothing to load
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        return new CStatus();
    }

    /// <summary>
    /// handle row databind
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundComp(object sender, GridViewRowEventArgs e)
    {
        GridViewRow gvr = (GridViewRow)e.Row;
        if (gvr == null)
        {
            ShowStatusInfo(new CStatus(false, k_STATUS_CODE.Failed, "TODO"));
            return;
        }

        if (gvr.RowType == DataControlRowType.DataRow)
        {
            CStatus status = LoadGridViewRowDDLs(gvr);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }

            status = HideGridViewRowTables(gvr);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }

            status = LoadGridViewRowValues(gvr);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }
        }
    }

    /// <summary>
    /// method
    /// adds a blank component
    /// </summary>
    public CStatus AddComponent()
    {
        CItemComponentDataItem di = new CItemComponentDataItem();
        di.ItemComponentLabel = string.Empty;
        di.SortOrder = gvItemComponent.Rows.Count + 1;
        di.ActiveID = k_ACTIVE_ID.Active;
        return AddComponent(di);
    }

    /// <summary>
    /// method
    /// adds a component with the specified values
    /// </summary>
    /// <param name="di"></param>
    public CStatus AddComponent(CItemComponentDataItem di)
    {
        if (di == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        DataRow row = ItemComponents.NewRow();

        row["ITEM_ID"] = 0;
        row["ITEM_COMPONENT_ID"] = -(gvItemComponent.Rows.Count + 1);
        row["ITEM_COMPONENT_LABEL"] = di.ItemComponentLabel;
        row["SORT_ORDER"] = di.SortOrder;
        row["ACTIVE_ID"] = di.ActiveID;

        row["IC_STATE_ID"] = 0;
        row["STATE_ID"] = 0;

        row["IC_RANGE_ID"] = 0;
        row["UNITS"] = string.Empty;
        row["LEGAL_MIN"] = 0;
        row["CRITICAL_LOW"] = 0;
        row["LOW"] = 0;
        row["HIGH"] = 0;
        row["CRITICAL_HIGH"] = 0;
        row["LEGAL_MAX"] = 0;

        ItemComponents.Rows.Add(row);

        MoveValuesFromGVtoDT();

        SourceAndBind();

        return new CStatus();
    }

    /// <summary>
    /// method
    /// saves a component with the values of the data row passed in
    /// </summary>
    /// <param name="dr"></param>
    /// <param name="lItemComponentID"></param>
    /// <returns></returns>
    protected CStatus SaveComponent(DataRow dr, out long lItemComponentID)
    {
        lItemComponentID = 0;

        CItemComponentDataItem icData = new CItemComponentDataItem();
        icData.ItemID = ItemID;
        icData.ItemComponentID = Convert.ToInt64(dr["ITEM_COMPONENT_ID"]);
        icData.ItemComponentLabel = dr["ITEM_COMPONENT_LABEL"].ToString();
        icData.SortOrder = Convert.ToInt64(dr["SORT_ORDER"]);
        icData.ActiveID = (k_ACTIVE_ID)Convert.ToInt64(dr["ACTIVE_ID"]);

        CItemComponentData icd = new CItemComponentData(BaseMstr.BaseData);
        CStatus status = new CStatus();
        if (icData.ItemComponentID < 1)
        {
            status = icd.InsertItemComponent(icData, out lItemComponentID);
            if (!status.Status)
            {
                return status;
            }

            dr["ITEM_COMPONENT_ID"] = lItemComponentID;
        }
        else
        {
            status = icd.UpdateItemComponent(icData);
            if (!status.Status)
            {
                return status;
            }

            lItemComponentID = icData.ItemComponentID;
        }

        return status;
    }

    /// <summary>
    /// method
    /// saves a range with the values from the data row passed in
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="lItemComponentID"></param>
    /// <returns></returns>
    protected CStatus SaveRange(DataRow dr, long lItemComponentID)
    {
        CICRangeDataItem icrData = new CICRangeDataItem();
        icrData.ItemID = ItemID;
        icrData.ItemComponentID = lItemComponentID;
        icrData.ICRangeID = Convert.ToInt64(dr["IC_RANGE_ID"]);
        icrData.Units = dr["UNITS"].ToString();
        icrData.LegalMin = Convert.ToDouble(dr["LEGAL_MIN"]);
        icrData.CriticalLow = Convert.ToDouble(dr["CRITICAL_LOW"]);
        icrData.Low = Convert.ToDouble(dr["LOW"]);
        icrData.High = Convert.ToDouble(dr["HIGH"]);
        icrData.CriticalHigh = Convert.ToDouble(dr["CRITICAL_HIGH"]);
        icrData.LegalMax = Convert.ToDouble(dr["LEGAL_MAX"]);

        CItemComponentData icd = new CItemComponentData(BaseMstr.BaseData);
        CStatus status = null;
        if (icrData.ICRangeID < 1)
        {
            long lICRangeID = 0;
            status = icd.InsertICRange(icrData, out lICRangeID);
            if (!status.Status)
            {
                return status;
            }

            dr["IC_RANGE_ID"] = lICRangeID;
        }
        else
        {
            status = icd.UpdateICRange(icrData);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// saves a selection with the values from the data row passed in
    /// </summary>
    /// <param name="dr"></param>
    /// <param name="lItemComponentID"></param>
    /// <returns></returns>
    protected CStatus SaveSelection(DataRow dr, long lItemComponentID)
    {
        CICStateDataItem icsData = new CICStateDataItem();
        icsData.ItemID = ItemID;
        icsData.ItemComponentID = lItemComponentID;
        icsData.ICStateID = Convert.ToInt64(dr["IC_STATE_ID"]);
        icsData.StateID = Convert.ToInt64(dr["STATE_ID"]);

        CItemComponentData icd = new CItemComponentData(BaseMstr.BaseData);
        CStatus status = null;
        if (icsData.ICStateID < 1)
        {
            long lICStateID = 0;
            status = icd.InsertICState(icsData, out lICStateID);
            if (!status.Status)
            {
                return status;
            }

            dr["IC_STATE_ID"] = lICStateID;
        }
        else
        {
            status = icd.UpdateICState(icsData);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// deletes the item components belonging to the item that are not in data table
    /// </summary>
    /// <returns></returns>
    protected CStatus DeleteItemComponents()
    {
        string strItemComponents = ",";
        foreach (DataRow dr in ItemComponents.Rows)
        {
            strItemComponents += dr["ITEM_COMPONENT_ID"].ToString() + ",";
        }

        CItemData item = new CItemData(BaseMstr.BaseData);
        return item.DeleteChildren(ItemID, ItemTypeID, strItemComponents, string.Empty);
    }

    /// <summary>
    /// saves the item components defined in the item component table
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = DeleteItemComponents();
        if (!status.Status)
        {
            return status;
        }

        foreach (DataRow dr in ItemComponents.Rows)
        {
            long lItemComponentID = 0;
            status = SaveComponent(dr, out lItemComponentID);
            if (!status.Status)
            {
                return status;
            }

            switch (ItemTypeID)
            {
                case (long)k_ITEM_TYPE_ID.Laboratory:
                    status = SaveRange(dr, lItemComponentID);
                    if(!status.Status)
                    {
                        return status;
                    }
                    break;
                case (long)k_ITEM_TYPE_ID.QuestionSelection:
                    status = SaveSelection(dr, lItemComponentID);
                    if (!status.Status)
                    {
                        return status;
                    }
                    break;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// override
    /// loads the control with the components of the item specified by
    /// the propery ItemID if the edit mode specified is update
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        if (EditMode == k_EDIT_MODE.UPDATE)
        {
            CItemComponentData icd = new CItemComponentData(BaseMstr.BaseData);
            DataSet dsItemComponents = null;
            CStatus status = icd.GetItemComponentOJDS(ItemID, k_ACTIVE_ID.All, out dsItemComponents);
            if (!status.Status)
            {
                return status;
            }

            ItemComponents = dsItemComponents.Tables[0];
            SourceAndBind();
        }
        else
        {
            ItemID = 0;
            InitializeDataTable();
            SourceAndBind();
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// validates the component columns for the data row passed in
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="plistErrors"></param>
    /// <returns></returns>
    protected bool ValidateComponent(DataRow dr, out CParameterList plistErrors)
    {
        plistErrors = new CParameterList();

        bool bStatus = true;

        if (string.IsNullOrEmpty(dr["ITEM_COMPONENT_LABEL"].ToString()))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_LABEL", Resources.ErrorMessages.ERROR_IE_ICE_LABEL);
            bStatus = false;
        }

        string strSortOrder = dr["SORT_ORDER"].ToString();
        if (string.IsNullOrEmpty(strSortOrder) || Convert.ToInt64(strSortOrder) < 1)
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_SORTORDER", Resources.ErrorMessages.ERROR_IE_ICE_SORTORDER);
            bStatus = false;
        }

        return bStatus;
    }

    /// <summary>
    /// method
    /// validates the range columns for the data row passed in
    /// </summary>
    /// <param name="dr"></param>
    /// <param name="plistErrors"></param>
    /// <returns></returns>
    protected bool ValidateRange(DataRow dr, out CParameterList plistErrors)
    {
        plistErrors = new CParameterList();
        bool bStatus = true;

        if (String.IsNullOrEmpty(dr["UNITS"].ToString()))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_UNITS", Resources.ErrorMessages.ERROR_IE_ICE_UNITS);
            bStatus = false;
        }

        double dResult = 0;
        if (!double.TryParse(dr["LEGAL_MIN"].ToString(), out dResult))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_MIN", Resources.ErrorMessages.ERROR_IE_ICE_MIN);
            bStatus = false;
        }

        if (!double.TryParse(dr["CRITICAL_LOW"].ToString(), out dResult))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_CRITLOW", Resources.ErrorMessages.ERROR_IE_ICE_CRITLOW);
            bStatus = false;
        }

        if (!double.TryParse(dr["LOW"].ToString(), out dResult))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_LOW", Resources.ErrorMessages.ERROR_IE_ICE_LOW);
            bStatus = false;
        }

        if (!double.TryParse(dr["HIGH"].ToString(), out dResult))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_HIGH", Resources.ErrorMessages.ERROR_IE_ICE_HIGH);
            bStatus = false;
        }

        if (!double.TryParse(dr["CRITICAL_HIGH"].ToString(), out dResult))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_CRITHIGH", Resources.ErrorMessages.ERROR_IE_ICE_CRITHIGH);
            bStatus = false;
        }

        if (!double.TryParse(dr["LEGAL_MAX"].ToString(), out dResult))
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_MAX", Resources.ErrorMessages.ERROR_IE_ICE_MAX);
            bStatus = false;
        }

        return bStatus;
    }

    /// <summary>
    /// method
    /// validates the selection columns for the data row passed in
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="plistErrors"></param>
    /// <returns></returns>
    protected bool ValidateSelection(DataRow dr, out CParameterList plistErrors)
    {
        plistErrors = new CParameterList();
        bool bStatus = true;

        string strStateID = dr["STATE_ID"].ToString();
        if (string.IsNullOrEmpty(strStateID) || Convert.ToInt64(strStateID) < 1)
        {
            plistErrors.AddInputParameter("ERROR_IE_ICE_STATE", Resources.ErrorMessages.ERROR_IE_ICE_STATE);
            bStatus = false;
        }

        return bStatus;
    }

    /// <summary>
    /// override
    /// validates user inputs
    /// </summary>
    /// <param name="plistErrors"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistErrors)
    {
        MoveValuesFromGVtoDT();

        plistErrors = new CParameterList();

        foreach (DataRow dr in ItemComponents.Rows)
        {
            if (!ValidateComponent(dr, out plistErrors))
            {
                return new CStatus(false, k_STATUS_CODE.Failed, string.Empty);
            }

            switch (ItemTypeID)
            {
                case (long)k_ITEM_TYPE_ID.Laboratory:
                    if (!ValidateRange(dr, out plistErrors))
                    {
                        return new CStatus(false, k_STATUS_CODE.Failed, string.Empty);
                    }
                    break;
                case (long)k_ITEM_TYPE_ID.QuestionSelection:
                    if (!ValidateSelection(dr, out plistErrors))
                    {
                        return new CStatus(false, k_STATUS_CODE.Failed, string.Empty);
                    }
                    break;
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
        AddComponent();
        btnAdd.Focus();
    }
}
