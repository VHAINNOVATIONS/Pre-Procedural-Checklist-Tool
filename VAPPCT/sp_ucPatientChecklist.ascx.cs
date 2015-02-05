using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;
using System.Data;

public partial class sp_ucPatientChecklist : CAppUserControl
{
    /// <summary>
    /// property
    /// stores a patient id for the page
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
            return (obj != null) ? Convert.ToInt64(obj.ToString()) : -1;
        }
        set { ViewState[ClientID + "ChecklistID"] = value; }
    }

    ///<summary>
    ///property
    /// stores a checklist label for the page
    /// </summary>
    public string ChecklistLabel
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistLabel"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "ChecklistLabel"] = value; }
    }

    /// <summary>
    /// property
    /// stores a checklist state for the page
    /// </summary>
    protected k_CHECKLIST_STATE_ID ChecklistStateID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistStateID"];
            return (obj != null) ? (k_CHECKLIST_STATE_ID)obj : k_CHECKLIST_STATE_ID.Closed;
        }
        set { ViewState[ClientID + "ChecklistStateID"] = value; }
    }

    /// <summary>
    /// property
    /// stores a patient checklist id
    /// </summary>
    public long PatCLID
    {
        get
        {
            object obj = ViewState[ClientID + "PatCLID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "PatCLID"] = value; }
    }

    /// <summary>
    /// enable/disable tiu note button
    /// </summary>
    protected void EnableTIU()
    {
        if (ddlPatChecklist.SelectedValue != string.Empty)
        {
            long lPatCLID = Convert.ToInt32(ddlPatChecklist.SelectedValue);

            //checklist data
            CChecklistDataItem diChecklist = new CChecklistDataItem();
            CChecklistData clData = new CChecklistData(BaseMstr.BaseData);
            clData.GetCheckListDI(ChecklistID, out diChecklist);
            string strNoteTitleTag = diChecklist.NoteTitleTag;
            if (strNoteTitleTag == null)
            {
                btnTIU.Enabled = false;
                return;
            }
            else
            {
                btnTIU.Enabled = true;
            }

            if (strNoteTitleTag == "-1")
            {
                btnTIU.Enabled = false;
            }
            else
            {
                btnTIU.Enabled = true;
            }
        }
        else
        {
            btnTIU.Enabled = false;
        }

    }

    /// <summary>
    /// event
    /// initializes the page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucPatCLItems.BaseMstr = BaseMstr;
        ucPatCLItems.MPE = MPE;
        ucPatCLItems.UCTimer = ucTimer;

        ucTIUNote.BaseMstr = BaseMstr;
        ucTIUNote.MPE = mpeTIU;
        ucTIUNote.ParentMPE = MPE;

        ucChecklistSelector.BaseMstr = BaseMstr;
        ucChecklistSelector.MPE = mpeChecklistSelector;
        ucChecklistSelector.ParentMPE = MPE;

        ucExistingChecklist.BaseMstr = BaseMstr;
        ucExistingChecklist.MPE = mpeExisting;
        ucExistingChecklist.ParentMPE = MPE;

        ucUpdateChecklistVersion.BaseMstr = BaseMstr;
        ucUpdateChecklistVersion.MPE = mpeUpdateChecklistVersion;
        ucUpdateChecklistVersion.ParentMPE = MPE;

        //enable the update checklist button if the patient has 
        //checklists that need updated
        if (ChecklistID == -1 )
        {
            btnUpdateCLVersion.Enabled = false;
            lblVersion.Text = "";
        }

        //don't allow the user to write a note if one is not mapped to the cl
        if (!IsPostBack)
        {
            EnableTIU();
        }

    }

    /// <summary>
    /// US:894
    /// US:878
    /// event
    /// maintains the procedure date across postbacks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnProcedureDateChanged(object sender, EventArgs e)
    {
        ShowMPE();
        try
        {
            calProcedureDate.SelectedDate = Convert.ToDateTime(tbProcedureDate.Text);
        }
        catch(Exception)
        {
            calProcedureDate.SelectedDate = null;
            tbProcedureDate.Text = string.Empty;
        }
    }

    /// <summary>
    /// event
    /// US:878
    /// saves the checklist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSaveChecklist(object sender, EventArgs e)
    {
        ShowMPE();

        CParameterList plistStatus = null;

        CStatus status = ValidateUserInput(out plistStatus);
        if (!status.Status)
        {
            ShowStatusInfo(status.StatusCode, plistStatus);
            return;
        }

        //if we are closing the checklist then write a note
        if (CDataUtils.ToLong(ddlChecklistState.SelectedValue) == (long)k_CHECKLIST_STATE_ID.Closed 
            && btnTIU.Enabled)
        {
            ShowTIUNote();
        }
        else
        {
            status = SaveControl();
            if (!status.Status)
            {
                ShowStatusInfo(status.StatusCode, plistStatus);
                return;
            }
        }
    }

    /// <summary>
    /// displays the TIU Note user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnTIUNote(object sender, EventArgs e)
    {
        ShowTIUNote();
    }
       
    /// <summary>
    /// fired when a note is successfully saved
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnNoteSaved(object sender, CAppUserControlArgs e)
    {
        //if we are closing this checklist then save it.
        if (CDataUtils.ToLong(ddlChecklistState.SelectedValue) == (long)k_CHECKLIST_STATE_ID.Closed)
        {
            CStatus status = SaveControl();
            if (!status.Status)
            {
                ShowStatusInfo(status.StatusCode, status.StatusComment);
                return;
            }
        }
    }

    /// <summary>
    /// shows the tiu note entry control
    /// </summary>
    protected void ShowTIUNote()
    {
        //get the selected patient checklist id
        ucTIUNote.PatChecklistID = Convert.ToInt32(ddlPatChecklist.SelectedValue);
        ucTIUNote.PatientID = PatientID;
        CStatus status = ucTIUNote.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            ShowMPE();
            ShowStatusInfo(status);
            return;
        }

        ucTIUNote.ShowMPE();
    }

    /// <summary>
    /// update the checklist version
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUpdateCLVersion(object sender, EventArgs e)
    {
        ucUpdateChecklistVersion.PatientID = PatientID;
        ucUpdateChecklistVersion.ChecklistID = ChecklistID;

        ucUpdateChecklistVersion.LoadControl(k_EDIT_MODE.INITIALIZE);
        ucUpdateChecklistVersion.ShowMPE();
    }

    /// <summary>
    /// reload the selected checklist after updaing the CL version
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUpdateCLVersion(object sender, CAppUserControlArgs e)
    {
        LoadChecklist();
    }

    /// <summary>
    /// event
    /// displays the checklist selector dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickAssignChecklist(object sender, EventArgs e)
    {
        ucChecklistSelector.ActiveChecklistsOnly = true;
        CStatus status = ucChecklistSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            ShowMPE();
            ShowStatusInfo(status);
            return;
        }

        ucChecklistSelector.ShowMPE();
    }

    /// <summary>
    /// event
    /// captures the selected checklist id and assigns it
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnChecklistSelect(object sender, CAppUserControlArgs e)
    {
        ShowMPE();
        ChecklistID = Convert.ToInt32(e.EventData);
        if (ChecklistID < 1)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_CHECKLIST);
            return;
        }

        ucExistingChecklist.ChecklistID = ChecklistID;
        ucExistingChecklist.PatientID = PatientID;
        CStatus status = ucExistingChecklist.LoadControl(k_EDIT_MODE.READ_ONLY);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
    }

    protected void OnContinueExisting(object sender, CAppUserControlArgs e)
    {
        CPatChecklistDataItem di = new CPatChecklistDataItem();
        di.ProcedureDate = CDataUtils.GetNullDate();
        di.AssignmentDate = DateTime.Now;
        di.ChecklistID = ucExistingChecklist.ChecklistID;
        di.ChecklistStateID = k_CHECKLIST_STATE_ID.Open;
        di.PatientID = PatientID;
        di.StateID = k_STATE_ID.Unknown;

        long lPatCLID = 0;
        CPatChecklistData pcld = new CPatChecklistData(BaseMstr.BaseData);
        CStatus status = pcld.InsertPatChecklist(di, out lPatCLID);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (BaseMstr.MDWSTransfer)
        {
            //talk to the communicator to update the 
            //patient checklist from mdws
            CCommunicator com = new CCommunicator();
            status = com.RefreshPatientCheckList(
                BaseMstr.DBConn,
                BaseMstr.BaseData,
                PatientID,
                lPatCLID);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }
        }

        CPatientChecklist pcl = new CPatientChecklist();
        status = pcl.LoadPatientChecklists(BaseMstr, PatientID, ddlPatChecklist);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
        ddlPatChecklist.SelectedValue = lPatCLID.ToString();

        status = LoadChecklist();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        //set the UI permissions based on the user logged in
        SetPermissions(btnTIU.Enabled);
    }

    /// <summary>
    /// event
    /// pulls the latest data from MDWS and loads the checklist items'
    /// everytime the timer ticks
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRefresh(object sender, EventArgs e)
    {
        ShowMPE();
        CStatus status = null;
        if (BaseMstr.MDWSTransfer)
        {
            //get the selected patient checklist id
            long lPatCLID = Convert.ToInt64(ddlPatChecklist.SelectedValue);

            //talk to the communicator to update the 
            //patient checklist from mdws
            CCommunicator com = new CCommunicator();
            status  = com.RefreshPatientCheckList(
                BaseMstr.DBConn,
                BaseMstr.BaseData,
                PatientID,
                lPatCLID);
            if (!status.Status)
            {
                ShowStatusInfo(status);
                return;
            }
        }

        status = LoadChecklist();
        if (!status.Status)
        {
            ShowStatusInfo(status);
        }

        //set the UI permissions based on the user logged in
        SetPermissions(btnTIU.Enabled);
    }

    /// <summary>
    /// US:1880 method
    /// loads the selected checklist
    /// </summary>
    /// <returns></returns>
    protected CStatus LoadChecklist()
    {
        PatCLID = Convert.ToInt64(ddlPatChecklist.SelectedValue);
        if (PatCLID < 1)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        CPatientChecklist pcl = new CPatientChecklist();
        CStatus status = pcl.LoadPatientChecklists(BaseMstr, PatientID, ddlPatChecklist);
        if (!status.Status)
        {
            return status;
        }
        ddlPatChecklist.SelectedValue = PatCLID.ToString();

        CPatChecklistData pcld = new CPatChecklistData(BaseMstr.BaseData);
        CPatChecklistDataItem di = null;
        status = pcld.GetPatChecklistDI(PatCLID, out di);
        if (!status.Status)
        {
            return status;
        }
        ddlChecklistState.SelectedValue = Convert.ToInt64(di.ChecklistStateID).ToString();
        ChecklistID = di.ChecklistID;
        ChecklistStateID = di.ChecklistStateID;
        EnableBasedOnChecklistState();

        //enable/disable the button based on the checklist selected
        CPatChecklistData dta = new CPatChecklistData(BaseMstr.BaseData);
        DataSet dsCL = null;
        dta.GetOutOfDatePatCLDS(PatientID, ChecklistID, out dsCL);
        if (!CDataUtils.IsEmpty(dsCL) )
        {
            btnUpdateCLVersion.Enabled = true;
            lblVersion.Text = "New Version Available!";
        }
        else
        {
            btnUpdateCLVersion.Enabled = false;
            lblVersion.Text = "Version is Current.";
        }


        if (!CDataUtils.IsDateNull(di.ProcedureDate))
        {
            tbProcedureDate.Text = CDataUtils.GetDateAsString(di.ProcedureDate);
            calProcedureDate.SelectedDate = di.ProcedureDate;
            ucProcedureTime.SetTime(di.ProcedureDate);
        }
        else
        {
            tbProcedureDate.Text = string.Empty;
            calProcedureDate.SelectedDate = null;
            ucProcedureTime.HH = 0;
            ucProcedureTime.MM = 0;
            ucProcedureTime.SS = 0;
        }

        //checklist data - check for notetitle and disable tiu if we dont have one
        CChecklistData clData = new CChecklistData(BaseMstr.BaseData);
        CChecklistDataItem diChecklist = null;
        status = clData.GetCheckListDI(di.ChecklistID, out diChecklist);
        if (!status.Status)
        {
            return status;
        }

        btnTIU.Enabled = (diChecklist.NoteTitleTag != "-1") ? true: false;

        CPatientData p = new CPatientData(BaseMstr.BaseData);
        string strBlurb = string.Empty;
        status = p.GetPatientBlurb(PatientID, out strBlurb);
        if (!status.Status)
        {
            return status;
        }

        sPatientBlurb.InnerText = strBlurb + " for " + diChecklist.ChecklistLabel;

        ucPatCLItems.PatientChecklistID = di.PatCLID;
        ucPatCLItems.PatientID = di.PatientID;
        ucPatCLItems.ChecklistID = di.ChecklistID;
        status = LoadPatientChecklistItems();
        if (!status.Status)
        {
            return status;
        }

        EnableTIU();

        return new CStatus();
    }

    /// <summary>
    /// US:894
    /// event
    /// loads the items for the selected checklist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelChangeChecklist(object sender, EventArgs e)
    {
        ShowMPE();

        ChecklistID = 0;
        ChecklistLabel = string.Empty;
        ChecklistStateID = k_CHECKLIST_STATE_ID.Null;
        PatCLID = 0;

        CStatus status = LoadChecklist();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        //set the UI permissions based on the user logged in
        SetPermissions(btnTIU.Enabled);

        //if no tiu tied to this note then disable the button
        EnableTIU();
    }

    /// <summary>
    /// method
    /// loads the selected patient's checklists in the checklist drop down list
    /// </summary>
    /// <returns></returns>
    protected CStatus LoadDDLs()
    {
        CPatientChecklist pcl = new CPatientChecklist();
        CStatus status = pcl.LoadPatientChecklists(BaseMstr, PatientID, ddlPatChecklist);
        if (!status.Status)
        {
            return status;
        }

        status = CSTAT.LoadChecklistStateDDL(BaseMstr.BaseData, ddlChecklistState);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads the items for the selected checklist
    /// </summary>
    protected CStatus LoadPatientChecklistItems()
    {
        CStatus status = ucPatCLItems.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// enables/disables the page's controls based on the checklist state
    /// </summary>
    protected void EnableBasedOnChecklistState()
    {
        switch (Convert.ToInt64(ddlChecklistState.SelectedValue))
        {
            case (long)k_CHECKLIST_STATE_ID.Open:
                Enable(true);
                //set the permissions on the items
                ucPatCLItems.SetPermissions();
                break;
            default:
                Enable(false);
                break;
        }
    }

    /// <summary>
    /// US:894
    /// method
    /// calls the enable based on checklist state method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelChangeChecklistState(object sender, EventArgs e)
    {
        ShowMPE();
        EnableBasedOnChecklistState();
    }

    /// <summary>
    /// method
    /// enables/disables the page's controls
    /// </summary>
    /// <param name="bEnable"></param>
    public void Enable(bool bEnable)
    {
        lblChecklistState.Enabled = (ChecklistStateID == k_CHECKLIST_STATE_ID.Open) ? true : bEnable;
        ddlChecklistState.Enabled = lblChecklistState.Enabled;
        lblProcedureDate.Enabled = bEnable;
        tbProcedureDate.Enabled = bEnable;
        ucProcedureTime.Enabled = bEnable;
        ucPatCLItems.Enabled = bEnable;
    }

    /// <summary>
    /// US:894
    /// US:892
    /// override
    /// initializes the control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        Enable(false);

        CPatientData p = new CPatientData(BaseMstr.BaseData);
        string strBlurb = string.Empty;
        CStatus status = p.GetPatientBlurb(PatientID, out strBlurb);
        if (!status.Status)
        {
            return status;
        }

        sPatientBlurb.InnerText = strBlurb;

        status = LoadDDLs();
        if (!status.Status)
        {
            return status;
        }

        if (EditMode == k_EDIT_MODE.UPDATE)
        {
            ddlPatChecklist.SelectedValue = PatCLID.ToString();

            //Manually Forcing OnSelChangeChecklist to fire
            OnSelChangeChecklist(this, new EventArgs());
        }

        return new CStatus();
    }

    /// <summary>
    /// US:864 US:876 sets UI permssions based on the users role
    /// </summary>
    public void SetPermissions(bool bHasNoteTitle)
    {
        //do permissions work
        CChecklistPermissionsDataItem pdi = null;
        CChecklistData clData = new CChecklistData(BaseMstr.BaseData);
        CStatus status = clData.GetCheckListPermissionsDI(ChecklistID, out pdi);
        if (!status.Status)
        {
            ddlChecklistState.Enabled = false;
            btnSaveCL.Enabled = false;
            ddlChecklistState.Enabled = false;
            tbProcedureDate.Enabled = false;
            calProcedureDate.Enabled = false;
            ucProcedureTime.Enabled = false;
            ShowStatusInfo(status);
            return;
        }

        //does the user have read only permissions to this checklist
        if (pdi.HasPermission(BaseMstr.AppUser, k_CHECKLIST_PERMISSION.ReadOnly))
        {
            ddlChecklistState.Enabled = false;
            btnSaveCL.Enabled = false;
            ddlChecklistState.Enabled = false;
            tbProcedureDate.Enabled = false;
            calProcedureDate.Enabled = false;
            ucProcedureTime.Enabled = false;
        }
        else
        {
            ddlChecklistState.Enabled = (ChecklistStateID == k_CHECKLIST_STATE_ID.Open) ? true : false;
            btnSaveCL.Enabled = true;
            btnTIU.Enabled = true;

            ddlChecklistState.Enabled = true;
            tbProcedureDate.Enabled = true;
            calProcedureDate.Enabled = true;
            ucProcedureTime.Enabled = true;

            //do not allow the user to edit a closed checklist.
            if (ChecklistStateID == k_CHECKLIST_STATE_ID.Closed)
            {
                ddlChecklistState.Enabled = false;
                tbProcedureDate.Enabled = false;
                calProcedureDate.Enabled = false;
                ucProcedureTime.Enabled = false;
                btnTIU.Enabled = false;
                //btnSaveCL.Enabled = false;

            }
        }

        EnableTIU();

        //is the user allowed to close this checklist
        if (!pdi.HasPermission(BaseMstr.AppUser, k_CHECKLIST_PERMISSION.Closeable))
        {
            ddlChecklistState.Enabled = false;
        }

        //does the user have permission to write a note for this checklist
        if (bHasNoteTitle
            && !pdi.HasPermission(BaseMstr.AppUser, k_CHECKLIST_PERMISSION.TIUNote))
        {
            btnTIU.Enabled = false;
        }
    }

    /// <summary>
    /// override
    /// US:878
    /// saves the checklist
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CPatChecklistData pcld = new CPatChecklistData(BaseMstr.BaseData);
        CPatChecklistDataItem di = null;
        CStatus status = pcld.GetPatChecklistDI(Convert.ToInt64(ddlPatChecklist.SelectedValue), out di);
        if (!status.Status)
        {
            return status;
        }

        di.ChecklistStateID = (k_CHECKLIST_STATE_ID)Convert.ToInt64(ddlChecklistState.SelectedValue);
        di.ProcedureDate = CDataUtils.GetDate(
            tbProcedureDate.Text,
            ucProcedureTime.HH,
            ucProcedureTime.MM,
            ucProcedureTime.SS);

        status = pcld.UpdatePatChecklist(di);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// override
    /// validates the checklist's fields
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CPatChecklistData pcld = new CPatChecklistData(BaseMstr.BaseData);
        CPatChecklistDataItem di = null;
        CStatus status = pcld.GetPatChecklistDI(Convert.ToInt64(ddlPatChecklist.SelectedValue), out di);
        if (!status.Status)
        {
            plistStatus.AddInputParameter("ERROR_SP_CURCHECKLIST", Resources.ErrorMessages.ERROR_SP_CURCHECKLIST);
            return status;
        }

        if (ChecklistStateID != di.ChecklistStateID
            && (di.ChecklistStateID == k_CHECKLIST_STATE_ID.Cancelled || di.ChecklistStateID == k_CHECKLIST_STATE_ID.Closed))
        {
            plistStatus.AddInputParameter("ERROR_SP_CHECKLISTSTATE", Resources.ErrorMessages.ERROR_SP_CHECKLISTSTATE);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            return status;
        }

        DateTime dtProceduteDate = CDataUtils.GetDate(
            tbProcedureDate.Text,
            ucProcedureTime.HH,
            ucProcedureTime.MM,
            ucProcedureTime.SS);

        if (dtProceduteDate != CDataUtils.GetNullDate() && dtProceduteDate < DateTime.Now)
        {
            plistStatus.AddInputParameter("ERROR_PROCEDURE_DATE", Resources.ErrorMessages.ERROR_PROCEDURE_DATE);
        }

        if (plistStatus.Count > 0)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        return status;
    }

    protected void OnClickCollapseFields(object sender, EventArgs e)
    {
        ShowMPE();
        pnlFields.Visible = !pnlFields.Visible;
        if (pnlFields.Visible)
        {
            btnCollapseFields.Text = "-";
            ucPatCLItems.Height = 342;
        }
        else
        {
            btnCollapseFields.Text = "+";
            ucPatCLItems.Height = 420;
        }
    }
}
