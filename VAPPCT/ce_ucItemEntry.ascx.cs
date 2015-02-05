using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ce_ucItemEntry : CAppUserControl
{
    public long ChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistID"] = value; }
    }

    protected DataTable Items
    {
        get
        {
            object obj = ViewState[ClientID + "Items"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "Items"] = value; }
    }

    protected DataTable DSChangeable
    {
        get
        {
            object obj = ViewState[ClientID + "DSChangeable"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "DSChangeable"] = value; }
    }

    public Unit Height
    {
        get { return pnlChecklistItems.Height; }
        set { pnlChecklistItems.Height = value; }
    }

    public bool AddEnabled
    {
        get { return btnAddItem.Enabled; }
        set { btnAddItem.Enabled = value; }
    }

    /// <summary>
    /// event
    /// initializes user control popups
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucStateLogicEditor.BaseMstr = BaseMstr;
        ucStateLogicEditor.MPE = mpeStateSelect;

        ucItemSelector.BaseMstr = BaseMstr;
        ucItemSelector.MPE = mpeAddItem;
    }

    /// <summary>
    /// method
    /// loads the drop down list controls for a grid view row
    /// </summary>
    /// <param name="gvr"></param>
    protected void LoadGridViewRowDDLs(GridViewRow gvr)
    {
        DropDownList ddlUnit = (DropDownList)gvr.FindControl("ddlUnit");
        if (ddlUnit == null)
        {
            return;
        }

        CStatus status = CSTAT.LoadUnitDDL(BaseMstr.BaseData, ddlUnit);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
    }

    /// <summary>
    /// method
    /// loads the check box lists for a grid view row
    /// </summary>
    /// <param name="gvr"></param>
    protected void LoadGridViewRowCBLs(GridViewRow gvr)
    {
        CheckBoxList cblDSChangeable = (CheckBoxList)gvr.FindControl("cblDSChangeable");
        if (cblDSChangeable == null)
        {
            return;
        }

        CSTATData STAT = new CSTATData(BaseMstr.BaseData);
        DataSet dsUserRoles = null;
        CStatus status = STAT.GetUserRolesDS(out dsUserRoles);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        cblDSChangeable.DataSource = dsUserRoles.Tables[0];
        cblDSChangeable.DataBind();
    }

    /// <summary>
    /// method
    /// loads the values from the data row into the grid view row controls
    /// </summary>
    /// <param name="gvr"></param>
    protected void LoadGridViewRowValues(GridViewRow gvr)
    {
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

        Button btnStates = (Button)gvr.FindControl("btnStates");
        CheckBox chkItemActive = (CheckBox)gvr.FindControl("chkItemActive");
        TextBox txtTSTimePeriod = (TextBox)gvr.FindControl("txtTSTimePeriod");
        DropDownList ddlUnit = (DropDownList)gvr.FindControl("ddlUnit");
        TextBox txtSortOrder = (TextBox)gvr.FindControl("txtSortOrder");
        Label lblItemLabel = (Label)gvr.FindControl("lblItemLabel");
        CheckBoxList cblDSChangeable = (CheckBoxList)gvr.FindControl("cblDSChangeable");

        if (btnStates == null
            || chkItemActive == null
            || txtTSTimePeriod == null
            || ddlUnit == null
            || txtSortOrder == null
            || lblItemLabel == null
            || cblDSChangeable == null)
        {
            return;
        }

        btnStates.Attributes.Add("ITEM_ID", dr["ITEM_ID"].ToString());
        btnStates.Enabled = (Convert.ToInt32(dr["CHECKLIST_ID"])< 1) ? false : true;

        chkItemActive.Checked = (Convert.ToInt64(dr["ACTIVE_ID"]) == (long)k_ACTIVE_ID.Active) ? true : false;

        txtTSTimePeriod.Text = dr["CLI_TS_TIME_PERIOD"].ToString();

        ddlUnit.SelectedValue = dr["TIME_UNIT_ID"].ToString();

        txtSortOrder.Text = dr["SORT_ORDER"].ToString();

        lblItemLabel.Text = dr["ITEM_LABEL"].ToString();

        DataRow[] draDSSelect = DSChangeable.Select("ITEM_ID = " + dr["ITEM_ID"].ToString());
        CCheckBoxList cbl = new CCheckBoxList();
        cbl.CheckSelected(cblDSChangeable, draDSSelect);
    }

    /// <summary>
    /// event
    /// calls the loads for the grid view row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnRowDataBoundItem(Object sender, GridViewRowEventArgs e)
    {
        GridViewRow gvr = (GridViewRow)e.Row;
        if (gvr == null || gvr.RowType != DataControlRowType.DataRow)
        {
            return;
        }

        LoadGridViewRowDDLs(gvr);
        LoadGridViewRowCBLs(gvr);
        LoadGridViewRowValues(gvr);
    }

    /// <summary>
    /// event
    /// loads the state selector dialog
    /// displays the state selector dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickStates(object sender, EventArgs e)
    {
        Button btnStates = (Button)sender;
        if (btnStates == null)
        {
            return;
        }

        ucStateLogicEditor.ChecklistItemID = Convert.ToInt64(btnStates.Attributes["ITEM_ID"]);
        ucStateLogicEditor.ChecklistID = ChecklistID;

        CStatus status = ucStateLogicEditor.LoadControl(k_EDIT_MODE.UPDATE);
        if(!status.Status)
        {
            ShowStatusInfo(status);
        }

        ucStateLogicEditor.ShowMPE();
    }

    /// <summary>
    /// method
    /// initializes the data tables with the columns required by the grid view
    /// </summary>
    protected void InitializeDataTables()
    {
        // items
        DataTable dtItems = new DataTable();

        dtItems.Columns.Add("CHECKLIST_ID", Type.GetType("System.Int32"));
        dtItems.Columns.Add("ITEM_ID", Type.GetType("System.Int32"));
        dtItems.Columns.Add("CLI_TS_TIME_PERIOD", Type.GetType("System.Int32"));
        dtItems.Columns.Add("TIME_UNIT_ID", Type.GetType("System.Int32"));
        dtItems.Columns.Add("SORT_ORDER", Type.GetType("System.Int32"));
        dtItems.Columns.Add("ACTIVE_ID", Type.GetType("System.Int32"));
        dtItems.Columns.Add("ITEM_LABEL", Type.GetType("System.String"));
        dtItems.Columns.Add("TIME_UNIT_LABEL", Type.GetType("System.String"));
        dtItems.Columns.Add("ACTIVE_LABEL", Type.GetType("System.String"));
        dtItems.Columns.Add("ITEM_LOGIC", Type.GetType("System.String"));

        Items = dtItems;

        // ds changeable
        DataTable dtDSChangeable = new DataTable();

        dtDSChangeable.Columns.Add("CHECKLIST_ID", Type.GetType("System.Int32"));
        dtDSChangeable.Columns.Add("ITEM_ID", Type.GetType("System.Int32"));
        dtDSChangeable.Columns.Add("USER_ROLE_ID", Type.GetType("System.Int32"));

        DSChangeable = dtDSChangeable;
    }

    /// <summary>
    ///check decision state changeable by and make sure its valid given the 
    ///viewable and read only states of the checklist.
    /// </summary>
    /// <param name="cblViewable"></param>
    /// <param name="cblReadOnly"></param>
    /// <returns></returns>
    public CStatus ValidateDSChangeableRoles( CheckBoxList cblViewable,
                                              CheckBoxList cblReadOnly)
    {
        CStatus status = new CStatus();

        //0 = administrator 
        //1 = doctor 
        //2 = nurse 
        bool bViewableAdmin = cblViewable.Items[0].Selected;
        bool bViewableDoctor = cblViewable.Items[1].Selected;
        bool bViewableNurse = cblViewable.Items[2].Selected;

        bool bReadOnlyAdmin = cblReadOnly.Items[0].Selected;
        bool bReadOnlyDoctor = cblReadOnly.Items[1].Selected;
        bool bReadOnlyNurse = cblReadOnly.Items[2].Selected;

        foreach (GridViewRow gvr in gvChecklistItems.Rows)
        {
            CheckBoxList cblDSChangeable = (CheckBoxList)gvr.FindControl("cblDSChangeable");
            if (cblDSChangeable != null)
            {
                if (!bViewableAdmin)
                {
                    if (cblDSChangeable.Items[0].Selected)
                    {
                        status.Status = false;
                        status.StatusCode = k_STATUS_CODE.Failed;
                        status.StatusComment = "Invalid DS Changeable permissions.";
                        return status;
                    }
                }
                if (!bViewableDoctor)
                {
                    if (cblDSChangeable.Items[1].Selected)
                    {
                        status.Status = false;
                        status.StatusCode = k_STATUS_CODE.Failed;
                        status.StatusComment = "Invalid DS Changeable permissions.";
                        return status;
                    }
                }
                if (!bViewableNurse)
                {
                    if (cblDSChangeable.Items[2].Selected)
                    {
                        status.Status = false;
                        status.StatusCode = k_STATUS_CODE.Failed;
                        status.StatusComment = "Invalid DS Changeable permissions.";
                        return status;
                    }      
                }

                if (bReadOnlyAdmin)
                {
                    if (cblDSChangeable.Items[0].Selected)
                    {
                        status.Status = false;
                        status.StatusCode = k_STATUS_CODE.Failed;
                        status.StatusComment = "Invalid DS Changeable permissions.";
                        return status;
                    }
                }
                if (bReadOnlyDoctor)
                {
                    if (cblDSChangeable.Items[1].Selected)
                    {
                        status.Status = false;
                        status.StatusCode = k_STATUS_CODE.Failed;
                        status.StatusComment = "Invalid DS Changeable permissions.";
                        return status;
                    }
                }
                if (bReadOnlyNurse)
                {
                    if (cblDSChangeable.Items[2].Selected)
                    {
                        status.Status = false;
                        status.StatusCode = k_STATUS_CODE.Failed;
                        status.StatusComment = "Invalid DS Changeable permissions.";
                        return status;
                    }
                }
            }
        }

        return status;
    }

    /// <summary>
    /// override
    /// loads the data sets required to load the item list
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        if(EditMode == k_EDIT_MODE.UPDATE)
        {
            CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
            DataSet dsDSChangeable = null;
            CStatus status = cld.GetChecklistDSChangeableDS(ChecklistID, out dsDSChangeable);
            if(!status.Status)
            {
                ShowStatusInfo(status);
                return status;
            }

            DSChangeable = dsDSChangeable.Tables[0];

            CChecklistItemData clid = new CChecklistItemData(BaseMstr.BaseData);
            DataSet dsChecklistItem = null;
            status = clid.GetChecklistItemsDS(ChecklistID, out dsChecklistItem);
            if(!status.Status)
            {
                ShowStatusInfo(status);
                return status;
            }

            Items = dsChecklistItem.Tables[0];
            gvChecklistItems.DataSource = Items;
            gvChecklistItems.DataBind();
        }
        else
        {
            ChecklistID = 0;
            gvChecklistItems.DataSource = null;
            gvChecklistItems.DataBind();
            InitializeDataTables();
        }

        return new CStatus(); 
    }

    /// <summary>
    /// method
    /// adds an empty item to the item list
    /// </summary>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    private CStatus AddItem(long lItemID)
    {
        if (Items.Select("ITEM_ID = " + lItemID.ToString()).Count() > 0)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_CE_ITEMEXISTS);
        }

        CItemData id = new CItemData(BaseMstr.BaseData);
        CItemDataItem ItemData = new CItemDataItem();
        CStatus status = id.GetItemDI(lItemID, out ItemData);
        if (!status.Status)
        {
            return status;
        }

        DataRow dr = Items.NewRow();

        dr["CHECKLIST_ID"] = 0;
        dr["ITEM_ID"] = ItemData.ItemID;
        dr["ACTIVE_ID"] = (long)k_ACTIVE_ID.Active;
        dr["ITEM_LABEL"] = ItemData.ItemLabel;

        Items.Rows.Add(dr);

        gvChecklistItems.DataSource = Items;
        gvChecklistItems.DataBind();

        return new CStatus();
    }

    /// <summary>
    /// method
    /// saves a checklist item to the database
    /// </summary>
    /// <param name="dr"></param>
    /// <returns></returns>
    protected CStatus SaveChecklistItem(DataRow dr)
    {
        CChecklistItemDataItem clidi = new CChecklistItemDataItem();
        clidi.ChecklistID = Convert.ToInt64(dr["CHECKLIST_ID"]);
        clidi.ItemID = Convert.ToInt64(dr["ITEM_ID"]);
        clidi.CLITSTimePeriod = Convert.ToInt64(dr["CLI_TS_TIME_PERIOD"]);
        clidi.TimeUnitID = (k_TIME_UNIT_ID)Convert.ToInt64(dr["TIME_UNIT_ID"]);
        clidi.SortOrder = Convert.ToInt64(dr["SORT_ORDER"]);
        clidi.ActiveID = (k_ACTIVE_ID)Convert.ToInt64(dr["ACTIVE_ID"]);

        CChecklistItemData clid = new CChecklistItemData(BaseMstr.BaseData);
        if (clidi.ChecklistID < 1)
        {
            clidi.ChecklistID = ChecklistID;
            clidi.Logic = CExpression.DefaultTemporalLogic
            + " " + CExpression.DefaultOutcomeLogic
            + " " + CExpression.DefaultDecisionLogic;

            CStatus status = clid.InsertChecklistItem(clidi);
            if (!status.Status)
            {
                return status;
            }

            dr["CHECKLIST_ID"] = ChecklistID;
        }
        else
        {
            CStatus status = clid.UpdateChecklistItem(clidi);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// saves the roles that may edit the decision state for an item to the database
    /// </summary>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    protected CStatus SaveCLIDSEdit(long lItemID)
    {
        CChecklistItemData clid = new CChecklistItemData(BaseMstr.BaseData);
        CStatus status = clid.DeleteAllCLItemDSRoles(ChecklistID, lItemID);
        if (!status.Status)
        {
            return status;
        }

        string strDSSelect = "ITEM_ID = " + lItemID.ToString();
        DataRow[] adr = DSChangeable.Select(strDSSelect);
        foreach (DataRow dr in adr)
        {
            CCLIDSEditDataItem di = new CCLIDSEditDataItem();
            di.ChecklistID = ChecklistID;
            di.ItemID = lItemID;
            di.UserRoleID = Convert.ToInt64(dr["USER_ROLE_ID"]);

            status = clid.InsertCLItemDSRole(di);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// moves the values from the grid view to the data table
    /// </summary>
    protected void MoveValuesFromGVtoDT()
    {
        foreach (GridViewRow gvr in gvChecklistItems.Rows)
        {
            CheckBox chkItemActive = (CheckBox)gvr.FindControl("chkItemActive");
            TextBox txtTSTimePeriod = (TextBox)gvr.FindControl("txtTSTimePeriod");
            DropDownList ddlUnit = (DropDownList)gvr.FindControl("ddlUnit");
            TextBox txtSortOrder = (TextBox)gvr.FindControl("txtSortOrder");
            CheckBoxList cblDSChangeable = (CheckBoxList)gvr.FindControl("cblDSChangeable");

            if (chkItemActive == null
                || txtTSTimePeriod == null
                || ddlUnit == null
                || txtSortOrder == null
                || cblDSChangeable == null)
            {
                return;
            }

            foreach (DataRow dr in Items.Rows)
            {
                string strItemID = dr["ITEM_ID"].ToString();
                if (gvChecklistItems.DataKeys[gvr.RowIndex].Value.ToString() == strItemID)
                {
                    dr["CLI_TS_TIME_PERIOD"] = (string.IsNullOrEmpty(txtTSTimePeriod.Text)) ? "0" : txtTSTimePeriod.Text;
                    dr["TIME_UNIT_ID"] = ddlUnit.SelectedValue;
                    dr["SORT_ORDER"] = (string.IsNullOrEmpty(txtSortOrder.Text)) ? "0" : txtSortOrder.Text;
                    dr["ACTIVE_ID"] = Convert.ToInt32((chkItemActive.Checked) ? k_ACTIVE_ID.Active : k_ACTIVE_ID.Inactive);

                    string strDSSelect = "ITEM_ID = " + strItemID;
                    DataRow[] adrDSChangeable = DSChangeable.Select(strDSSelect);
                    foreach (DataRow drDSChangeable in adrDSChangeable)
                    {
                        drDSChangeable.Delete();
                    }

                    foreach (ListItem li in cblDSChangeable.Items)
                    {
                        if (li.Selected)
                        {
                            DataRow drDSChangeable = DSChangeable.NewRow();

                            drDSChangeable["CHECKLIST_ID"] = ChecklistID;
                            drDSChangeable["ITEM_ID"] = strItemID;
                            drDSChangeable["USER_ROLE_ID"] = li.Value;

                            DSChangeable.Rows.Add(drDSChangeable);
                        }
                    }

                    break;
                }
            }
        }
    }

    /// <summary>
    /// override
    /// calls the save methods for each item in the item list
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        foreach (DataRow dr in Items.Rows)
        {
            CStatus status = SaveChecklistItem(dr);
            if (!status.Status)
            {
                return status;
            }

            status = SaveCLIDSEdit(Convert.ToInt64(dr["ITEM_ID"]));
            if (!status.Status)
            {
                return status;
            }
        }

        foreach (GridViewRow gvr in gvChecklistItems.Rows)
        {
            Button btnStates = (Button)gvr.FindControl("btnStates");
            if (btnStates == null)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            string strSelect = "ITEM_ID = " + btnStates.Attributes["ITEM_ID"];
            DataRow[] dra = Items.Select(strSelect);
            if(dra == null)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            DataRow dr = dra[0];
            if (dr == null)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            btnStates.Enabled = (Convert.ToInt32(dr["CHECKLIST_ID"]) < 1) ? false : true;
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
        string strTSTimePeriod = dr["CLI_TS_TIME_PERIOD"].ToString();
        if (string.IsNullOrEmpty(strTSTimePeriod) || Convert.ToInt32(strTSTimePeriod) < 1)
        {
            plistStatus.AddInputParameter("ERROR_CE_TIMEPERIOD", Resources.ErrorMessages.ERROR_CE_TIMEPERIOD);
        }

        string strUnitID = dr["TIME_UNIT_ID"].ToString();
        if (string.IsNullOrEmpty(strUnitID) || Convert.ToInt32(strUnitID) < 1)
        {
            plistStatus.AddInputParameter("ERROR_CE_TIMEUNITS", Resources.ErrorMessages.ERROR_CE_TIMEUNITS);
        }

        string strSortOrder = dr["SORT_ORDER"].ToString();
        if (string.IsNullOrEmpty(strSortOrder) || Convert.ToInt32(strSortOrder) < 1)
        {
            plistStatus.AddInputParameter("ERROR_CE_SORTORDER", Resources.ErrorMessages.ERROR_CE_SORTORDER);
        }

        string strDSSelect = "ITEM_ID = " + dr["ITEM_ID"].ToString();
        if (DSChangeable.Select(strDSSelect).Count() < 1)
        {
            plistStatus.AddInputParameter("ERROR_CE_DSCHANGE", Resources.ErrorMessages.ERROR_CE_DSCHANGE);
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
    /// calls the validate method for each item in the item list
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        MoveValuesFromGVtoDT();

        plistStatus = null;

        foreach (DataRow dr in Items.Rows)
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
    /// event
    /// loads the item selector dialog
    /// displays the item selector dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickAddItem(object sender, EventArgs e)
    {
        CStatus status = ucItemSelector.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        //show the popup
        ucItemSelector.ShowMPE();
    }

    /// <summary>
    /// method
    /// adds a new item to the item list
    /// </summary>
    /// <param name="args"></param>
    protected void OnSelectItem(object sender, CAppUserControlArgs args)
    {
        if (!string.IsNullOrEmpty(args.EventData))
        {
            CStatus status = AddItem(Convert.ToInt64(args.EventData));
            if (!status.Status)
            {
                ShowStatusInfo(status);
            }
        }
    }
}
