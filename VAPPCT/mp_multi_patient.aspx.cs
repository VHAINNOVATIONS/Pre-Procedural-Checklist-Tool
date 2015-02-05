using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

//our data and ui libraries
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// code behind for the multi patient page
/// </summary>
public partial class mp_multi_patient : System.Web.UI.Page
{
    // data key indexes
    private const int nPatientIDDataKeyIndex = 0;
    private const int nChecklistIDDataKeyIndex = 1;
    private const int nChecklistLabelDataKeyIndex = 2;
    private const int nPatCLIDDataKeyIndex = 3;

    // Worst Case Row State Cell Position in Gridview
    private const int nWorstRowStateIDIndex = 1;

    // column index of the column containing the link button control
    // used for selecting a row
    private const int nLinkButtonColumn = 1;

    // First Item Group Column
    private const int nDefaultColumnCount = 5;

    // number of threads - 1 used on refresh
    private const int nRefreshThreads = 0;

    // number of threads - 1 used on logic
    private const int nLogicThreads = 14;

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
    /// method
    /// loads the service drop down list
    /// </summary>
    /// <returns></returns>
    private CStatus LoadServiceDDL()
    {
        CStatus status = CService.LoadServiceDDL(
                Master.BaseData,
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

    protected k_MULTI_PAT_THREAD_TYPE ThreadType
    {
        get
        {
            object obj = ViewState[ClientID + "ThreadType"];
            return (obj != null) ? (k_MULTI_PAT_THREAD_TYPE)obj : k_MULTI_PAT_THREAD_TYPE.Unknown;
        }
        set { ViewState[ClientID + "ThreadType"] = value; }
    }

    protected bool ProcessingCancelled
    {
        get
        {
            object obj = ViewState[ClientID + "ProcessingCancelled"];
            return (obj != null) ? Convert.ToBoolean(obj) : true;
        }
        set { ViewState[ClientID + "ProcessingCancelled"] = value; }
    }

    //caches whether we should enable the apply new version button
    protected bool EnableVersionUpdate
    {
        get
        {
            object obj = ViewState[ClientID + "EnableVersionUpdate"];
            return (obj != null) ? Convert.ToBoolean(obj) : false;
        }
        set { ViewState[ClientID + "EnableVersionUpdate"] = value; }
    }

    /// <summary>
    /// patient ids for threading off running logic...
    /// </summary>
    public string PatCLIDs
    {
        get
        {
            object obj = ViewState[ClientID + "PatCLIDs"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }

        set { ViewState[ClientID + "PatCLIDs"] = value; }
    }

    /// <summary>
    /// get the patient checklist ids that need to be threaded
    /// </summary>
    /// <param name="dtFrom"></param>
    /// <param name="dtTo"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lChecklistStatusID"></param>
    public CStatus GetPatCLIDs(
        DateTime dtFrom,
        DateTime dtTo,
        long lChecklistID,
        long lChecklistStatusID,
        long lServiceID)
    {
        PatCLIDs = string.Empty;

        CPatientData p = new CPatientData(Master.BaseData);
        CStatus status = null;
        DataSet ds = null;
        switch (ThreadType)
        {
            case k_MULTI_PAT_THREAD_TYPE.Refresh:
                status = p.GetMultiPatientSearchDS(
                    dtFrom,
                    dtTo,
                    lChecklistID,
                    lChecklistStatusID,
                    lServiceID,
                    out ds);
                break;
            case k_MULTI_PAT_THREAD_TYPE.Logic:
                status = p.GetMultiPatientPatCLIDSearchDS(
                    dtFrom,
                    dtTo,
                    lChecklistID,
                    lChecklistStatusID,
                    lServiceID,
                    out ds);
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        if (!status.Status)
        {
            return status;
        }

        string strPatCLIDs = string.Empty;
        CDataUtils.GetDSDelimitedData(
            ds,
            "PAT_CL_ID",
            ",",
            out strPatCLIDs);

        PatCLIDs = "," + strPatCLIDs;

        return new CStatus();
    }

    /// <summary>
    /// property
    /// gets/sets a event start date for the page
    /// </summary>
    protected DateTime MPEventStartDate
    {
        get
        {
            object obj = ViewState[ClientID + "MPEventStartDate"];
            if (obj != null)
            {
                return Convert.ToDateTime(obj);
            }
            else
            {
                return CDataUtils.GetNullDate();
            }
        }
        set { ViewState[ClientID + "MPEventStartDate"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a event end date for the page
    /// </summary>
    protected DateTime MPEventEndDate
    {
        get
        {
            object obj = ViewState[ClientID + "MPEventEndDate"];
            if (obj != null)
            {
                return Convert.ToDateTime(obj);
            }
            else
            {
                return CDataUtils.GetNullDate();
            }
        }
        set { ViewState[ClientID + "MPEventEndDate"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a checklist id for the page
    /// </summary>
    protected long MPChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "MPChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "MPChecklistID"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a checklist status id for the page
    /// </summary>
    protected long MPChecklistStatusID
    {
        get
        {
            object obj = ViewState[ClientID + "MPChecklistStatusID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "MPChecklistStatusID"] = value; }
    }

    /// <summary>
    /// property
    /// gets/sets a checklist service id for the page
    /// </summary>
    protected long MPChecklistServiceID
    {
        get
        {
            object obj = ViewState[ClientID + "MPChecklistServiceID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "MPChecklistServiceID"] = value; }
    }

    /// <summary>
    /// update the checklist version
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUpdateCLVersion(object sender, EventArgs e)
    {
        ucUpdateChecklistVersion.EventStartDate = MPEventStartDate;
        ucUpdateChecklistVersion.EventEndDate = MPEventEndDate;
        ucUpdateChecklistVersion.ChecklistID = MPChecklistID;
        ucUpdateChecklistVersion.ChecklistStatusID = MPChecklistStatusID;

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
        LoadGridView();
    }

    /// <summary>
    /// loads the gridview after we are done updating the checklists
    /// </summary>
    protected CStatus LoadGridView()
    {
        //done processing the logic so now load the gridview
        //set the date from and date to
        DateTime dtFrom = CDataUtils.GetNullDate();
        DateTime dtTo = CDataUtils.GetNullDate();
        if (chkFilterByEvent.Checked)
        {
            dtFrom = CDataUtils.GetDate(txtFromDate.Text);
            dtTo = CDataUtils.GetDate(txtToDate.Text);
        }
        MPEventStartDate = dtFrom;
        MPEventEndDate = dtTo;

        //checklist id
        long lChecklistID = -1;
        if (chkChecklist.Checked)
        {
            lChecklistID = ChecklistID;
        }
        MPChecklistID = lChecklistID;

        //set the checklist Status
        long lChecklistStatusID = -1;
        if (chkChecklistStatus.Checked
            && ddlChecklistStatus.SelectedItem != null
            && ddlChecklistStatus.SelectedValue != "-1")
        {
            lChecklistStatusID = CDataUtils.ToLong(ddlChecklistStatus.SelectedValue);
        }
        MPChecklistStatusID = lChecklistStatusID;

        long lChecklistServiceID = -1;
        if (chkFilterByCLService.Checked
            && ddlFilterByService.SelectedItem != null
            && ddlFilterByService.SelectedValue != "-1")
        {
            lChecklistServiceID = CDataUtils.ToLong(ddlFilterByService.SelectedValue);
        }
        MPChecklistServiceID = lChecklistServiceID;

        CStatus status = GetPatients(dtFrom, 
                                     dtTo, 
                                     lChecklistID, 
                                     lChecklistStatusID,
                                     lChecklistServiceID);
        if (!status.Status)
        {
            return status;
        }

        AddColumns();

        gvMultiPatientView.EmptyDataText = "No result(s) found.";
        gvMultiPatientView.DataSource = MultiPatients;
        gvMultiPatientView.DataBind();

        //now that the grid is loaded check for new versions...

        //get all the patients for the mulitpatient
        string strCLIDs = string.Empty;
        string strPatIDs = string.Empty;

        CPatientData pat = new CPatientData(Master.BaseData);
        DataSet dsMultiPatientSearch = null;
        status = pat.GetMultiPatientSearchDS(
            MPEventStartDate,
            MPEventEndDate,
            MPChecklistID,
            MPChecklistStatusID,
            MPChecklistServiceID,
            out dsMultiPatientSearch);
        if (!status.Status)
        {
            return status;
        }

        //patient ids
        CDataUtils.GetDSDelimitedData(
            dsMultiPatientSearch,
            "PATIENT_ID",
            ",",
            out strPatIDs);
        strPatIDs = "," + strPatIDs;

        //pat cl ids
        CDataUtils.GetDSDelimitedData(
            dsMultiPatientSearch,
            "CHECKLIST_ID",
            ",",
            out strCLIDs);

        strCLIDs = "," + strCLIDs;

        CPatChecklistData dta = new CPatChecklistData(Master.BaseData);
        DataSet dsCL = null;
        status = dta.GetOutOfDatePatCLDS(
            MPEventStartDate,
            MPEventEndDate,
            MPChecklistID,
            MPChecklistStatusID,
            strPatIDs,
            strCLIDs,
            out dsCL);
        if (!status.Status)
        {
            return status;
        }

        btnUpdateCLVersion.Enabled = (!CDataUtils.IsEmpty(dsCL)) ? true : false;
        //EnableVersionUpdate = (!CDataUtils.IsEmpty(dsCL)) ? true : false;
        upLookup.Update();

        return new CStatus();
    }

    /// <summary>
    /// US:2483 method
    /// runs logic for all patient checklists that match the patient filters
    /// </summary>
    /// <returns></returns>
    public CStatus ThreadMultiPatient()
    {
        int nThreads;
        switch (ThreadType)
        {
            case k_MULTI_PAT_THREAD_TYPE.Refresh:
                nThreads = nRefreshThreads;
                break;
            case k_MULTI_PAT_THREAD_TYPE.Logic:
                nThreads = nLogicThreads;
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        string[] astrPatCLIDs = PatCLIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        int nRemaining = astrPatCLIDs.Count();

        ucCancelProcessing.Count = nRemaining.ToString();
        ucCancelProcessing.ShowMPE();

        string strPatCLIDs = ",";
        for (int i = 0; i < nRemaining; i++)
        {
            strPatCLIDs += astrPatCLIDs[i] + ",";

            if (i >= nThreads)
            {
                break;
            }
        }

        PatCLIDs = PatCLIDs.Replace(strPatCLIDs, ",");

        CStatus status = null;
        switch (ThreadType)
        {
            case k_MULTI_PAT_THREAD_TYPE.Refresh:
                CMPRefreshThreadPool tpRefresh = new CMPRefreshThreadPool(Master.BaseData);
                status = tpRefresh.ThreadRefresh(strPatCLIDs);
                break;
            case k_MULTI_PAT_THREAD_TYPE.Logic:
                CPMRunlogicThreadPool tpLogic = new CPMRunlogicThreadPool(Master.BaseData);
                status = tpLogic.ThreadRunLogic(strPatCLIDs);
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        if (!status.Status)
        {
            ucCancelProcessing.ShowParentMPE();
            PatCLIDs = string.Empty;
            return status;
        }

        //set the isProcessing flag
        if (nRemaining - nThreads > 0
            && !ProcessingCancelled)
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "Processing",
                "document.getElementById('" + btnProcess.ClientID + "').click();",
                true);
            return new CStatus();
        }
        else
        {
            ucCancelProcessing.ShowParentMPE();
            PatCLIDs = string.Empty;
            return LoadGridView();
        }
    }

    /// <summary>
    /// property
    /// stores a MultiPatient dataset for the user control
    /// </summary>
    protected DataTable MultiPatients
    {
        get
        {
            object obj = ViewState[ClientID + "MultiPatients"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "MultiPatients"] = value; }
    }

    /// <summary>
    /// property
    /// stores all the states in the database
    /// </summary>
    protected DataTable States
    {
        get
        {
            object obj = ViewState[ClientID + "States"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "States"] = value; }
    }

    /// <summary>
    /// US:896
    /// US:897
    /// property
    /// stores the worst state id for each column in the grid view
    /// key = column index
    /// value = state id
    /// </summary>
    protected Dictionary<int, long> WorstStateIDInColumn
    {
        get
        {
            object obj = ViewState[ClientID + "WorstStateIDInColumn"];
            if (obj == null)
            {
                Dictionary<int, long> dic = new Dictionary<int, long>();
                WorstStateIDInColumn = dic;
                return dic;
            }
            return obj as Dictionary<int, long>;
        }
        set { ViewState[ClientID + "WorstStateIDInColumn"] = value; }
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
    /// US:894
    /// US:899
    /// Abstract validate user input method
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public CStatus ValidateUserInput(out CParameterList plistStatus)
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
                    status.StatusComment = Resources.ErrorMessages.ERROR_DATE_RANGE;
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
                        status.StatusComment = Resources.ErrorMessages.ERROR_DATE_RANGE;
                    }
                }
            }
        }

        return status;
    }

    /// <summary>
    /// method
    /// gets result set of all the patients that match the patient filters from the database
    /// </summary>
    /// <param name="dtFrom"></param>
    /// <param name="dtTo"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lChecklistStatusID"></param>
    /// <returns></returns>
    protected CStatus GetPatients(
        DateTime dtFrom,
        DateTime dtTo,
        long lChecklistID,
        long lChecklistStatusID,
        long lChecklistServiceID)
    {
        CPatientData pat = new CPatientData(Master.BaseData);
        DataSet dsMultiPatientSearch = null;
        CStatus status = pat.GetMultiPatientSearchDS(
            dtFrom,
            dtTo,
            lChecklistID,
            lChecklistStatusID,
            lChecklistServiceID,
            out dsMultiPatientSearch);
        if (!status.Status)
        {
            return status;
        }

        // add url value for each row based on the state id
        dsMultiPatientSearch.Tables[0].Columns.Add("WR_ITEM_GROUP_URL", Type.GetType("System.String"));

        foreach (DataRow dr in dsMultiPatientSearch.Tables[0].Rows)
        {
            switch (Convert.ToInt64(dr["WR_ITEM_GROUP_STATE_ID"]))
            {
                case (long)k_STATE_ID.Bad:
                    dr["WR_ITEM_GROUP_URL"] = Resources.Images.STATE_BAD;
                    break;
                case (long)k_STATE_ID.Good:
                    dr["WR_ITEM_GROUP_URL"] = Resources.Images.STATE_GOOD;
                    break;
                default:
                    dr["WR_ITEM_GROUP_URL"] = Resources.Images.STATE_UNKNOWN;
                    break;
            }
        }

        MultiPatients = dsMultiPatientSearch.Tables[0];

        return new CStatus();
    }

    /// <summary>
    /// US:896
    /// US:897
    /// method
    /// dynamically adds the columns from the result set
    /// </summary>
    protected void AddColumns()
    {
        gvMultiPatientView.SelectedIndex = -1;
        gvMultiPatientView.Columns.Clear();
        WorstStateIDInColumn.Clear();

        int nGridviewWidth = 0;

        //Add Columns To Gridview
        foreach (DataColumn col in MultiPatients.Columns)
        {
            // create columns
            string strColumn = col.ToString();
            if (strColumn == "WR_ITEM_GROUP_STATE_ID")
            {
                ImageField imageField = new ImageField();
                imageField.AccessibleHeaderText = "Row State";
                imageField.AlternateText = "Row State for";
                imageField.DataImageUrlField = "WR_ITEM_GROUP_URL";
                imageField.HeaderStyle.CssClass = "gv_pleasewait";
                imageField.HeaderStyle.Width = 40;
                imageField.HeaderText = imageField.AccessibleHeaderText;
                imageField.ItemStyle.Width = 40;
                imageField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                imageField.SortExpression = strColumn;

                gvMultiPatientView.Columns.Insert(0, imageField);

                nGridviewWidth += Convert.ToInt32(imageField.HeaderStyle.Width.Value);
            }
            else if (strColumn == "LAST_NAME")
            {
                ButtonField buttonField = new ButtonField();
                buttonField.AccessibleHeaderText = strColumn.Replace("_", " ");
                buttonField.ButtonType = ButtonType.Link;
                buttonField.CommandName = "Select";
                buttonField.ControlStyle.CssClass = "gv_truncated";
                buttonField.ControlStyle.Width = 150;
                buttonField.DataTextField = strColumn;
                buttonField.HeaderStyle.CssClass = "gv_pleasewait";
                buttonField.HeaderText = buttonField.AccessibleHeaderText;
                buttonField.HeaderStyle.Width = 150;
                buttonField.ItemStyle.CssClass = "gv_pleasewait";
                buttonField.ItemStyle.Width = 150;
                buttonField.SortExpression = strColumn;

                gvMultiPatientView.Columns.Add(buttonField);

                nGridviewWidth += Convert.ToInt32(buttonField.HeaderStyle.Width.Value);
            }
            else if (strColumn != "CHECKLIST_ID"
                && strColumn != "PAT_CL_ID"
                && strColumn != "PATIENT_ID"
                && strColumn != "WR_ITEM_GROUP_URL")
            {
                BoundField boundfield = new BoundField();
                boundfield.AccessibleHeaderText = strColumn.Replace("_", " ");
                boundfield.DataField = strColumn;
                boundfield.HeaderStyle.CssClass = "gv_pleasewait";
                boundfield.HeaderText = boundfield.AccessibleHeaderText;
                boundfield.ItemStyle.CssClass = "gv_truncated";
                boundfield.ItemStyle.Wrap = false;
                boundfield.SortExpression = strColumn;

                switch (strColumn)
                {
                    case "FIRST_NAME":
                    case "CHECKLIST_LABEL":
                        boundfield.HeaderStyle.Width = 100;
                        boundfield.ItemStyle.Width = 100;
                        break;
                    case "LAST_4":
                        boundfield.HeaderStyle.Width = 40;
                        boundfield.ItemStyle.Width = 40;
                        break;
                    default:
                        boundfield.HeaderStyle.Width = 75;
                        boundfield.ItemStyle.Width = 75;
                        break;
                }

                gvMultiPatientView.Columns.Add(boundfield);

                nGridviewWidth += Convert.ToInt32(boundfield.HeaderStyle.Width.Value);
            }
        }

        gvMultiPatientView.Width = nGridviewWidth + gvMultiPatientView.Columns.Count + 1;
    }

    /// <summary>
    /// US:891
    /// US:894
    /// Load Gridview Header images for Item Groups
    /// </summary>
    protected void LoadStateImages()
    {
        GridViewRow HeaderRow = gvMultiPatientView.HeaderRow;
        if (HeaderRow == null)
        {
            return;
        }

        if (WorstStateIDInColumn.Count == 0)
        {
            return;
        }

        for (int i = nDefaultColumnCount; i < HeaderRow.Cells.Count; i++)
        {
            string strPath = Resources.Images.STATE_UNKNOWN;
            switch (WorstStateIDInColumn[i])
            {
                case (long)k_STATE_ID.Bad:
                    strPath = Resources.Images.STATE_BAD;
                    break;

                case (long)k_STATE_ID.Good:
                    strPath = Resources.Images.STATE_GOOD;
                    break;
            }

            TableCell HeaderCell = HeaderRow.Cells[i];
            if (HeaderCell == null)
            {
                return;
            }

            Image imgWorstState = new Image();
            imgWorstState.AlternateText = "Worst State Image for ";
            imgWorstState.ID = "WorstStateID" + i.ToString();
            imgWorstState.ImageUrl = strPath;
            imgWorstState.CssClass = "gv_header_image";

            HeaderCell.Controls.AddAt(0, imgWorstState);
        }
    }

 //   protected override void Render(HtmlTextWriter output)
 //   {
 //       btnUpdateCLVersion.Enabled = EnableVersionUpdate;
 //       
 //       //render the control
 //       base.RenderChildren(output);
  //  }

    /// <summary>
    /// US:894
    /// US:892
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //enable the apply new version button based on the cached version
        //btnUpdateCLVersion.Enabled = EnableVersionUpdate;
        
        //pass basemaster and mpe to the checklist selector
        ucChecklistSelector.BaseMstr = Master;
        ucChecklistSelector.MPE = mpeChecklistSelector;

        //pass basemaster and mpe to the Single Patient Editor
        ucSinglePatientPopup.BaseMstr = Master;
        ucSinglePatientPopup.MPE = mpeSinglePatientEditor;

        ucCancelProcessing.BaseMstr = Master;
        ucCancelProcessing.MPE = mpeCancelProcessing;

        RegisterJavaScript();

        if (!IsPostBack)
        {
            Master.PageTitle = "Multi Patient";
            LoadServiceDDL();

            CSTATData StatData = new CSTATData(Master.BaseData);
            DataSet ds = null;
            CStatus status = StatData.GetStateDS(out ds);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
                return;
            }

            States = ds.Tables[0];

            status = CSTAT.LoadChecklistStateDDL(
            Master.BaseData,
            ddlChecklistStatus);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
                return;
            }

            gvMultiPatientView.DataSource = null;
            gvMultiPatientView.DataBind();
        }

        ucUpdateChecklistVersion.BaseMstr = Master;
        ucUpdateChecklistVersion.MPE = mpeUpdateChecklistVersion;
    }

    /// <summary>
    /// US:894
    /// US:899
    /// user clicked the search button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSearch(object sender, EventArgs e)
    {
        Master.ClearStatusInfo();

        CParameterList pListStatus = null;
        CStatus status = ValidateUserInput(out pListStatus);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status.StatusCode, pListStatus);
            return;
        }

        //set the date from and date to
        DateTime dtFrom = CDataUtils.GetNullDate();
        DateTime dtTo = CDataUtils.GetNullDate();
        if (chkFilterByEvent.Checked)
        {
            dtFrom = CDataUtils.GetDate(txtFromDate.Text);
            dtTo = CDataUtils.GetDate(txtToDate.Text);
        }

        //checklist id
        long lChecklistID = -1;
        if (chkChecklist.Checked)
        {
            lChecklistID = ChecklistID;
        }

        //set the checklist Status
        long lChecklistStatusID = -1;
        if (chkChecklistStatus.Checked
            && ddlChecklistStatus.SelectedItem != null
            && ddlChecklistStatus.SelectedValue != "-1")
        {
            lChecklistStatusID = CDataUtils.ToLong(ddlChecklistStatus.SelectedValue);
        }

        ThreadType = k_MULTI_PAT_THREAD_TYPE.Logic;


        //service
        long lServiceID = -1;
        if (chkFilterByCLService.Checked)
        {
            if (ddlFilterByService.SelectedItem != null)
            {
                if (ddlFilterByService.SelectedValue != "-1")
                {
                    lServiceID = CDataUtils.ToLong(ddlFilterByService.SelectedValue);
                }
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

        //get patient checklist ids
        GetPatCLIDs(
                dtFrom,
                dtTo,
                lChecklistID,
                lChecklistStatusID,
                lServiceID);

        ProcessingCancelled = false;
        
        //thread off the multi patient search
        status = ThreadMultiPatient();

        //handle a bad status that comes back with no message
        if (!status.Status)
        {
            if (String.IsNullOrEmpty(status.StatusComment))
            {
                status.StatusComment = "An error occured while processing records!";
            }
        }

        //show the status
        Master.ShowStatusInfo(status);
    }

    /// <summary>
    /// US:894
    /// US:899
    /// user clicked the select checklist button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickChecklist(object sender, EventArgs e)
    {
        CStatus status = ucChecklistSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
        }
        else
        {
            ucChecklistSelector.ShowMPE();
        }
    }

    /// <summary>
    /// US:894
    /// US:899
    /// user selected a checklist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnChecklistSelect(object sender, CAppUserControlArgs e)
    {
        ChecklistID = Convert.ToInt32(e.EventData);

        CChecklistData cld = new CChecklistData(Master.BaseData);
        CChecklistDataItem di = new CChecklistDataItem();
        CStatus status = cld.GetCheckListDI(ChecklistID, out di);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
        }

        txtChecklist.Text = di.ChecklistLabel;
    }

    /// <summary>
    /// US:894
    /// US:899
    /// user checked/unchecked the filter by event checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnCheckChangedEvent(object sender, EventArgs e)
    {
        chkFilterByEvent.Focus();
        txtFromDate.Enabled = chkFilterByEvent.Checked;
        txtToDate.Enabled = chkFilterByEvent.Checked;
    }

    /// <summary>
    /// US:894
    /// US:899
    /// user checked/unchecked the filter by CL checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnCheckChangedChecklist(object sender, EventArgs e)
    {
        chkChecklist.Focus();
        txtChecklist.Enabled = chkChecklist.Checked;
        btnSelectChecklist.Enabled = chkChecklist.Checked;
    }

    /// <summary>
    /// US:894
    /// user checked/unchecked the filter by cl status checkbox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnCheckChangedCLStatus(object sender, EventArgs e)
    {
        chkChecklistStatus.Focus();
        ddlChecklistStatus.Enabled = chkChecklistStatus.Checked;
    }

    /// <summary>
    /// US:891
    /// US:894
    /// event
    /// refreshes the page with vista data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRefresh(object sender, EventArgs e)
    {
        //if MDWS is on, transfer data from mdws to the vappct db
        if (Master.MDWSTransfer)
        {
            //set the date from and date to
            DateTime dtFrom = CDataUtils.GetNullDate();
            DateTime dtTo = CDataUtils.GetNullDate();
            if (chkFilterByEvent.Checked)
            {
                dtFrom = CDataUtils.GetDate(txtFromDate.Text);
                dtTo = CDataUtils.GetDate(txtToDate.Text);
            }

            //checklist id
            long lChecklistID = -1;
            if (chkChecklist.Checked)
            {
                lChecklistID = ChecklistID;
            }

            //set the checklist Status
            long lChecklistStatus = -1;
            if (chkChecklistStatus.Checked
                && ddlChecklistStatus.SelectedItem != null
                && ddlChecklistStatus.SelectedValue != "-1")
            {
                lChecklistStatus = CDataUtils.ToLong(ddlChecklistStatus.SelectedValue);
            }

            //set the checklist Status
            long lChecklistServiceID = -1;
            if (chkFilterByCLService.Checked
                && ddlFilterByService.SelectedItem != null
                && ddlFilterByService.SelectedValue != "-1")
            {
                lChecklistServiceID = CDataUtils.ToLong(ddlFilterByService.SelectedValue);
            }

            ThreadType = k_MULTI_PAT_THREAD_TYPE.Refresh;

            GetPatCLIDs(
                dtFrom,
                dtTo,
                lChecklistID,
                lChecklistStatus,
                lChecklistServiceID);

            ProcessingCancelled = false;
            Master.ShowStatusInfo(ThreadMultiPatient());
        }
    }

    /// <summary>
    /// US:891
    /// US:894
    /// method
    /// loads the values from the data row into the grid view row controls
    /// </summary>
    /// <param name="gvr"></param>
    protected void LoadGridViewRowValues(GridViewRow gvr)
    {
        PopulateItemGroupValues(gvr);
    }

    /// <summary>
    /// US:891
    /// US:894
    /// Populate Item Group Data 
    /// </summary>
    /// <param name="gvr"></param>
    /// <param name="dr"></param>
    protected void PopulateItemGroupValues(GridViewRow gvr)
    {
        for (int i = nDefaultColumnCount; i < gvr.Cells.Count; i++)
        {
            TableCell Cell = gvr.Cells[i];
            if (Cell == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(Cell.Text))
            {
                continue;
            }

            long lCellValue = Convert.ToInt64(Cell.Text);
            if (lCellValue > 0)
            {
                DataRow[] draDSSelect = States.Select("STATE_ID = " + Cell.Text);

                Cell.Text = draDSSelect[0]["STATE_LABEL"].ToString();

                switch (lCellValue)
                {
                    case (long)k_STATE_ID.Bad:
                        Cell.BackColor = System.Drawing.Color.Red;
                        break;
                    case (long)k_STATE_ID.Good:
                        Cell.BackColor = System.Drawing.Color.Green;
                        break;
                    default:
                        Cell.BackColor = System.Drawing.Color.Yellow;
                        break;
                }
            }
            else
            {
                Cell.Text = "NA";
                Cell.BackColor = System.Drawing.Color.Gray;
            }

            if (!WorstStateIDInColumn.ContainsKey(i) || lCellValue > WorstStateIDInColumn[i])
            {
                WorstStateIDInColumn[i] = lCellValue;
            }
        }
    }

    /// <summary>
    /// US:891
    /// US:894
    /// event calls the loads for the grid view row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundPatient(Object sender, GridViewRowEventArgs e)
    {
        GridViewRow gvr = (GridViewRow)e.Row;
        if (gvr == null)
        {
            return;
        }

        if (gvr.RowType == DataControlRowType.DataRow)
        {
            LoadGridViewRowValues(gvr);
        }
    }

    /// <summary>
    /// US:892
    /// US:894
    /// event
    /// displays the edit checklist item component dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickEdit(object sender, EventArgs e)
    {
        DataKey dk = gvMultiPatientView.SelectedDataKey;
        if (dk == null)
        {
            return;
        }

        //Setting to pass to Single Patient View
        ucSinglePatientPopup.PatientID = dk.Values[nPatientIDDataKeyIndex].ToString();
        ucSinglePatientPopup.ChecklistID = Convert.ToInt64(dk.Values[nChecklistIDDataKeyIndex]);
        ucSinglePatientPopup.ChecklistLabel = dk.Values[nChecklistLabelDataKeyIndex].ToString();
        ucSinglePatientPopup.PatCLID = Convert.ToInt64(dk.Values[nPatCLIDDataKeyIndex]);

        CStatus status = ucSinglePatientPopup.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        ucSinglePatientPopup.ShowMPE();
    }

    protected void SetSelectedLinkButtonColor()
    {
        foreach (GridViewRow gvr in gvMultiPatientView.Rows)
        {
            LinkButton lb = (LinkButton)gvr.Cells[nLinkButtonColumn].Controls[0];
            if (lb != null)
            {
                lb.ForeColor = System.Drawing.Color.Blue;
            }
        }

        GridViewRow gvrSel = gvMultiPatientView.SelectedRow;
        if (gvrSel != null)
        {
            LinkButton lb = (LinkButton)gvrSel.Cells[nLinkButtonColumn].Controls[0];
            if (lb != null)
            {
                lb.ForeColor = System.Drawing.Color.White;
            }
        }
    }

    protected void OnSelIndexChangedPat(object sender, EventArgs e)
    {
        SetSelectedLinkButtonColor();
    }

    /// <summary>
    /// event
    /// sets the header images on every postback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnPreRenderPatients(object sender, EventArgs e)
    {
        LoadStateImages();
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        DataKey dk = gvMultiPatientView.SelectedDataKey;
        if (dk == null)
        {
            return;
        }

        gvMultiPatientView.DataSource = MultiPatients;
        gvMultiPatientView.DataBind();

        CGridView.SetSelectedRow(
            gvMultiPatientView,
            nPatCLIDDataKeyIndex,
            Convert.ToInt64(dk.Values[nPatCLIDDataKeyIndex]));

        SetSelectedLinkButtonColor();
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
    protected void OnSortingPatients(object sender, GridViewSortEventArgs e)
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

        DataView dv = MultiPatients.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        MultiPatients = dv.ToTable();

        RebindAndSelect();
    }

    protected void OnClickCollapseFilters(object sender, EventArgs e)
    {
        pnlFilters.Visible = !pnlFilters.Visible;
        if (pnlFilters.Visible)
        {
            btnCollapseFilters.Text = "-";
            pnlMultiPatientView.Height = 295;
        }
        else
        {
            btnCollapseFilters.Text = "+";
            pnlMultiPatientView.Height = 455;
        }
    }

    protected void OnCancelProcessing(object sender, CAppUserControlArgs e)
    {
        ProcessingCancelled = true;
        Master.ShowStatusInfo(k_STATUS_CODE.Success, Resources.SuccessMessages.SUCCESS_CANCEL_PROCESSING);
    }

    protected void OnClickProcess(object sender, EventArgs e)
    {
        Master.ShowStatusInfo(ThreadMultiPatient());
    }
}