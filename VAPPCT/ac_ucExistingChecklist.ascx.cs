using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ac_ucExistingChecklist : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _Continue;
    /// <summary>
    /// property
    /// adds/removes event handlers to the select event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> Continue
    {
        add { _Continue += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Continue -= value; }
    }

    /// <summary>
    /// property
    /// stores a the checklist id to check
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

    public string PatientIDs
    {
        get
        {
            object obj = ViewState[ClientID + "PatientIDs"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "PatientIDs"] = value; }
    }

    private string SelectedPatientIDs
    {
        get
        {
            object obj = ViewState[ClientID + "SelectedPatientIDs"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "SelectedPatientIDs"] = value; }
    }

    /// <summary>
    /// property
    /// cache the complete dataset for sorting
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
    /// US:838
    /// event
    /// initializes checklist selector dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterJavaScript();

        if (!IsPostBack)
        {
            Title = "Patient(s) With Checklist Open";
            SelectedPatientIDs = ",";
        }
    }

    /// <summary>
    /// US:838
    /// override
    /// sets focus
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        //get the data
        DataSet ds = null;
        CPatChecklistData cld = new CPatChecklistData(BaseMstr.BaseData);
        CStatus status = cld.GetPatCLByCLIDCLSTATEDS(
            ChecklistID,
            (long)k_CHECKLIST_STATE_ID.Open,
            PatientIDs,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        PatientDataTable = ds.Tables[0];
        gvExistingCL.DataSource = PatientDataTable;
        gvExistingCL.DataBind();

        if (PatientDataTable.Rows.Count > 0)
        {
            ShowMPE();
        }
        else
        {
            StoreAndContinue();
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// override
    /// does nothing
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// US:838
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    private void StoreAndContinue()
    {
        List<string> lstIDs = CGridView.GetUncheckedRowsList(
            gvExistingCL,
            "chkSelect");

        foreach (string strID in lstIDs)
        {
            PatientIDs = PatientIDs.Replace("," + strID + ",", ",");
        }

        if (_Continue != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                "1");

            _Continue(this, args);
        }
    }

    /// <summary>
    /// US:838
    /// event
    /// fires the continue event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnExistCLContinue_Click(object sender, EventArgs e)
    {
        ShowParentMPE();
        StoreAndContinue();
    }

   /// <summary>
    /// US:838
   /// event
   /// closes the dialog
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
    protected void btnExistCLCancel_Click(object sender, EventArgs e)
    {
        ShowParentMPE();
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
        Label lblAssignmentDate = (Label)gvr.FindControl("lblAssignmentDate");
        Label lblProcedureDate = (Label)gvr.FindControl("lblProcedureDate");

        if (chkSelect == null
            || lblFirstName == null
            || lblMI == null
            || lblLastFour == null
            || lblAssignmentDate == null
            || lblProcedureDate == null)
        {
            return;
        }

        chkSelect.Text = dr["LAST_NAME"].ToString();
        chkSelect.Checked = true;
        SelectedPatientIDs += dr["patient_id"].ToString() + ",";
        lblFirstName.Text = dr["FIRST_NAME"].ToString();
        lblMI.Text = dr["MIDDLE_INITIAL"].ToString();
        lblLastFour.Text = dr["SSN_LAST_4"].ToString();
        lblAssignmentDate.Text = dr["assignment_date"].ToString();
        lblProcedureDate.Text = dr["procedure_date"].ToString();
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and rechecks any checked items
    /// </summary>
    private void RebindAndCheck()
    {
        gvExistingCL.DataSource = PatientDataTable;
        gvExistingCL.DataBind();

        CGridView.SetCheckedRows(
            gvExistingCL,
            SelectedPatientIDs,
            "chkSelect");
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
        SelectedPatientIDs = CGridView.GetCheckedRows(
            gvExistingCL,
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

        DataView dv = PatientDataTable.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        PatientDataTable = dv.ToTable();

        RebindAndCheck();
    }
}
