using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ac_assign_checklist : System.Web.UI.Page
{
    // number of threads - 1 used
    private const int nAssignThreads = 4;

    protected bool ProcessingCancelled
    {
        get
        {
            object obj = ViewState[ClientID + "ProcessingCancelled"];
            return (obj != null) ? Convert.ToBoolean(obj) : true;
        }
        set { ViewState[ClientID + "ProcessingCancelled"] = value; }
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
    /// event
    /// US:838 US:2484 
    /// sets page title
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucPatientLookup.BaseMstr = Master;

        ucChecklistSelector.BaseMstr = Master;
        ucChecklistSelector.MPE = mpeChecklistSelector;

        ucExistingChecklist.BaseMstr = Master;
        ucExistingChecklist.MPE = mpeExistingChecklist;

        ucCancelProcessing.BaseMstr = Master;
        ucCancelProcessing.MPE = mpeCancelProcessing;

        RegisterJavaScript();

        if (!IsPostBack)
        {
            Master.PageTitle = "Assign Checklist";
            gvPatients.DataSource = null;
            gvPatients.DataBind();

            CStatus status = ucPatientLookup.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
            }
        }
    }
   
    /// <summary>
    /// property
    /// gets/sets the selected patient ids
    /// </summary>
    public string ACPatientIDs
    {
        get
        {
            object obj = ViewState[ClientID + "ACPIDs"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "ACPIDs"] = value; }
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

    /// <summary>
    /// property
    /// gets/sets the selected patient ids
    /// </summary>
    public string PatientIDs
    {
        get
        {
            object obj = ViewState[ClientID + "PIDs"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "PIDs"] = value; }
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
    /// US:2484 processes 5 assign checklists at a time...
    /// </summary>
    /// <returns></returns>
    public CStatus ThreadAssignChecklists()
    {
        //process 5 at a time
        string[] astrPatientIDs = ACPatientIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        int nRemaining = astrPatientIDs.Count();

        ucCancelProcessing.Count = nRemaining.ToString();
        ucCancelProcessing.ShowMPE();

        string strPatIDs = ",";
        for (int i = 0; i < nRemaining; i++)
        {
            strPatIDs += astrPatientIDs[i] + ",";

            // grab the first five
            if (i >= nAssignThreads)
            {
                break;
            }
        }

        ACPatientIDs = ACPatientIDs.Replace(strPatIDs, ",");

        //now threading off the calls to assign the checklists
        CAssignChecklistThreadPool pool = new CAssignChecklistThreadPool(Master.BaseData);
        CStatus status = pool.ThreadAssignChecklist(strPatIDs, ChecklistID);
        if (!status.Status)
        {
            ucCancelProcessing.ShowParentMPE();
            ProcessingCancelled = true;
            ACPatientIDs = string.Empty;
            return status;
        }

        //set the isProcessing flag
        if (nRemaining - nAssignThreads > 0
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
            ProcessingCancelled = true;
            ACPatientIDs = string.Empty;
            return new CStatus(true, k_STATUS_CODE.Success, Resources.SuccessMessages.SUCCESS_AC_ASSIGNMENT);
        }
    }
        
    /// <summary>
    /// event
    /// US:838
    /// displays the checklist selector dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelectChecklist(object sender, EventArgs e)
    {
        ucChecklistSelector.ActiveChecklistsOnly = true;
        CStatus status = ucChecklistSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        ucChecklistSelector.ShowMPE();
    }

    /// <summary>
    /// event
    /// US:838
    /// captures the selected checklist id and displays the selected checklist's label
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnChecklistSelect(object sender, CAppUserControlArgs e)
    {
        ChecklistID = Convert.ToInt32(e.EventData);
        if (ChecklistID < 1)
        {
            Master.ShowStatusInfo(k_STATUS_CODE.Failed, "TODO");
            return;
        }

        CChecklistData cld = new CChecklistData(Master.BaseData);
        CChecklistDataItem di = new CChecklistDataItem();
        CStatus status = cld.GetCheckListDI(ChecklistID, out di);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        tbChecklist.Text = di.ChecklistLabel;
    }

    /// <summary>
    /// event
    /// US:838
    /// assigns the selected checklist to the selected patients
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnAssignChecklist(object sender, EventArgs e)
    {
        if (ChecklistID < 1)
        {
            Master.ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_AC_CLID);
            return;
        }

        PatientIDs = CGridView.GetCheckedRows(
            gvPatients,
            "chkSelect");

        string[] astrPatientIDs = PatientIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (astrPatientIDs.Count() < 1)
        {
            Master.ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_AC_NONESEL);
            return;
        }

        ucExistingChecklist.ChecklistID = ChecklistID;
        ucExistingChecklist.PatientIDs = PatientIDs;
        CStatus status = ucExistingChecklist.LoadControl(k_EDIT_MODE.READ_ONLY);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }
    }

    /// <summary>
    /// event
    /// US:838
    /// displays the existing checklist dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnContinueExisting(object sender, CAppUserControlArgs e)
    {
        ACPatientIDs = ucExistingChecklist.PatientIDs;
        ProcessingCancelled = false;
        Master.ShowStatusInfo(ThreadAssignChecklists());
    }

    /// <summary>
    /// event
    /// US:838
    /// captures the search results and displays them to the user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnPatientSearch(object sender, EventArgs e)
    {
        btnCheckAll.Text = "Check All";
        PatientIDs = string.Empty;
        gvPatients.PageIndex = 0;
        gvPatients.SelectedIndex = -1;
        gvPatients.EmptyDataText = "No result(s) found.";
        gvPatients.DataSource = ucPatientLookup.PatientDataTable;
        gvPatients.DataBind();
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and rechecks any checked items
    /// </summary>
    private void RebindAndCheck()
    {
        gvPatients.DataSource = ucPatientLookup.PatientDataTable;
        gvPatients.DataBind();

        CGridView.SetCheckedRows(
            gvPatients,
            PatientIDs,
            "chkSelect");
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundPat(Object sender, GridViewRowEventArgs e)
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

        CheckBox chkSelect = (CheckBox)gvr.FindControl("chkSelect");
        Label lblFirstName = (Label)gvr.FindControl("lblFirstName");
        Label lblMI = (Label)gvr.FindControl("lblMI");
        Label lblLastFour = (Label)gvr.FindControl("lblLastFour");
        Label lblAge = (Label)gvr.FindControl("lblAge");
        Label lblSex = (Label)gvr.FindControl("lblSex");

        if (chkSelect == null
            || lblFirstName == null
            || lblMI == null
            || lblLastFour == null
            || lblAge == null
            || lblSex == null)
        {
            return;
        }

        chkSelect.Text = dr["LAST_NAME"].ToString();
        lblFirstName.Text = dr["FIRST_NAME"].ToString();
        lblMI.Text = dr["MIDDLE_INITIAL"].ToString();
        lblLastFour.Text = dr["SSN_LAST_4"].ToString();
        lblAge.Text = dr["PATIENT_AGE"].ToString();
        lblSex.Text = dr["SEX_ABBREVIATION"].ToString();
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
    protected void OnSortingPat(object sender, GridViewSortEventArgs e)
    {
        PatientIDs = CGridView.GetCheckedRows(
            gvPatients,
            "chkSelect");

        if (SortExpression == e.SortExpression)
        {
            SortDirection = (SortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
        }
        else
        {
            SortExpression = e.SortExpression;
            SortDirection = SortDirection.Ascending;
        }

        DataView dv = ucPatientLookup.PatientDataTable.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        ucPatientLookup.PatientDataTable = dv.ToTable();

        RebindAndCheck();
    }

    protected void OnFiltersCollapse(object sender, CFiltersCollapseEventArgs e)
    {
        pnlPatients.Height = (e.Collapsed) ? 421 : 135;
    }

    protected void OnClickCheckAll(object sender, EventArgs e)
    {
        bool bChecked;
        if (btnCheckAll.Text == "Check All")
        {
            bChecked = true;
            btnCheckAll.Text = "Uncheck All";
        }
        else
        {
            bChecked = false;
            btnCheckAll.Text = "Check All";
        }

        foreach (GridViewRow gvr in gvPatients.Rows)
        {
            CheckBox cb = gvr.FindControl("chkSelect") as CheckBox;
            if (cb == null)
            {
                Master.ShowStatusInfo(new CStatus(false, k_STATUS_CODE.Failed, "TODO"));
                return;
            }

            cb.Checked = bChecked;
        }
    }

    protected void OnCancelProcessing(object sender, CAppUserControlArgs e)
    {
        ProcessingCancelled = true;
        Master.ShowStatusInfo(k_STATUS_CODE.Success, Resources.SuccessMessages.SUCCESS_CANCEL_PROCESSING);
    }

    protected void OnClickProcess(object sender, EventArgs e)
    {
        Master.ShowStatusInfo(ThreadAssignChecklists());
    }
}
