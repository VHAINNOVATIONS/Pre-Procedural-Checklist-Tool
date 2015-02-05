using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucPatItemEditor : CAppUserControlPopup 
{
    private const int nItemComponentIDIndex = 0;
    private const int nLegalMinIndex = 1;
    private const int nCriticalLowIndex = 2;
    private const int nLowIndex = 3;
    private const int nHighIndex = 4;
    private const int nCriticalHighIndex = 5;
    private const int nLegalMaxIndex = 6;

    /// <summary>
    /// property to get/set the patient id
    /// </summary>
    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientID"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "PatientID"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a checklist id for the page
    /// </summary>
    public long ChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : 0;
        }
        set { ViewState[ClientID + "ChecklistID"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a checklist id for the page
    /// </summary>
    public long PatientChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "PatChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : 0;
        }
        set { ViewState[ClientID + "PatChecklistID"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a checklist item id
    /// </summary>
    public long ItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : 0;
        }
        set { ViewState[ClientID + "ItemID"] = value; }
    }

    private long ItemTypeID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemTypeID"];
            return (obj != null) ? Convert.ToInt64(obj) : 0;
        }
        set { ViewState[ClientID + "ItemTypeID"] = value; }
    }

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        ((app_ucTimer)UCTimer).StopRefresh();

        //clear the collection and items ddl
        ddlColItems.Items.Clear();
        ddlItems.Items.Clear();

        pnlMapped.Visible = false;
        ucNoteTitle.Visible = false;
        pnlComponents.Visible = false;

        CItemData Item = new CItemData(BaseMstr.BaseData);
        CItemDataItem di = null;
        CStatus status = Item.GetItemDI(ItemID, out di);
        if (!status.Status)
        {
            return status;
        }

        ItemTypeID = di.ItemTypeID;

        if (di.ItemTypeID == (long)k_ITEM_TYPE_ID.Collection)
        {
            lblColl.Text = di.ItemLabel;
            lblCollDesc.Text = di.ItemDescription;

            status = CItem.LoadItemCollectionDDL(
                BaseMstr.BaseData,
                ddlColItems,
                ItemID);
            if (!status.Status)
            {
                return status;
            }

            lblItem.Text = string.Empty;
            lblItemDescription.Text = string.Empty;

            pnlCollection.Visible = true;
            pnlComponents.Visible = false;
            ucNoteTitle.Visible = true;
            ucNoteTitle.Clear();

            txtEntryDate.ReadOnly = true;
            calEntryDate.Enabled = false;
            ucTimePicker.Enabled = false;
            txtEntryDate.Text = string.Empty;
            calEntryDate.SelectedDate = null;
            ucTimePicker.SetTime(0, 0, 0);

            txtComment.Enabled = false;
            txtComment.Text = string.Empty;

            gvComponents.DataSource = null;
            gvComponents.DataBind();

            gvComments.DataSource = null;
            gvComments.DataBind();

            //hide the panels and labels for 
            //edit until they 
            //pick an option
            lblDate.Visible = false;
            txtEntryDate.Visible = false;
            ucTimePicker.Visible = false;
            pnlComments.Visible = false;
            pnlComponents.Visible = false;
            lblItemComps.Visible = false;
            lblNewComment.Visible = false;
            lblCommentHistory.Visible = false;
            txtComment.Visible = false;
            ucNoteTitle.Visible = false;
        }
        else
        {
            pnlCollection.Visible = false;

            status = LoadItemAndComponents(di);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    protected CStatus LoadItemAndComponents(CItemDataItem di)
    {
        if (di == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        lblItem.Text = di.ItemLabel;
        lblItemDescription.Text = di.ItemDescription;

        //load a ddl with all results ordered newest to oldest
        CStatus status = CPatientItem.LoadPatientItemsDDL(
            BaseMstr.BaseData,
            ddlItems,
            PatientID,
            ItemID);
        if (!status.Status)
        {
            return status;
        }

        if (di.MapID != "-1")
        {
            ddlItems.Items[0].Text = "[Select Result]";
            txtEntryDate.ReadOnly = true;
            calEntryDate.Enabled = false;
            ucTimePicker.Enabled = false;
            txtEntryDate.Text = string.Empty;
            calEntryDate.SelectedDate = null;
            ucTimePicker.SetTime(0, 0, 0);
            txtComment.Enabled = false;

            ShowEditOptions(false);
        }
        else
        {
            ddlItems.Items[0].Text = "[New Result]";
            txtEntryDate.ReadOnly = false;
            calEntryDate.Enabled = true;
            ucTimePicker.Enabled = true;
            txtEntryDate.Text = CDataUtils.GetDateAsString(DateTime.Now);
            calEntryDate.SelectedDate = DateTime.Now;
            ucTimePicker.SetTime(DateTime.Now);
            txtComment.Enabled = true;

            ShowEditOptions(true);
        }

        if (di.ItemTypeID == (long)k_ITEM_TYPE_ID.NoteTitle)
        {
            pnlComponents.Visible = false;

            ucNoteTitle.Visible = false;
            ucNoteTitle.Clear();
                
            if (ddlItems.Items[0].Text != "[Select Result]")
            {
                if (ddlItems.Items[0].Text != "")
                {
                    ucNoteTitle.Visible = true;
                }
            }
        }
        else
        {
            pnlComponents.Visible = false;
            if (ddlItems.Items[0].Text != "[Select Result]")
            {
                if (ddlItems.Items[0].Text != "")
                {
                    pnlComponents.Visible = true;
                }
            }

            ucNoteTitle.Visible = false;

            status = LoadComponents();
            if (!status.Status)
            {
                return status;
            }
        }

        txtComment.Text = string.Empty;
        gvComments.DataSource = null;
        gvComments.DataBind();

        return new CStatus();
    }

    protected CStatus LoadComponents()
    {
        //get the item components
        DataSet dsComponents = null;
        CItemComponentData icd = new CItemComponentData(BaseMstr.BaseData);
        CStatus status = icd.GetItemComponentOJDS(
            ItemID,
            k_ACTIVE_ID.Active,
            out dsComponents);
        if (!status.Status)
        {
            return status;
        }

        gvComponents.DataSource = dsComponents;
        gvComponents.DataBind();

        return new CStatus();
    }

    protected CStatus LoadPatItemAndComponents()
    {
        CPatientItemData dta = new CPatientItemData(BaseMstr.BaseData);
        CPatientItemDataItem di = null;
        CStatus status = dta.GetPatientItemDI(
            PatientID,
            ItemID,
            Convert.ToInt64(ddlItems.SelectedValue),
            out di);
        if (!status.Status)
        {
            return status;
        }

        // set date/time
        txtEntryDate.Text = CDataUtils.GetDateAsString(di.EntryDate);
        calEntryDate.SelectedDate = di.EntryDate;
        ucTimePicker.SetTime(di.EntryDate);

        if (di.ItemTypeID == (long)k_ITEM_TYPE_ID.NoteTitle)
        {
            ucNoteTitle.ItemID = di.ItemID;
            ucNoteTitle.PatientItemID = Convert.ToInt64(ddlItems.SelectedValue);
            ucNoteTitle.PatientID = PatientID;
            status = ucNoteTitle.LoadControl(k_EDIT_MODE.UPDATE);
            if (!status.Status)
            {
                return status;
            }
        }
        else
        {
            status = LoadPatComponents();
            if (!status.Status)
            {
                return status;
            }
        }

        status = LoadPatComments();
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    protected CStatus LoadPatComponents()
    {
        CPatientItemData pid = new CPatientItemData(BaseMstr.BaseData);
        DataSet dsComps = null;
        CStatus status = pid.GetPatientItemComponentDS(
            PatientID,
            Convert.ToInt64(ddlItems.SelectedValue),
            ItemID,
            out dsComps);
        if (!status.Status)
        {
            return status;
        }

        DataTable dtComps = dsComps.Tables[0];
        if (dtComps == null || dtComps.Rows.Count != gvComponents.Rows.Count)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        foreach (GridViewRow gvr in gvComponents.Rows)
        {
            RadioButton rbSelect = (RadioButton)gvr.FindControl("rbSelComponent");
            TextBox txtVal = (TextBox)gvr.FindControl("txtValue");
            if (rbSelect == null || txtVal == null)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            DataRow drComp = dtComps.Select("ITEM_COMPONENT_ID = " + gvComponents.DataKeys[gvr.DataItemIndex][nItemComponentIDIndex].ToString())[0];
            switch ((k_ITEM_TYPE_ID)ItemTypeID)
            {
                case k_ITEM_TYPE_ID.Laboratory:
                    txtVal.Text = drComp["COMPONENT_VALUE"].ToString();
                    break;
                case k_ITEM_TYPE_ID.QuestionFreeText:
                    txtVal.Text = drComp["COMPONENT_VALUE"].ToString();
                    break;
                case k_ITEM_TYPE_ID.QuestionSelection:
                    rbSelect.Checked = (drComp["COMPONENT_VALUE"].ToString() == "1") ? true : false;
                    break;
                default:
                    return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }
        }

        return new CStatus();
    }

    protected CStatus LoadPatComments()
    {
        CPatientItemData itemData = new CPatientItemData(BaseMstr.BaseData);
        DataSet dsComments = null;
        CStatus status = itemData.GetPatientItemCommmentDS(
            Convert.ToInt64(ddlItems.SelectedValue),
            ItemID,
            out dsComments);
        if (!status.Status)
        {
            return status;
        }

        gvComments.DataSource = dsComments;
        gvComments.DataBind();

        return new CStatus();
    }

    /// <summary>
    /// event
    /// loads grid view row data for non header rows
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
            CStatus status = LoadGridViewRow(gvr);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }
        }
    }

    /// <summary>
    /// method
    /// loads all of the item's component data into the gridview
    /// </summary>
    /// <param name="gvr"></param>
    /// <returns></returns>
    protected CStatus LoadGridViewRow(GridViewRow gvr)
    {
        DataRowView drv = (DataRowView)gvr.DataItem;
        if (drv == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        DataRow dr = drv.Row;
        if (dr == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        RadioButton rbSelect = (RadioButton)gvr.FindControl("rbSelComponent");
        Label lblComponent = (Label)gvr.FindControl("lblComponent");
        TextBox txtVal = (TextBox)gvr.FindControl("txtValue");
        Label lblUnit = (Label)gvr.FindControl("lblUnits");
        Label lblRanges = (Label)gvr.FindControl("lblRanges");

        if (rbSelect == null
            || lblComponent == null
            || txtVal == null
            || lblUnit == null
            || lblRanges == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        if (ddlItems.Items[0].Text == "[Select Result]")
        {
            rbSelect.Enabled = false;
            txtVal.ReadOnly = true;
        }

        switch ((k_ITEM_TYPE_ID)Convert.ToInt64(dr["ITEM_TYPE_ID"]))
        {
            case k_ITEM_TYPE_ID.Laboratory:
                rbSelect.Visible = false;
                lblComponent.Visible = true;
                txtVal.Visible = true;
                lblUnit.Visible = true;
                lblRanges.Visible = true;

                lblComponent.Text = dr["ITEM_COMPONENT_LABEL"].ToString();
                lblUnit.Text = dr["UNITS"].ToString();
                lblRanges.Text = " (Legal Min: " + dr["LEGAL_MIN"].ToString()
                    + " Critical Low: " + dr["CRITICAL_LOW"].ToString()
                    + " Low: " + dr["LOW"].ToString()
                    + " High: " + dr["HIGH"].ToString()
                    + " Critical High: " + dr["CRITICAL_HIGH"].ToString()
                    + " Legal Max: " + dr["LEGAL_MAX"].ToString() + ") ";
                txtVal.Width = 200;
                break;

            case k_ITEM_TYPE_ID.QuestionFreeText:
                rbSelect.Visible = false;
                lblComponent.Visible = true;
                txtVal.Visible = true;
                lblUnit.Visible = false;
                lblRanges.Visible = false;

                lblComponent.Text = dr["ITEM_COMPONENT_LABEL"].ToString();
                txtVal.Width = 500;
                break;
            case k_ITEM_TYPE_ID.QuestionSelection:
                rbSelect.Visible = true;
                lblComponent.Visible = false;
                txtVal.Visible = false;
                lblUnit.Visible = false;
                lblRanges.Visible = false;

                rbSelect.Text = dr["ITEM_COMPONENT_LABEL"].ToString();
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        return new CStatus();
    }

    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucNoteTitle.BaseMstr = BaseMstr;

        if (!IsPostBack)
        {
            Title = "Single Patient Item Editor";
        }
    }

    /// <summary>
    /// US:1883 method
    /// builds a list of all the item components for an item from the controls on the page
    /// </summary>
    /// <param name="PatItemCompList"></param>
    /// <returns></returns>
    private CStatus BuildPatItemCompList(out CPatientItemCompList PatItemCompList)
    {
        PatItemCompList = new CPatientItemCompList();

        //insert components
        foreach (GridViewRow gr in gvComponents.Rows)
        {
            // create patient item component data item for the grid view row
            CPatientItemComponentDataItem di = new CPatientItemComponentDataItem();
            di.PatientID = PatientID;
            di.ItemID = (ItemTypeID == (long)k_ITEM_TYPE_ID.Collection) ? Convert.ToInt64(ddlColItems.SelectedValue) : ItemID;
            di.ComponentID = Convert.ToInt64(gvComponents.DataKeys[gr.DataItemIndex][nItemComponentIDIndex]);

            RadioButton rbSelect = (RadioButton)gr.FindControl("rbSelComponent");
            TextBox txtVal = (TextBox)gr.FindControl("txtValue");

            if (rbSelect == null || txtVal == null)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            //switch on the type and get the value
            switch ((k_ITEM_TYPE_ID)ItemTypeID)
            {
                // laboratory and question free text components are handled the same
                case k_ITEM_TYPE_ID.Laboratory:
                case k_ITEM_TYPE_ID.QuestionFreeText:
                    // get the value from the text box
                    di.ComponentValue = txtVal.Text;
                    PatItemCompList.Add(di);
                    break;
                case k_ITEM_TYPE_ID.QuestionSelection:
                    // get the value from the radio button
                    di.ComponentValue = Convert.ToInt64((rbSelect.Checked) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False).ToString();
                    PatItemCompList.Add(di);
                    break;
                default:
                    PatItemCompList = null;
                    return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }
        }

        return new CStatus(true, k_STATUS_CODE.Success, "TODO");
    }

    /// <summary>
    ///US:1883 Abstract save control method
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CPatientItemData itemData = new CPatientItemData(BaseMstr.BaseData);
        CStatus status = new CStatus();
        long lPatItemID = -1;

        if (ddlItems.SelectedItem.Text == "[New Result]")
        {
            //load an item for insert
            CPatientItemDataItem di = new CPatientItemDataItem();
            di.PatientID = PatientID;
            di.ItemID = ItemID;
            di.SourceTypeID = (long)k_SOURCE_TYPE_ID.VAPPCT;

            //get the date time, which is a combination of the 2 controls
            di.EntryDate = CDataUtils.GetDate(txtEntryDate.Text, ucTimePicker.HH, ucTimePicker.MM, ucTimePicker.SS);

            // build a list of all the item components in the grid view
            CPatientItemCompList PatItemCompList = null;
            status = BuildPatItemCompList(out PatItemCompList);
            if (!status.Status)
            {
                return status;
            }

            // insert the patient item and all of its item components
            status = itemData.InsertPatientItem(di, PatItemCompList, out lPatItemID);
            if (!status.Status)
            {
                return status;
            }
        }
        else
        {
            lPatItemID = CDropDownList.GetSelectedLongID(ddlItems);
        }

        // update the comments if there is a new one
        if (!string.IsNullOrEmpty(txtComment.Text))
        {
            status = itemData.InsertPatientItemComment(lPatItemID, ItemID, txtComment.Text);
            if (!status.Status)
            {
                return status;
            }
        }

        //show status
        return new CStatus();
    }

    /// <summary>
    /// US:1880
    /// override
    /// Abstract validate user input method
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CStatus status = new CStatus();

        if (ddlItems.SelectedItem.Text != "[New Result]")
        {
            return status;
        }

        //make sure date range is valid
        DateTime dtEntryDate = CDataUtils.GetDate(
            txtEntryDate.Text,
            ucTimePicker.HH,
            ucTimePicker.MM,
            ucTimePicker.SS);

        if (dtEntryDate > DateTime.Now)
        {
            plistStatus.AddInputParameter("ERROR_FUTURE_DATE", Resources.ErrorMessages.ERROR_FUTURE_DATE);
        }
        
        //validate components
        bool bHasSelectedValue = false;
        foreach (GridViewRow gr in gvComponents.Rows)
        {
            TextBox txtValue = (TextBox)gr.FindControl("txtValue");
            Label lblComponent = (Label)gr.FindControl("lblComponent");
            RadioButton rbSelect = (RadioButton)gr.FindControl("rbSelComponent");

            if (txtValue == null
                || lblComponent == null
                || rbSelect == null)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }
            
            //switch on the type and get the value
            switch ((k_ITEM_TYPE_ID)ItemTypeID)
            {
                case k_ITEM_TYPE_ID.Laboratory:
                    {
                        bHasSelectedValue = true;

                        //get the min and max from the keys
                        double dblLegalMin = Convert.ToDouble(gvComponents.DataKeys[gr.DataItemIndex][nLegalMinIndex]);
                        double dblLegalMax = Convert.ToDouble(gvComponents.DataKeys[gr.DataItemIndex][nLegalMaxIndex]);

                        string strError = "Please enter a valid '"
                            + lblComponent.Text
                            + "' value, valid range is from"
                            + dblLegalMin.ToString()
                            + " to "
                            + dblLegalMax.ToString()
                            + ".";

                        // if the value is not numeric
                        // or if the value is outside the legal ranges
                        double dblValue = 0;
                        if (!double.TryParse(txtValue.Text, out dblValue)
                            || dblValue < dblLegalMin
                            || dblValue > dblLegalMax)
                        {
                            plistStatus.AddInputParameter("ERROR", strError);
                        }
                        break;
                    }

                case k_ITEM_TYPE_ID.NoteTitle:
                    bHasSelectedValue = true;
                    break;

                case k_ITEM_TYPE_ID.QuestionFreeText:
                    {
                        bHasSelectedValue = true;
                        if (txtValue.Text.Length < 1)
                        {
                            string strError = "Please enter a valid '"
                                + lblComponent.Text
                                + "' value!";

                            plistStatus.AddInputParameter("ERROR", strError);
                        }
                        break;
                    }

                case k_ITEM_TYPE_ID.QuestionSelection:
                    if (rbSelect.Checked)
                    {
                        bHasSelectedValue = true;
                    }
                    break;
             }
        }
        
        //make sure the user selected a radio button
        if(!bHasSelectedValue)
        {
            plistStatus.AddInputParameter("ERROR", "Please select a valid option!");
        }

        if (plistStatus.Count > 0)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        return status;
    }
          
    /// <summary>
    /// user clicked the ok button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickOK(object sender, EventArgs e)
    {
        //validate user input
        CParameterList plistStatus = null;
        CStatus status = ValidateUserInput(out plistStatus);
        if (!status.Status)
        {
            ShowMPE();
            ShowStatusInfo(
                status.StatusCode,
                plistStatus);
            return;
        }

        //save the data
        status = SaveControl();
        if (!status.Status)
        {
            ShowMPE();
            ShowStatusInfo(status);
            return;
        }

        Visible = false;
        ShowParentMPE();
        ((app_ucTimer)UCTimer).StartRefresh();
    }

    /// <summary>
    /// event
    /// starts the timer after before closing the dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {
        Visible = false;
        ShowParentMPE();
        ((app_ucTimer)UCTimer).StartRefresh();
    }
    
    /// <summary>
    /// US:1883 the selected item in the collection ddl was changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedColItem(object sender, EventArgs e)
    {
        txtComment.Text = string.Empty;

        if (ddlColItems.SelectedIndex > 0)
        {
            txtComment.Enabled = true;

            ItemID = Convert.ToInt64(ddlColItems.SelectedValue);

            CItemData Item = new CItemData(BaseMstr.BaseData);
            CItemDataItem di = null;
            CStatus status = Item.GetItemDI(ItemID, out di);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }

            ItemTypeID = di.ItemTypeID;

            status = LoadItemAndComponents(di);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }
        }
        else
        {
            txtComment.Enabled = false;
        }

        ShowMPE();
    }

    /// <summary>
    /// show or hide options for editing
    /// </summary>
    /// <param name="bShow"></param>
    protected void ShowEditOptions(bool bShow)
    {
        //show hide the panels and labels for edit
        lblDate.Visible = bShow;
        txtEntryDate.Visible = bShow;
        ucTimePicker.Visible = bShow;

        pnlComments.Visible = bShow;
        pnlComponents.Visible = bShow;

        lblItemComps.Visible = bShow;
        lblNewComment.Visible = bShow;
        lblCommentHistory.Visible = bShow;
        txtComment.Visible = bShow;

        if (!bShow)
        {
            pnlMapped.Visible = true;
        }
        else
        {
            pnlMapped.Visible = false;
        }
    }

    /// <summary>
    /// event
    /// loads the selected patient item into the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedItem(object sender, EventArgs e)
    {
        ShowMPE();

        if (ddlItems.SelectedIndex == 0)
        {
            foreach (GridViewRow gvr in gvComponents.Rows)
            {
                RadioButton rbSelect = (RadioButton)gvr.FindControl("rbSelComponent");
                TextBox txtValue = (TextBox)gvr.FindControl("txtValue");

                if (rbSelect == null || txtValue == null)
                {
                    ShowStatusInfo(new CStatus(false, k_STATUS_CODE.Failed, "TODO"));
                }
                rbSelect.Checked = false;
                rbSelect.Enabled = true;
                txtValue.Text = string.Empty;
                txtValue.ReadOnly = false;
            }

            if (ddlItems.SelectedItem.Text == "[New Result]")
            {
                ShowEditOptions(true);

                // set date/time
                txtEntryDate.ReadOnly = false;
                calEntryDate.Enabled = true;
                ucTimePicker.Enabled = true;
                txtEntryDate.Text = CDataUtils.GetDateAsString(DateTime.Now);
                calEntryDate.SelectedDate = DateTime.Now;
                ucTimePicker.SetTime(DateTime.Now);
                txtComment.Enabled = true;
            }
            else if (ddlItems.SelectedItem.Text == "[Select Result]")
            {
                txtEntryDate.ReadOnly = true;
                calEntryDate.Enabled = false;
                ucTimePicker.Enabled = false;
                txtEntryDate.Text = string.Empty;
                calEntryDate.SelectedDate = null;
                ucTimePicker.SetTime(0, 0, 0);
                txtComment.Enabled = false;

                ucNoteTitle.Clear();

                ShowEditOptions(false);
            }

            txtComment.Text = string.Empty;
            gvComments.DataSource = null;
            gvComments.DataBind();
        }
        else
        {
            ShowEditOptions(true);

            foreach (GridViewRow gvr in gvComponents.Rows)
            {
                RadioButton rbSelect = (RadioButton)gvr.FindControl("rbSelComponent");
                TextBox txtValue = (TextBox)gvr.FindControl("txtValue");
                if (rbSelect == null || txtValue == null)
                {
                    ShowStatusInfo(new CStatus(false, k_STATUS_CODE.Failed, "TODO"));
                }
                rbSelect.Enabled = false;
                txtValue.ReadOnly = true;
            }

            txtEntryDate.ReadOnly = true;
            calEntryDate.Enabled = false;
            ucTimePicker.Enabled = false;
            txtComment.Enabled = true;
            CStatus status = LoadPatItemAndComponents();
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }
        }
    }

    /// <summary>
    /// event
    /// maintains the entry date across postbacks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnEntryDateChanged(object sender, EventArgs e)
    {
        ShowMPE();
        try
        {
            calEntryDate.SelectedDate = Convert.ToDateTime(txtEntryDate.Text);
        }
        catch (Exception)
        {
            calEntryDate.SelectedDate = null;
            txtEntryDate.Text = string.Empty;
        }
    }
}
