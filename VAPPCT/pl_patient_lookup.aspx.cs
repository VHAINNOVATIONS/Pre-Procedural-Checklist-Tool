using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Drawing;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// code behind for the patient lookup page
/// </summary>
public partial class pl_patient_lookup : System.Web.UI.Page
{
    /// <summary>
    /// patient id property used by sp_single_patient.aspx
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
    /// page load, set the title on the masterpage top right
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucPatientLookup.BaseMstr = Master;

        if (!IsPostBack)
        {
            //set the page title
            Master.PageTitle = "Patient Lookup";
            
            //load the patient lookup control
            CStatus status = ucPatientLookup.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
                return;
            }

            gvPatients.DataSource = null;
            gvPatients.DataBind();
        }
    }

    /// <summary>
    /// fired when the patient clicks search on the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnPatientSearch(object sender, EventArgs e)
    {
        PatientID = "-1";
        btnLookup.Enabled = false;

        gvPatients.PageIndex = 0;
        gvPatients.SelectedIndex = -1;
        gvPatients.EmptyDataText = "No result(s) found.";
        gvPatients.DataSource = ucPatientLookup.PatientDataTable;
        gvPatients.DataBind();
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvPatients.DataSource = ucPatientLookup.PatientDataTable;
        gvPatients.DataBind();

        CGridView.SetSelectedRow(gvPatients, PatientID);
        CGridView.SetSelectedLinkButtonForeColor(gvPatients, "lnkSelect", Color.White);

        btnLookup.Enabled = (gvPatients.SelectedRow != null) ? true : false;
    }

    protected void OnSelIndexChangedPat(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in gvPatients.Rows)
        {
            LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
            if (lnkSelect == null)
            {
                return;
            }

            lnkSelect.ForeColor = Color.Blue;
        }

        DataKey dk = gvPatients.SelectedDataKey;
        if (dk != null)
        {
            PatientID = dk.Value.ToString();
            btnLookup.Enabled = true;
            CGridView.SetSelectedLinkButtonForeColor(gvPatients, "lnkSelect", Color.White);
        }
        else
        {
            PatientID = "-1";
            btnLookup.Enabled = false;
        }
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
        if (gvr == null)
        {
            return;
        }

        if (gvr.RowType != DataControlRowType.DataRow)
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

        LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
        Label lblFirstName = (Label)gvr.FindControl("lblFirstName");
        Label lblMI = (Label)gvr.FindControl("lblMI");
        Label lblLastFour = (Label)gvr.FindControl("lblLastFour");
        Label lblAge = (Label)gvr.FindControl("lblAge");
        Label lblSex = (Label)gvr.FindControl("lblSex");

        if (lnkSelect == null
            || lblFirstName == null
            || lblMI == null
            || lblLastFour == null
            || lblAge == null
            || lblSex == null)
        {
            return;
        }

        lnkSelect.Text = dr["LAST_NAME"].ToString();
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

        RebindAndSelect();
    }

    protected void OnFiltersCollapse(object sender, CFiltersCollapseEventArgs e)
    {
        pnlPatients.Height = (e.Collapsed) ? 484 : 198;
    }
}
