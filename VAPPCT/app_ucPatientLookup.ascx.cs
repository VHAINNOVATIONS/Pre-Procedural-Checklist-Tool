using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class app_ucPatientLookup : CAppUserControl
{
    protected string OPTION_NONE = "0";
    protected string OPTION_PROVIDERS = "1";
    protected string OPTION_TEAMS = "2";
    protected string OPTION_SPECIALTIES = "3";
    protected string OPTION_CLINICS = "4";
    protected string OPTION_WARDS = "5";

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
    /// event
    /// toggles the enabled state of the service control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnCheckedChangedCLService(object sender, EventArgs e)
    {
        //ShowMPE();
        ddlFilterByService.Enabled = chkFilterByCLService.Checked;
        chkFilterByCLService.Focus();
    }

    /// <summary>
    /// property
    /// gets/sets a checklist id for the page
    /// </summary>
    protected long ChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistID"] = value; }
    }

    protected event EventHandler _Search;
    /// <summary>
    /// property
    /// adds/removes event handlers to the search event
    /// </summary>
    public event EventHandler Search
    {
        add { _Search += new EventHandler(value); }
        remove { _Search -= value; }
    }

    protected event EventHandler<CFiltersCollapseEventArgs> _Collapse;
    /// <summary>
    /// property
    /// adds/removes event handlers to the collapse event
    /// </summary>
    public event EventHandler<CFiltersCollapseEventArgs> Collapse
    {
        add { _Collapse += new EventHandler<CFiltersCollapseEventArgs>(value); }
        remove { _Collapse -= value; }
    }

    /// <summary>
    /// cache the complete dataset for paging
    /// </summary>
    public DataTable PatientDataTable
    {
        get
        {
            object obj = ViewState[ClientID + "PatientDataTable"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "PatientDataTable"] = value; }
    }

    /// <summary>
    /// event
    /// US:838
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //pass basemaster and mpe to the checklist selector
        ucChecklistSelector.BaseMstr = BaseMstr;
        ucChecklistSelector.MPE = mpeChecklistSelector;

        if (!IsPostBack)
        {
            //no checklist selected to start with
            ChecklistID = -1;
            
            //default clinic control to false
            ShowClinicApptControls(false);
        }
    }

    /// <summary>
    /// override
    /// US:838
    /// load control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        //load the checklist status dropdown list
        CStatus status = CSTAT.LoadChecklistStateDDL(BaseMstr.BaseData, ddlChecklistStatus);
        if (!status.Status)
        {
            return status;
        }

        //load the service ddl
        status = LoadServiceDDL();
        if (!status.Status)
        {
            return status;
        }

        //set the clinic date range to today
        if(String.IsNullOrEmpty(txtApptFromDate.Text))
        {
            DateTime dtNow = DateTime.Now;

            calApptFromDate.SelectedDate = dtNow;
            txtApptFromDate.Text = CDataUtils.GetDateAsString(dtNow);

            calApptToDate.SelectedDate = dtNow;
            txtApptToDate.Text = CDataUtils.GetDateAsString(dtNow);
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
    /// US:838
    /// validates the user's search filters
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CStatus status = new CStatus();
        
        //if date in either from or to make sure to is greater that from
        if (chkFilterByEvent.Checked)
        {
            if (txtFromDate.Text != String.Empty ||
                txtToDate.Text != String.Empty)
            {
                if (txtFromDate.Text == String.Empty ||
                    txtToDate.Text == String.Empty)
                {
                    plistStatus.AddInputParameter("ERROR_DATE_RANGE", Resources.ErrorMessages.ERROR_DATE_RANGE);
                    
                    status.Status = false;
                    status.StatusCode = k_STATUS_CODE.Failed;
                }
                else
                {
                    DateTime dtFrom = CDataUtils.GetDate(txtFromDate.Text);
                    DateTime dtTo = CDataUtils.GetDate(txtToDate.Text);

                    k_COMPARE nCompare = CDataUtils.CompareDates(dtFrom, dtTo);
                    if (!((nCompare == k_COMPARE.EQUALTO) || (nCompare == k_COMPARE.LESSTHAN)))
                    {
                        plistStatus.AddInputParameter("ERROR_DATE_RANGE", Resources.ErrorMessages.ERROR_DATE_RANGE);
                        status.Status = false;
                        status.StatusCode = k_STATUS_CODE.Failed;
                    }
                }
            }
        }

        //if lssn make sure 5 chars and last 4 are numbers
        if (chkLSSN.Checked)
        {
            if (txtLSSN.Text != String.Empty)
            {
                if (txtLSSN.Text.Length < 5)
                {
                    plistStatus.AddInputParameter("ERROR_LSSN", Resources.ErrorMessages.ERROR_LSSN);
                    status.Status = false;
                    status.StatusCode = k_STATUS_CODE.Failed;
                }
            }
        }

        if (!status.Status)
        {
            ShowStatusInfo(status.StatusCode, plistStatus);
        }

        return status;
    }

    /// <summary>
    /// method
    /// US:838
    /// helper to determine if the user has selected criteria 
    /// from the left side checklist options for the query
    /// </summary>
    /// <returns></returns>
    protected bool HasChecklistCriteria(string strUserID)
    {
        //has date range
        bool bHasDateRange = false;
        DateTime dtFrom = CDataUtils.GetNullDate();
        DateTime dtTo = CDataUtils.GetNullDate();
        if (chkFilterByEvent.Checked)
        {
            dtFrom = CDataUtils.GetDate(txtFromDate.Text);
            dtTo = CDataUtils.GetDate(txtToDate.Text);
        }

        if (!CDataUtils.IsDateNull(dtFrom))
        {
            if (chkFilterByEvent.Checked)
            {
                bHasDateRange = true;
            }
        }

        //has last name
        bool bHasLastName = false;
        if (!String.IsNullOrEmpty(txtLastName.Text))
        {
            bHasLastName = true;
            if (chkLastName.Checked)
            {
                bHasLastName = true;
            }
        }

        //has lssn
        bool bHasLSSN = false;
        if (!String.IsNullOrEmpty(txtLSSN.Text))
        {
            if (chkLSSN.Checked)
            {
                bHasLSSN = true;
            }
        }

        //has checklist
        bool bHasChecklist = false;
        if (ChecklistID > 0)
        {
            if (chkChecklist.Checked)
            {
                bHasChecklist = true;
            }
        }

        //has checklist status
        bool bHasChecklistStatus = false;
        if (ddlChecklistStatus.SelectedIndex > 0)
        {
            if (chkChecklistStatus.Checked)
            {
                bHasChecklistStatus = true;
            }
        }

        //has checklist service
        bool bHasChecklistService = false;
        if (ddlFilterByService.SelectedIndex > 0)
        {
            if (chkFilterByCLService.Checked)
            {
                bHasChecklistService = true;
            }
        }

        bool bHasChecklistCriteria = false;

        if (bHasDateRange ||
            bHasLastName ||
            bHasLSSN ||
            bHasChecklist ||
            bHasChecklistStatus ||
            bHasChecklistService)
        {
            bHasChecklistCriteria = true;
        }

        if (!String.IsNullOrEmpty(strUserID))
        {
            bHasChecklistCriteria = true;
        }

        return bHasChecklistCriteria;
    }

    /// <summary>
    /// method
    /// US:838
    /// gets the selected option id given the option
    /// </summary>
    /// <param name="nOption"></param>
    /// <returns></returns>
    public CStatus GetSelectedOptionID(string strOption,
                                       bool bHasChecklistCriteria,                                
                                       out string strOptionID)
    {
        CStatus status = new CStatus();
        
        strOptionID = String.Empty;
        if (!String.IsNullOrEmpty(rblOptions.SelectedValue))
        {
            if (rblOptions.SelectedValue == strOption)
            {
                //option selected
                if (!String.IsNullOrEmpty(lbOptions.SelectedValue))
                {
                    long lTeamID = CDataUtils.ToLong(lbOptions.SelectedValue);
                    if (lTeamID > 0)
                    {
                        //set the team
                        strOptionID = lbOptions.SelectedValue;
                    }
                    else
                    {
                        //if the user chose All teams then make sure they 
                        //have checklist criteria selected before running
                        //query, otherwise it takes way too long
                        if (!bHasChecklistCriteria)
                        {
                            lbOptions.SelectedIndex = -1;

                            status.Status = false;
                            status.StatusCode = k_STATUS_CODE.Failed;
                            //todo: add comment to resources
                            status.StatusComment = "Please select additional Checklist search criteria before selecting 'All'";
                            return status;
                        }
                    }
                }
            }
        }

        return status;
    }

    /// <summary>
    /// method
    /// US:838
    /// do the actual search for patients
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="strUserID"></param>
    /// <returns></returns>
    protected CStatus PatientLookup(
        Object sender,
        EventArgs e,
        string strUserID)
    {
        CParameterList pListStatus = null;
        
        //validate the user input
        CStatus status = ValidateUserInput(out pListStatus);
        if (!status.Status)
        {
            return status;
        }

        //get values for the query

        //date from and date to
        DateTime dtFrom = CDataUtils.GetNullDate();
        DateTime dtTo = CDataUtils.GetNullDate();
        if (chkFilterByEvent.Checked)
        {
            dtFrom = CDataUtils.GetDate(txtFromDate.Text);
            dtTo = CDataUtils.GetDate(txtToDate.Text);
        }

        //last name
        string strLastName = String.Empty;
        if (chkLastName.Checked)
        {
            strLastName = txtLastName.Text;
        }

        //lssn
        string strLSSN = String.Empty;
        if (chkLSSN.Checked)
        {
            strLSSN = txtLSSN.Text;
        }

        //checklist id
        long lChecklistID = -1;
        if (chkChecklist.Checked)
        {
            lChecklistID = ChecklistID;
        }

        //checklist Status
        long lChecklistStatus = -1;
        if (chkChecklistStatus.Checked)
        {
            if (ddlChecklistStatus.SelectedItem != null)
            {
                if (ddlChecklistStatus.SelectedValue != "-1")
                {
                    lChecklistStatus = CDataUtils.ToLong(ddlChecklistStatus.SelectedValue);
                }
            }
        }

        //service
        long lServiceID = -1;
        if(chkFilterByCLService.Checked)
        {
            if(ddlFilterByService.SelectedItem != null)
            {
                if(ddlFilterByService.SelectedValue != "-1")
                {
                    lServiceID = CDataUtils.ToLong(ddlFilterByService.SelectedValue);
                }
            }
        }

        //do we have left side checklist criteria for the query
        bool bHasChecklistCriteria = HasChecklistCriteria(strUserID);

        //team
        string strTeamID = String.Empty;
        status = GetSelectedOptionID(
            OPTION_TEAMS,
            bHasChecklistCriteria,
            out strTeamID);
        if (!status.Status)
        {
            return status;
        }

        //specialty
        string strSpecialtyID = String.Empty;
        status = GetSelectedOptionID(
            OPTION_SPECIALTIES,
            bHasChecklistCriteria,
            out strSpecialtyID);
        if (!status.Status)
        {
            return status;
        }

        //ward
        string strWardID = String.Empty;
        status = GetSelectedOptionID(
            OPTION_WARDS,
            bHasChecklistCriteria,
            out strWardID);
        if (!status.Status)
        {
            return status;
        }

        //clinic
        string strClinicID = String.Empty;
        status = GetSelectedOptionID(
            OPTION_CLINICS,
            bHasChecklistCriteria,
            out strClinicID);
        if (!status.Status)
        {
            return status;
        }
            
        //if team, ward or specialty are loaded then we have criteria
        //beacause the user can search for patients without cl criteria
        //for team,ward,specialty searches
        if(!String.IsNullOrEmpty(strTeamID))
        {
            bHasChecklistCriteria = true;
        }
        if(!String.IsNullOrEmpty(strWardID))
        {
            bHasChecklistCriteria = true;
        }
        if(!String.IsNullOrEmpty(strSpecialtyID))
        {
            bHasChecklistCriteria = true;
        }
        if (!String.IsNullOrEmpty(strClinicID))
        {
            bHasChecklistCriteria = true;
        }
        
        //make sure some criteria is selected before searching
        if (!bHasChecklistCriteria)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "Please select at least one checklist search criteria before searching.";
            return status;
        }

        //get a dataset matching criteria
        CPatientData pat = new CPatientData(BaseMstr.BaseData);
        DataSet ds = null;
        status = pat.GetPatientSearchDS(dtFrom,
                                        dtTo,
                                        strLastName,
                                        strLSSN,
                                        lChecklistID,
                                        lChecklistStatus,
                                        strUserID,
                                        strTeamID,
                                        strWardID,
                                        strSpecialtyID,
                                        strClinicID,
                                        lServiceID, 
                                        out ds);
        if (status.Status)
        {
            //keep a copy of the full DS for paging if needed
            PatientDataTable = ds.Tables[0];

            //raise the event
            if (_Search != null)
            {
                _Search(sender, e);
            }
        }

        return status;
    }

    /// <summary>
    /// event
    /// US:838
    /// user clicked the search button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        CStatus status = PatientLookup(sender, e, string.Empty);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            upucPatientLookup.Update();
        }

        btnSearch.Focus();
    }

    /// <summary>
    /// event
    /// US:838
    /// user clicked the select checklist button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelectChecklist_Click(object sender, EventArgs e)
    {
        CStatus status = ucChecklistSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if(!status.Status)
        {
            ShowStatusInfo(status);
        }
        else
        {
            ucChecklistSelector.ShowMPE();
        }
    }

    /// <summary>
    /// event
    /// US:838
    /// user selected a checklist item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnChecklistSelect(object sender, CAppUserControlArgs e)
    {
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        CChecklistDataItem di = null;
        CStatus status = cld.GetCheckListDI(Convert.ToInt32(e.EventData), out di);
        if (!status.Status)
        {
            ShowStatusInfo(status);
        }

        txtChecklist.Text = di.ChecklistLabel;
        ChecklistID = di.ChecklistID;
    }
    
    /// <summary>
    /// event
    /// US:838
    /// user checked/unchecked the filter by event checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkFilterByEvent_CheckedChanged(object sender, EventArgs e)
    {
        chkFilterByEvent.Focus();
        txtFromDate.Enabled = chkFilterByEvent.Checked;
        txtToDate.Enabled = chkFilterByEvent.Checked;
    }
    
    /// <summary>
    /// event
    /// US:838
    /// user checked/unchecked the filter by last name checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkLastName_CheckedChanged(object sender, EventArgs e)
    {
        chkLastName.Focus();
        txtLastName.Enabled = chkLastName.Checked;
    }

    /// <summary>
    /// event
    /// US:838
    /// user checked/unchecked the filter by lssn checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkLSSN_CheckedChanged(object sender, EventArgs e)
    {
        chkLSSN.Focus();
        txtLSSN.Enabled = chkLSSN.Checked;
    }

    /// <summary>
    /// event
    /// US:838
    /// user checked/unchecked the filter by CL checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkChecklist_CheckedChanged(object sender, EventArgs e)
    {
        chkChecklist.Focus();
        txtChecklist.Enabled = chkChecklist.Checked;
        btnSelectChecklist.Enabled = chkChecklist.Checked;
    }

    /// <summary>
    /// event
    /// US:838
    /// user checked/unchecked the filter by cl status checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void chkChecklistStatus_CheckedChanged(object sender, EventArgs e)
    {
        chkChecklistStatus.Focus();
        ddlChecklistStatus.Enabled = chkChecklistStatus.Checked;
    }

    /// <summary>
    /// event
    /// US:838
    /// handler for My Patients button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMyPatients_Click(object sender, EventArgs e)
    {
        btnMyPatients.Focus();

        //get providers patients from the query
        //get a dataset matching criteria
        DataSet dsPatients = null;
        CPatientData pat = new CPatientData(BaseMstr.BaseData);
        CStatus status = pat.GetUserPatientDS(BaseMstr.UserID, out dsPatients);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        PatientLookup(sender, e, Convert.ToString(BaseMstr.UserID)); 
    }

    /// <summary>
    /// event
    /// US:838
    /// load the options list box based on the radio option selected
    /// </summary>
    /// <param name="bClearSearch"></param>
    /// <returns></returns>
    public CStatus LoadOptionsListBox(bool bClearSearch)
    {
        CStatus status = new CStatus();
        if (!String.IsNullOrEmpty(rblOptions.SelectedValue))
        {
            //hide clinic appt controls to start with 
            ShowClinicApptControls(false);

            if (bClearSearch)
            {
                txtSearchOptions.Text = string.Empty;
            }
            //clear current options
            lbOptions.Items.Clear();

            //set enabled to start with
            lbOptions.Enabled = true;
            txtSearchOptions.Enabled = true;
            btnSearchOptions.Enabled = true;

            string strValue = rblOptions.SelectedValue;
            if (strValue == OPTION_NONE)
            {
                //0=none
                lbOptions.Items.Clear();
                lbOptions.Enabled = false;
                txtSearchOptions.Enabled = false;
                btnSearchOptions.Enabled = false;
            }
            else if (strValue == OPTION_TEAMS)
            {
                //get the dataset from our db
                DataSet dsTeams = null;
                CTeamData td = new CTeamData(BaseMstr.BaseData);
                status = td.GetTeamDS(out dsTeams);

                CListBox lb = new CListBox();
                lb.RenderDataSet(
                    dsTeams,
                    lbOptions,
                    "All",
                    "TEAM_LABEL",
                    "TEAM_ID");

            }
            else if (strValue == OPTION_SPECIALTIES)
            {
                //get the dataset from our db
                DataSet dsSpecialties = null;
                CSpecialtyData sd = new CSpecialtyData(BaseMstr.BaseData);
                status = sd.GetSpecialtyDS(out dsSpecialties);

                CListBox lb = new CListBox();
                lb.RenderDataSet(
                    dsSpecialties,
                    lbOptions,
                    "All",
                    "SPECIALTY_LABEL",
                    "SPECIALTY_ID");

            }
            else if (strValue == OPTION_CLINICS)
            {
                //show clinic appt controls
                ShowClinicApptControls(true);

                //get the dataset from our db
                DataSet dsClinics = null;
                CClinicData cd = new CClinicData(BaseMstr.BaseData);
                status = cd.GetClinicDS(out dsClinics);

                CListBox lb = new CListBox();
                lb.RenderDataSet(
                    dsClinics,
                    lbOptions,
                    "All",
                    "CLINIC_LABEL",
                    "CLINIC_ID");
            }
            else if (strValue == OPTION_WARDS)
            {
                //get the dataset from our db
                DataSet dsWards = null;
                CWardData wd = new CWardData(BaseMstr.BaseData);
                status = wd.GetWardDS(out dsWards);

                CListBox lb = new CListBox();
                lb.RenderDataSet(
                    dsWards,
                    lbOptions,
                    "All",
                    "WARD_LABEL",
                    "WARD_ID");
            }

            string strScript = string.Format("document.getElementById('{0}_{1}').focus();", rblOptions.ClientID, rblOptions.SelectedIndex);

            if (ScriptManager.GetCurrent(Page) != null && ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
                ScriptManager.RegisterStartupScript(rblOptions, typeof(RadioButtonList), rblOptions.ClientID, strScript, true);
            else
                Page.ClientScript.RegisterStartupScript(typeof(RadioButtonList), rblOptions.ClientID, strScript, true);
        }

        return status;
    }

    /// <summary>
    /// event
    /// US:838
    /// search by provider, team etc... 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelectedIndexChangedOptions(object sender, EventArgs e)
    {
        LoadOptionsListBox(true);
    }

    /// <summary>
    /// event
    /// US:838
    /// load the patient list based on the options selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbOptions_SelectedIndexChanged(object sender, EventArgs e)
    {
        CStatus status = new CStatus();

        if(rblOptions.SelectedIndex == -1)
        {
            return;
        }

        string strValue = rblOptions.SelectedValue;
        string strUserID = string.Empty;
        if (strValue == OPTION_PROVIDERS)
        {
            //get the selected team id
            long lUserID = CDataUtils.ToLong(lbOptions.SelectedValue);

            //get the dataset from our db
            if (lUserID > 0)
            {
                //get providers patients from the query
                //get a dataset matching criteria
                DataSet dsPatients = null;
                CPatientData pat = new CPatientData(BaseMstr.BaseData);
                status = pat.GetUserPatientDS(lUserID, out dsPatients);
                strUserID = Convert.ToString(lUserID); 
            }
        }
        else if (strValue == OPTION_TEAMS)
        {
            //get the selected team id
            long lTeamID = CDataUtils.ToLong(lbOptions.SelectedValue);

            //get the dataset from our db
            if (lTeamID > 0)
            {
                CTeamData td = new CTeamData(BaseMstr.BaseData);
                DataSet dsTeams = null;
                status = td.GetTeamPatientsDS(lTeamID, out dsTeams);
            }
        }
        else if (strValue == OPTION_SPECIALTIES)
        {
            //get the selected team id
            long lSpecialtyID = CDataUtils.ToLong(lbOptions.SelectedValue);

            if (lSpecialtyID > 0)
            {
                //get the dataset from our db
                CSpecialtyData sd = new CSpecialtyData(BaseMstr.BaseData);
                DataSet dsSpecialty = null;
                status = sd.GetSpecialtyPatientsDS(lSpecialtyID, out dsSpecialty);
            }
        }
        else if (strValue == OPTION_CLINICS)
        {
            //get the selected team id
            long lClinicID = CDataUtils.ToLong(lbOptions.SelectedValue);
            if (lClinicID > 0)
            {
                //get the dataset from our db
                DataSet dsClinics = null;
                CClinicData cd = new CClinicData(BaseMstr.BaseData);
                status = cd.GetClinicPatientsDS( lClinicID, 
                                                 calApptFromDate.SelectedDate.GetValueOrDefault(),
                                                 calApptToDate.SelectedDate.GetValueOrDefault(),
                                                 out dsClinics);
            }
        }
        else if (strValue == OPTION_WARDS)
        {
            //get the selected team id
            long lWardID = CDataUtils.ToLong(lbOptions.SelectedValue);
            if (lWardID > 0)
            {
                //get the dataset from our db
                DataSet dsWards = null;
                CWardData wd = new CWardData(BaseMstr.BaseData);
                status = wd.GetWardPatientsDS(lWardID, out dsWards);
            }
        }

        status = PatientLookup(sender, e, strUserID);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            upucPatientLookup.Update();
        }


        return;
    }

    /// <summary>
    /// event
    /// US:838
    /// handler for the Search button for filtering the options list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearchOptions_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(rblOptions.SelectedValue))
        {
            //clear current options
            lbOptions.Items.Clear();

            string strValue = rblOptions.SelectedValue;
            if (strValue == OPTION_PROVIDERS)
            {
                string strSearch = txtSearchOptions.Text;
                string strFirstName = string.Empty;
                string strLastName = string.Empty;

                string[] splitSearch = strSearch.Split(',');
                if (splitSearch.Length > 0)
                {
                    strLastName = splitSearch[0];
                }
                if (splitSearch.Length > 1)
                {
                    strFirstName = splitSearch[1];
                }

                if (strLastName.Length < 3)
                {
                    ShowStatusInfo(new CStatus(false, k_STATUS_CODE.Failed, "Please enter at least 3 characters to search for"));
                    return;
                }

                //1=providers
                //get the dataset from our db
                DataSet dsUsers = null;
                CUserData ud = new CUserData(BaseMstr.BaseData);
                CStatus status = ud.GetUserDS(strLastName, strFirstName, out dsUsers);

                CListBox lb = new CListBox();
                lb.RenderDataSet(dsUsers,
                                  lbOptions,
                                  "",
                                  "LAST_NAME,FIRST_NAME",
                                  "USER_ID");

            }
            else if (strValue == OPTION_TEAMS)
            {
                //load the team lb
                LoadOptionsListBox(false);
                //filter the listbox using helper
                CListBox clb = new CListBox();
                clb.FilterListBox(lbOptions, txtSearchOptions.Text);
            }
            else if (strValue == OPTION_SPECIALTIES)
            {
                //load the specialties lb
                LoadOptionsListBox(false);
                //filter the listbox using helper
                CListBox clb = new CListBox();
                clb.FilterListBox(lbOptions, txtSearchOptions.Text);
            }
            else if (strValue == OPTION_CLINICS)
            {

            }
            else if (strValue == OPTION_WARDS)
            {
                //load the wards lb
                LoadOptionsListBox(false);
                //filter the listbox using helper
                CListBox clb = new CListBox();
                clb.FilterListBox(lbOptions, txtSearchOptions.Text);
            }
            
            
        }
    }

    /// <summary>
    /// event
    /// US:838
    /// show hide clinic appoinment controls
    /// </summary>
    /// <param name="bVisible"></param>
    public void ShowClinicApptControls(bool bVisible)
    {
        lblApptFor.Visible = bVisible;
        //calApptFromDate.Visible = bVisible;
        txtApptFromDate.Visible = bVisible;
        lblApptTo.Visible = bVisible;
        //calApptToDate.Visible = bVisible;
        txtApptToDate.Visible = bVisible;

        //todo: disable the appt from and to date for now
        //txtApptFromDate.Enabled = false;
        //txtApptToDate.Enabled = false;
    }

    protected void OnClickCollapseFilters(object sender, EventArgs e)
    {
        pnlPatientLookup.Visible = !pnlPatientLookup.Visible;
        CFiltersCollapseEventArgs args = null;
        if (pnlPatientLookup.Visible)
        {
            btnCollapseFilters.Text = "-";
            args = new CFiltersCollapseEventArgs(false);
        }
        else
        {
            btnCollapseFilters.Text = "+";
            args = new CFiltersCollapseEventArgs(true);
        }

        if (_Collapse != null)
        {
            _Collapse(sender, args);
        }
    }
}
