using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// user control used to map a lab test to a checklist item
/// </summary>
public partial class ie_ucMapLabTest : CAppUserControlPopup
{
    private const int LabTestLabelColIndex = 0;

    protected event EventHandler<CAppUserControlArgs> _SelectLabTest;
    /// <summary>
    /// US:1880 property
    /// adds/removes event handlers to the select event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> SelectLabTest
    {
        add { _SelectLabTest += new EventHandler<CAppUserControlArgs>(value); }
        remove { _SelectLabTest -= value; }
    }

    /// <summary>
    /// property
    /// US:852 stores the data table of the search results
    /// </summary>
    protected DataTable Labs
    {
        get
        {
            object obj = ViewState[ClientID + "Labs"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "Labs"] = value; }
    }

    /// <summary>
    /// property
    /// stores the lab test id of the selected grid view row
    /// </summary>
    public string SelectedLabTestID
    {
        get
        {
            object obj = ViewState[ClientID + "LabTestID"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "LabTestID"] = value; }
    }

    /// <summary>
    /// property
    /// stores the lab test label of the selected grid view row
    /// </summary>
    public string SelectedLabTestLabel
    {
        get
        {
            object obj = ViewState[ClientID + "LabTestLabel"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "LabTestLabel"] = value; }
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
    /// US:852 page load for map lab test dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterJavaScript();

        if (!IsPostBack)
        {
            //Set the title
            Title = "Map Lab Test";
        }
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvLabTests.DataSource = Labs;
        gvLabTests.DataBind();

        CGridView.SetSelectedRow(gvLabTests, SelectedLabTestID);
        CGridView.SetSelectedLinkButtonForeColor(gvLabTests, "lnkSelect", Color.White);
    }

    /// <summary>
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedLab(object sender, EventArgs e)
    {
        ShowMPE();

        foreach (GridViewRow gvr in gvLabTests.Rows)
        {
            LinkButton lb = (LinkButton)gvr.FindControl("lnkSelect");
            if (lb == null)
            {
                return;
            }

            lb.ForeColor = Color.Blue;
        }

        DataKey dkSelected = gvLabTests.SelectedDataKey;
        if (dkSelected == null)
        {
            return;
        }

        SelectedLabTestID = dkSelected.Value.ToString();

        GridViewRow gvrSelected = gvLabTests.SelectedRow;
        if (gvrSelected == null)
        {
            return;
        }

        LinkButton lnkSelect = (LinkButton)gvrSelected.FindControl("lnkSelect");
        if (lnkSelect == null)
        {
            return;
        }

        SelectedLabTestLabel = lnkSelect.Text;
        lnkSelect.ForeColor = Color.White;
        btnSelect.Enabled = true;
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundLab(Object sender, GridViewRowEventArgs e)
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

        LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
        Label lblHiRef = (Label)gvr.FindControl("lblHiRef");
        Label lblLoRef = (Label)gvr.FindControl("lblLoRef");
        Label lblRefRange = (Label)gvr.FindControl("lblRefRange");
        Label lblUnits = (Label)gvr.FindControl("lblUnits");
        Label lblDescription = (Label)gvr.FindControl("lblDescription");

        if (lnkSelect == null
            || lblHiRef == null
            || lblLoRef == null
            || lblRefRange == null
            || lblUnits == null
            || lblDescription == null)
        {
            return;
        }

        lnkSelect.Text = dr["lab_test_name"].ToString();
        lblHiRef.Text = dr["lab_test_hiref"].ToString();
        lblLoRef.Text = dr["lab_test_loref"].ToString();
        lblRefRange.Text = dr["lab_test_refrange"].ToString();
        lblUnits.Text = dr["lab_test_units"].ToString();
        lblDescription.Text = dr["lab_test_description"].ToString();
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
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
    /// does nothing
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        //throw new NotImplementedException();

        CGridView.SetSelectedRow(gvLabTests, SelectedLabTestID);
        CGridView.SetSelectedLinkButtonForeColor(gvLabTests, "lnkSelect", Color.White);

        CStatus status = new CStatus();
        return status;
    }

    /// <summary>
    /// US:852
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSearch(object sender, EventArgs e)
    {
        ShowMPE();

        //make sure the suer entered search criteria
        if (txtSearchOptions.Text.Length < 1)
        {
            //todo: add error message to string table
            ShowStatusInfo(k_STATUS_CODE.Failed, "Please enter search criteria.");
            return;
        }

        //get the tests based on the search criteria
        DataSet dsLabTests = null;
        CLabData ld = new CLabData(BaseMstr.BaseData);
        CStatus status = ld.GetLabTestDS(txtSearchOptions.Text, out dsLabTests);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        Labs = dsLabTests.Tables[0];
        gvLabTests.DataSource = Labs;
        gvLabTests.DataBind();

        gvLabTests.SelectedIndex = -1;
        btnSelect.Enabled = false;
    }

    /// <summary>
    /// US:852 fires an event when the lab test is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSelect(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedLabTestID))
        {
            ShowMPE();
            return;
        }

        if (_SelectLabTest != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                SelectedLabTestID);

            _SelectLabTest(this, args);
        }
    }

    /// <summary>
    /// US:852 user cancelled the map dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {
        ShowParentMPE();
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
    protected void OnSortingLab(object sender, GridViewSortEventArgs e)
    {
        ShowMPE();
        if (SortExpression == e.SortExpression)
        {
            SortDirection = (SortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
        }
        else
        {
            SortExpression = e.SortExpression;
            SortDirection = SortDirection.Ascending;
        }

        DataView dv = Labs.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        Labs = dv.ToTable();

        RebindAndSelect();
    }
}
