using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucPatientChecklistItems : CAppUserControl
{
    /// <summary>
    /// property
    /// stores a patient checklist id for the user control
    /// </summary>
    public long PatientChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "PatientChecklistID"] = value; }
    }

    /// <summary>
    /// property
    /// stores a patient id for the user control
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
    /// stores a checklist id for the user control
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
    /// stores the enabled/disabled state of the user control
    /// </summary>
    public bool Enabled
    {
        get
        {
            object obj = ViewState[ClientID + "Enabled"];
            return (obj != null) ? Convert.ToBoolean(obj) : false;
        }
        set
        {
            ViewState[ClientID + "Enabled"] = value;
            EnableGV(value);
        }
    }

    /// <summary>
    /// property
    /// stores a patient item dataset for the user control
    /// </summary>
    protected DataTable PatientItems
    {
        get
        {
            object obj = ViewState[ClientID + "PatientItems"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "PatientItems"] = value; }
    }

    /// <summary>
    /// property
    /// stores a patient item component dataset for the user control
    /// </summary>
    protected DataTable PatientItemComponents
    {
        get
        {
            object obj = ViewState[ClientID + "PatientItemComponents"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "PatientItemComponents"] = value; }
    }

    public Unit Height
    {
        get { return pnlPatCLItems.Height; }
        set { pnlPatCLItems.Height = value; }
    }

    protected long TSColStateID { get; set; }
    protected long OSColStateID { get; set; }
    protected long DSColStateID { get; set; }

    /// <summary>
    /// event
    /// initializes the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucItemEditor.BaseMstr = BaseMstr;
        ucItemEditor.MPE = mpeItemEditor;
        ucItemEditor.UCTimer = UCTimer;
        ucItemEditor.ParentMPE = MPE;

        ucPatCLIEditor.BaseMstr = BaseMstr;
        ucPatCLIEditor.MPE = mpePatCLIEditor;
        ucPatCLIEditor.UCTimer = UCTimer;
        ucPatCLIEditor.ParentMPE = MPE;

        ucViewValuePopup.BaseMstr = BaseMstr;
        ucViewValuePopup.MPE = mpeViewValuePopup;
        ucViewValuePopup.UCTimer = UCTimer;
        ucPatCLIEditor.ParentMPE = MPE;

        ucPatItemHistory.BaseMstr = BaseMstr;
        ucPatItemHistory.MPE = mpePatItemHistory;
        ucPatItemHistory.ParentMPE = MPE;
    }

    /// <summary>
    /// event
    /// calls the loads for the grid view row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundItem(Object sender, GridViewRowEventArgs e)
    {
        GridViewRow gvr = (GridViewRow)e.Row;
        if (gvr == null)
        {
            return;
        }

        switch (gvr.RowType)
        {
            case DataControlRowType.DataRow:
                LoadGridViewRowValues(gvr);
                break;
            case DataControlRowType.Header:
                LoadGridViewHeader(gvr);
                break;
        }
    }

    /// <summary>
    /// event
    /// displays the edit checklist item component dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickEditItem(object sender, EventArgs e)
    {
        CPatChecklistData pcld = new CPatChecklistData(BaseMstr.BaseData);
        CPatChecklistDataItem di = null;
        CStatus status = pcld.GetPatChecklistDI(PatientChecklistID, out di);
        if (!status.Status)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_CURCHECKLIST);
            return;
        }

        if (di.ChecklistStateID == k_CHECKLIST_STATE_ID.Cancelled
            || di.ChecklistStateID == k_CHECKLIST_STATE_ID.Closed)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_CHECKLISTSTATE);
            return;
        }

        Button btnSender = (Button)sender;
        if (btnSender == null)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_ITEM);
            return;
        }

        ucItemEditor.PatientID = PatientID;
        ucItemEditor.ChecklistID = ChecklistID;
        ucItemEditor.ItemID = Convert.ToInt64(btnSender.Attributes["ITEM_ID"]);
        ucItemEditor.PatientChecklistID = PatientChecklistID;

        k_EDIT_MODE lEditMode = k_EDIT_MODE.INITIALIZE;
        status = ucItemEditor.LoadControl(lEditMode);
        if(!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
        
        ucItemEditor.ShowMPE();
    }

    /// <summary>
    /// event
    /// displays the override checklist item dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickOverrideItem(object sender, EventArgs e)
    {
        CPatChecklistData pcld = new CPatChecklistData(BaseMstr.BaseData);
        CPatChecklistDataItem di = null;
        CStatus status = pcld.GetPatChecklistDI(PatientChecklistID, out di);
        if (!status.Status)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_CURCHECKLIST);
            return;
        }

        if (di.ChecklistStateID == k_CHECKLIST_STATE_ID.Cancelled
            || di.ChecklistStateID == k_CHECKLIST_STATE_ID.Closed)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_CHECKLISTSTATE);
            return;
        }

        Button btnSender = (Button)sender;
        if (btnSender == null)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_ITEM);
            return;
        }

        ucPatCLIEditor.PatientID = PatientID;
        ucPatCLIEditor.ChecklistID = ChecklistID;
        ucPatCLIEditor.ItemID = Convert.ToInt64(btnSender.Attributes["ITEM_ID"]);
        ucPatCLIEditor.PatientChecklistID = PatientChecklistID;

        k_EDIT_MODE lEditMode = k_EDIT_MODE.INITIALIZE;
        status = ucPatCLIEditor.LoadControl(lEditMode);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucPatCLIEditor.ShowMPE();
    }

    protected void OnClickViewValue(object sender, EventArgs e)
    {
        Button btnSender = (Button)sender;
        if (btnSender == null)
        {
            ShowMPE();
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_ITEM);
            return;
        }

        ucViewValuePopup.ItemID = Convert.ToInt64(btnSender.Attributes["ITEM_ID"]);
        ucViewValuePopup.ItemTypeID = Convert.ToInt64(btnSender.Attributes["ITEM_TYPE_ID"]);
        ucViewValuePopup.PatientID = PatientID;
        CStatus status = ucViewValuePopup.LoadControl(EditMode);
        if (!status.Status)
        {
            ShowMPE();
            ShowStatusInfo(status);
            return;
        }

        ucViewValuePopup.ShowMPE();
    }

    /// <summary>
    /// US:888 displays the item history when the trend button is pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickTrend(object sender, EventArgs e)
    {
        ShowMPE();

        Button btnSender = (Button)sender;
        if (btnSender == null)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_ITEM);
            return;
        }

        ucPatItemHistory.PatientID = PatientID;
        ucPatItemHistory.ItemID = Convert.ToInt64(btnSender.Attributes["ITEM_ID"]);
        CStatus status = ucPatItemHistory.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
        ucPatItemHistory.ShowMPE();
    }

    protected void LoadGridViewHeader(GridViewRow gvr)
    {
        if (gvr == null)
        {
            return;
        }

        Image imgTSColumnState = (Image)gvr.FindControl("imgTSColumnState");
        Image imgOSColumnState = (Image)gvr.FindControl("imgOSColumnState");
        Image imgDSColumnState = (Image)gvr.FindControl("imgDSColumnState");

        if (imgTSColumnState == null
            || imgOSColumnState == null
            || imgDSColumnState == null)
        {
            return;
        }

        imgTSColumnState.Visible = true;
        switch (TSColStateID)
        {
            case (long)k_STATE_ID.Bad:
                imgTSColumnState.ImageUrl = Resources.Images.STATE_BAD;
                break;
            case (long)k_STATE_ID.Good:
                imgTSColumnState.ImageUrl = Resources.Images.STATE_GOOD;
                break;
            default:
                imgTSColumnState.ImageUrl = Resources.Images.STATE_UNKNOWN;
                break;
        }

        imgOSColumnState.Visible = true;
        switch (OSColStateID)
        {
            case (long)k_STATE_ID.Bad:
                imgOSColumnState.ImageUrl = Resources.Images.STATE_BAD;
                break;
            case (long)k_STATE_ID.Good:
                imgOSColumnState.ImageUrl = Resources.Images.STATE_GOOD;
                break;
            default:
                imgOSColumnState.ImageUrl = Resources.Images.STATE_UNKNOWN;
                break;
        }

        imgDSColumnState.Visible = true;
        switch (DSColStateID)
        {
            case (long)k_STATE_ID.Bad:
                imgDSColumnState.ImageUrl = Resources.Images.STATE_BAD;
                break;
            case (long)k_STATE_ID.Good:
                imgDSColumnState.ImageUrl = Resources.Images.STATE_GOOD;
                break;
            default:
                imgDSColumnState.ImageUrl = Resources.Images.STATE_UNKNOWN;
                break;
        }
    }

    /// <summary>
    /// method
    /// US:880 US:882
    /// sets the state text and background color for the item states
    /// </summary>
    /// <param name="gvr"></param>
    protected void LoadGridViewRowStates(GridViewRow gvr)
    {
        if (gvr == null)
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

        Image imgRowState = (Image)gvr.FindControl("imgRowState");
        Label lblTemporalState = (Label)gvr.FindControl("lblTemporalState");
        Label lblOutcomeState = (Label)gvr.FindControl("lblOutcomeState");
        Label lblDecisionState = (Label)gvr.FindControl("lblDecisionState");

        if (imgRowState == null
            || lblTemporalState == null
            || lblOutcomeState == null
            || lblDecisionState == null)
        {
            return;
        }

        switch (Convert.ToInt32(dr["wr_state_id"]))
        {
            case (int)k_STATE_ID.Bad:
                imgRowState.ImageUrl = Resources.Images.STATE_BAD;
                break;
            case (int)k_STATE_ID.Good:
                imgRowState.ImageUrl = Resources.Images.STATE_GOOD;
                break;
            default:
                imgRowState.ImageUrl = Resources.Images.STATE_UNKNOWN;
                break;
        }

        lblTemporalState.Text = dr["TS_LABEL"].ToString();
        switch (Convert.ToInt32(dr["TS_STATE_ID"]))
        {
            case (int)k_STATE_ID.Bad:
                lblTemporalState.BackColor = System.Drawing.Color.Red;
                break;
            case (int)k_STATE_ID.Good:
                lblTemporalState.BackColor = System.Drawing.Color.Green;
                break;
            default:
                lblTemporalState.BackColor = System.Drawing.Color.Yellow;
                break;
        }

        lblOutcomeState.Text = dr["OS_LABEL"].ToString();
        switch (Convert.ToInt32(dr["OS_STATE_ID"]))
        {
            case (int)k_STATE_ID.Bad:
                lblOutcomeState.BackColor = System.Drawing.Color.Red;
                break;
            case (int)k_STATE_ID.Good:
                lblOutcomeState.BackColor = System.Drawing.Color.Green;
                break;
            default:
                lblOutcomeState.BackColor = System.Drawing.Color.Yellow;
                break;
        }

        lblDecisionState.Text = dr["DS_LABEL"].ToString();
        switch (Convert.ToInt32(dr["DS_STATE_ID"]))
        {
            case (int)k_STATE_ID.Bad:
                lblDecisionState.BackColor = System.Drawing.Color.Red;
                break;
            case (int)k_STATE_ID.Good:
                lblDecisionState.BackColor = System.Drawing.Color.Green;
                break;
            default:
                lblDecisionState.BackColor = System.Drawing.Color.Yellow;
                break;
        }
    }

    /// <summary>
    /// method
    /// loads the date and component values for a row
    /// </summary>
    /// <param name="gvr"></param>
    protected void LoadGridViewRowComponents(GridViewRow gvr)
    {
        if (gvr == null)
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

        Panel pnlComponents = (Panel)gvr.FindControl("pnlComponents");
        Panel pnlViewValue = (Panel)gvr.FindControl("pnlViewValue");
        Literal litHiddenValues = (Literal)gvr.FindControl("litHiddenValues");
        Literal litShownValues = (Literal)gvr.FindControl("litShownValues");
        Button btnShowHiddenValues = (Button)gvr.FindControl("btnShowHiddenValues");
        Button btnTrend = (Button)gvr.FindControl("btnTrend");

        if (pnlComponents == null
            || litHiddenValues == null
            || litShownValues == null
            || btnShowHiddenValues == null
            || pnlViewValue == null
            || btnTrend == null)
        {
            return;
        }

        pnlComponents.Visible = true;
        pnlViewValue.Visible = false;
        btnShowHiddenValues.Enabled = true;

        litHiddenValues.Text = string.Empty;
        litShownValues.Text = string.Empty;

        // filter the patient items by item id for the item we're on
        DataRow[] draPatientItems = PatientItems.Select("ITEM_ID = " + dr["ITEM_ID"].ToString());
        if (draPatientItems == null || draPatientItems.Count() < 1)
        {
            litShownValues.Text = "NA";
            btnShowHiddenValues.Enabled = false;
            btnTrend.Enabled = false;
            return;
        }

        long lItemTypeID = Convert.ToInt64(dr["ITEM_TYPE_ID"]);
        if (lItemTypeID == (long)k_ITEM_TYPE_ID.Laboratory)
        {
            btnTrend.Enabled = true;
        }

        // filter by patient item id for the first item in the list
        // the first item in the list should be the the most recent one
        // if the first item in the list isnt the most recent check the stored procedure for errors
        DataRow[] draPatientItemComps = PatientItemComponents.Select("PAT_ITEM_ID = " + draPatientItems[0]["PAT_ITEM_ID"].ToString());
        if (draPatientItemComps == null && draPatientItemComps.Count() < 1)
        {
            litShownValues.Text = "NA";
            btnShowHiddenValues.Enabled = false;
            btnTrend.Enabled = false;
            return;
        }

        foreach (DataRow drItemComp in draPatientItemComps)
        {
            StringBuilder sbItemCompHTML = new StringBuilder();
            sbItemCompHTML.Append("<img alt=\"");
            sbItemCompHTML.Append(Server.HtmlEncode(drItemComp["ITEM_COMPONENT_LABEL"].ToString()));
            sbItemCompHTML.Append(" Component State Image\" width=\"10\" height=\"10\" src=\"");

            long lStateID = Convert.ToInt64(drItemComp["IC_STATE_ID"]);
            switch (lStateID)
            {
                case (long)k_STATE_ID.Good:
                    sbItemCompHTML.Append(Resources.Images.STATE_GOOD_SMALL);
                    break;
                case (long)k_STATE_ID.Bad:
                    sbItemCompHTML.Append(Resources.Images.STATE_BAD_SMALL);
                    break;
                case (long)k_STATE_ID.Unknown:
                    sbItemCompHTML.Append(Resources.Images.STATE_UNKNOWN_SMALL);
                    break;
            }

            sbItemCompHTML.Append("\" />");
            sbItemCompHTML.Append("<span style=\"font-weight:bold;\">");
            sbItemCompHTML.Append(Server.HtmlEncode(drItemComp["ITEM_COMPONENT_LABEL"].ToString()));

            switch (lItemTypeID)
            {
                case (long)k_ITEM_TYPE_ID.Laboratory:
                case (long)k_ITEM_TYPE_ID.QuestionFreeText:
                    sbItemCompHTML.Append(": </span>");
                    sbItemCompHTML.Append(Server.HtmlEncode(drItemComp["COMPONENT_VALUE"].ToString()));
                    break;
                case (long)k_ITEM_TYPE_ID.QuestionSelection:
                    sbItemCompHTML.Append("</span>");
                    break;
            }

            sbItemCompHTML.Append("<div class=\"app_horizontal_spacer\"></div>");

            switch (lStateID)
            {
                case (long)k_STATE_ID.Good:
                    litHiddenValues.Text += sbItemCompHTML.ToString();
                    break;
                case (long)k_STATE_ID.Bad:
                case (long)k_STATE_ID.Unknown:
                    litShownValues.Text += sbItemCompHTML.ToString();
                    break;
            }
        }

        btnShowHiddenValues.Enabled = (String.IsNullOrEmpty(litHiddenValues.Text)) ? false : btnShowHiddenValues.Enabled;
    }

    protected void LoadGridViewValue(GridViewRow gvr)
    {
        if (gvr == null)
        {
            return;
        }

        Panel pnlComponents = (Panel)gvr.FindControl("pnlComponents");
        Panel pnlViewValue = (Panel)gvr.FindControl("pnlViewValue");

        if (pnlComponents == null || pnlViewValue == null)
        {
            return;
        }

        pnlComponents.Visible = false;
        pnlViewValue.Visible = true;
    }

    /// <summary>
    /// method
    /// loads the values from the data row into the grid view row controls
    /// </summary>
    /// <param name="gvr"></param>
    protected void LoadGridViewRowValues(GridViewRow gvr)
    {
        if (gvr == null)
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

        HiddenField hfEnableID = (HiddenField)gvr.FindControl("hfEnableID");
        Label lblItemLabel = (Label)gvr.FindControl("lblItemLabel");
        Button btnEdit = (Button)gvr.FindControl("btnEdit");
        Button btnOverride = (Button)gvr.FindControl("btnOverride");
        Button btnViewValue = (Button)gvr.FindControl("btnViewValue");
        Button btnTrend = (Button)gvr.FindControl("btnTrend");

        if (hfEnableID == null
            || lblItemLabel == null
            || btnEdit == null
            || btnOverride == null
            || btnViewValue == null
            || btnTrend == null)
        {
            return;
        }

        hfEnableID.Value = dr["IS_ENABLED"].ToString();

        lblItemLabel.Text = dr["ITEM_LABEL"].ToString();

        btnEdit.Attributes.Add("ITEM_ID", dr["ITEM_ID"].ToString());
        btnOverride.Attributes.Add("ITEM_ID", dr["ITEM_ID"].ToString());
        
        btnViewValue.Attributes.Add("ITEM_ID", dr["ITEM_ID"].ToString());
        btnViewValue.Attributes.Add("ITEM_TYPE_ID", dr["ITEM_TYPE_ID"].ToString());

        btnTrend.Attributes.Add("ITEM_ID", dr["ITEM_ID"].ToString());
        btnTrend.Attributes.Add("ITEM_TYPE_ID", dr["ITEM_TYPE_ID"].ToString());
        btnTrend.Enabled = false;
        
        //get the item type id
        switch (Convert.ToInt64(dr["ITEM_TYPE_ID"]))
        {
            case (long)k_ITEM_TYPE_ID.Laboratory:
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                LoadGridViewRowComponents(gvr);
                break;
            case (long)k_ITEM_TYPE_ID.Collection:
            case (long)k_ITEM_TYPE_ID.NoteTitle:
                LoadGridViewValue(gvr);
                break;
        }

        LoadGridViewRowStates(gvr);
        EnableGVR(gvr, Enabled);
    }

    /// <summary>
    /// method
    /// enables/disables the grid view row
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="bEnable"></param>
    private void EnableGVR(GridViewRow gvr, bool bEnable)
    {
        if (gvr == null)
        {
            return;
        }

        HiddenField hfEnableID = (HiddenField)gvr.FindControl("hfEnableID");
        Button btnEdit = (Button)gvr.FindControl("btnEdit");
        Button btnOverride = (Button)gvr.FindControl("btnOverride");

        if (hfEnableID == null || btnEdit == null || btnOverride == null)
        {
            return;
        }

        long lEnableID = Convert.ToInt64(hfEnableID.Value);
        bEnable = (bEnable && lEnableID == (long)k_TRUE_FALSE_ID.True) ? true : false;

        btnEdit.Enabled = bEnable;

        btnOverride.Enabled = bEnable;
    }

    /// <summary>
    /// method
    /// enables/disables the grid view
    /// </summary>
    /// <param name="bEnable"></param>
    private void EnableGV(bool bEnable)
    {
        foreach (GridViewRow gvr in gvPatCLItems.Rows)
        {
            EnableGVR(gvr, bEnable);
        }
    }

    /// <summary>
    /// override
    /// US:880
    /// loads the user control with the data related to the patient checklist id stored in the patient checklist id property
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;
        
        CPatChecklistItemData pci = new CPatChecklistItemData(BaseMstr.BaseData);
        long lTSColStateID = 0;
        long lOSColStateID = 0;
        long lDSColStateID = 0;
        long lSummaryStateID = 0;
        DataSet dsChecklistItems = null;
        CStatus statusPatCL = pci.GetPatCLItemsByPatCLIDDS(
            PatientChecklistID,
            out lTSColStateID,
            out lOSColStateID,
            out lDSColStateID,
            out lSummaryStateID,
            out dsChecklistItems);
        if (!statusPatCL.Status)
        {
            return statusPatCL;
        }

        TSColStateID = lTSColStateID;
        OSColStateID = lOSColStateID;
        DSColStateID = lDSColStateID;

        lblSummaryState.Visible = true;
        imgSummaryState.Visible = true;
        switch (lSummaryStateID)
        {
            case (long)k_STATE_ID.Bad:
                imgSummaryState.ImageUrl = Resources.Images.STATE_BAD_LARGE;
                break;
            case (long)k_STATE_ID.Good:
                imgSummaryState.ImageUrl = Resources.Images.STATE_GOOD_LARGE;
                break;
            default:
                imgSummaryState.ImageUrl = Resources.Images.STATE_UNKNOWN_LARGE;
                break;
        }

        CPatientData p = new CPatientData(BaseMstr.BaseData);
        DataSet dsPatientItems = null;
        CStatus statusPatItems = p.GetPatItemsByPatCLIDDS(PatientChecklistID, out dsPatientItems);
        if (!statusPatItems.Status)
        {
            return statusPatItems;
        }
        PatientItems = dsPatientItems.Tables[0];

        DataSet dsPatientItemComponents = null;
        CStatus statusPatItemComps = p.GetPatItemCompsByPatCLIDDS(PatientChecklistID, out dsPatientItemComponents);
        if (!statusPatItemComps.Status)
        {
            return statusPatItemComps;
        }
        PatientItemComponents = dsPatientItemComponents.Tables[0];

        gvPatCLItems.DataSource = dsChecklistItems.Tables[0];
        gvPatCLItems.DataBind();

        //set the ui permissions based on the users role
        SetPermissions();

        return new CStatus();
    }

    /// <summary>
    /// US:865 sets UI permssions based on the users role
    /// </summary>
    public void SetPermissions()
    {
        //do permissions work
        CChecklistPermissionsDataItem pdi = null;
        CChecklistData clData = new CChecklistData(BaseMstr.BaseData);
        clData.GetCheckListPermissionsDI(ChecklistID, out pdi);

        //does the user have read only permissions to this checklist
        if (pdi.HasPermission(BaseMstr.AppUser, k_CHECKLIST_PERMISSION.ReadOnly))
        {
            foreach (GridViewRow gr in gvPatCLItems.Rows)
            {
                Button btnOR = (Button)gr.FindControl("btnOverride");
                if (btnOR != null)
                {
                    btnOR.Enabled = false;
                }

                Button btnED = (Button)gr.FindControl("btnEdit");
                if (btnED != null)
                {
                    btnED.Enabled = false;
                }
            }

        }
        else
        {
            //loop over the items and determine if the user is allowed to override
            //decision state for each item
            foreach (GridViewRow gr in gvPatCLItems.Rows)
            {
                if (gvPatCLItems.DataKeys[gr.RowIndex].Value != null)
                {
                    //get the item id
                    long lItemID = Convert.ToInt32(gvPatCLItems.DataKeys[gr.RowIndex].Value);

                    //load a item permissions data item
                    CChecklistItemPermissionsDataItem pi = null;
                    CChecklistItemData cli = new CChecklistItemData(BaseMstr.BaseData);
                    CStatus status = new CStatus();
                    status = cli.GetCLPermissionItem( ChecklistID,
                                                      lItemID,
                                                      out pi);
                    if (status.Status)
                    {
                        if(!pi.HasPermission( BaseMstr.AppUser,
                                              k_CHECKLIST_PERMISSION.DSOverride))
                        {
                            Button btnOR = (Button)gr.FindControl("btnOverride");
                            if (btnOR != null)
                            {
                                btnOR.Enabled = false;
                            }

                            Button btnED = (Button)gr.FindControl("btnEdit");
                            if (btnED != null)
                            {

                            }
                        }                   
                    }                    
                }
            }
        }
    }

    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }
}