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

public partial class ve_ucDecisionState : CAppUserControl
{
    private long DecisionStateID
    {
        get
        {
            object obj = ViewState[ClientID + "DecisionStateID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "DecisionStateID"] = value; }
    }

    private string DefaultLabel
    {
        get
        {
            object obj = ViewState[ClientID + "DefaultLabel"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "DefaultLabel"] = value; }
    }

    private DataTable DecisionStates
    {
        get
        {
            object obj = ViewState[ClientID + "DecisionStates"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "DecisionStates"] = value; }
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
    /// method
    /// load the decision states into the grid view
    /// </summary>
    /// <returns></returns>
    private CStatus LoadDecisionStates()
    {
        //get the data
        DataSet ds = null;
        CDecisionStateData dsd = new CDecisionStateData(BaseMstr.BaseData);
        CStatus status = dsd.GetDecisionStateDS(k_ACTIVE_ID.All, out ds);
        if (!status.Status)
        {
            return status;
        }

        DecisionStates = ds.Tables[0];
        gvDecisionStates.DataSource = DecisionStates;
        gvDecisionStates.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// event
    /// all loading is done at this point, re select the 
    /// gridview rows and enable/disable buttons
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        GridViewRow gvr = gvDecisionStates.SelectedRow;
        if (gvr != null)
        {
            Label lblDefault = (Label)gvr.FindControl("lblDefault");
            if (lblDefault != null)
            {
                btnPopupDSEdit.Enabled = (lblDefault.Text != DefaultLabel) ? true : false;
            }
        }
        else if (DecisionStateID < 1)
        {
            btnPopupDSEdit.Enabled = false;
        }
    }

    /// <summary>
    /// event
    /// sets required properties on child controls
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucDecisionStateEdit.BaseMstr = BaseMstr;
        ucDecisionStateEdit.MPE = mpeDSEdit;
        ucDecisionStateEdit.GView = gvDecisionStates;

        RegisterJavaScript();

        Page.LoadComplete += new EventHandler(Page_LoadComplete);
    }

    /// <summary>
    /// override
    /// loads the decision states into the grid view
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        CStatus status = LoadDecisionStates();
        if (!status.Status)
        {
            return status;
        }

        CTrueFalseData TrueFalse = new CTrueFalseData(BaseMstr.BaseData);
        CTrueFalseDataItem di = null;
        status = TrueFalse.GetTrueFalseDI((long)k_TRUE_FALSE_ID.True, out di);
        if (!status.Status)
        {
            return status;
        }

        DefaultLabel = di.DefaultLabel;

        return new CStatus();
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
    /// event
    /// a new decision state was inserted
    /// </summary>
    /// <param name="args"></param>
    protected void OnSaveDecisionState(object sender, CAppUserControlArgs e)
    {
        //reload the temporal states
        CStatus status = LoadDecisionStates();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        //select the item we just updated
        DecisionStateID = Convert.ToInt64(e.EventData);
        CGridView.SetSelectedRow(gvDecisionStates, e.EventData);
        CGridView.SetSelectedLinkButtonForeColor(gvDecisionStates, "lnkSelect", Color.White);
    }

    /// <summary>
    /// enter a new decision state 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickNew(object sender, EventArgs e)
    {
        CStatus status = ucDecisionStateEdit.LoadControl(k_EDIT_MODE.INSERT);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucDecisionStateEdit.ShowMPE();
    }

    /// <summary>
    /// edit a decision state
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickEdit(object sender, EventArgs e)
    {
        if (DecisionStateID < 1)
        {
            return;
        }

        //pass the basemaster and popupextender to the contol 
        //and load it in "new" mode
        ucDecisionStateEdit.LongID = DecisionStateID;
        CStatus status = ucDecisionStateEdit.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucDecisionStateEdit.ShowMPE();
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and reselects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvDecisionStates.DataSource = DecisionStates;
        gvDecisionStates.DataBind();

        CGridView.SetSelectedRow(gvDecisionStates, DecisionStateID);
        CGridView.SetSelectedLinkButtonForeColor(gvDecisionStates, "lnkSelect", Color.White);
    }

    /// <summary>
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedDS(object sender, EventArgs e)
    {
        ShowMPE();

        foreach (GridViewRow gvr in gvDecisionStates.Rows)
        {
            LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
            if (lnkSelect == null)
            {
                return;
            }

            lnkSelect.ForeColor = Color.Blue;
        }

        DataKey dk = gvDecisionStates.SelectedDataKey;
        if (dk != null)
        {
            DecisionStateID = Convert.ToInt64(dk.Value);
            CGridView.SetSelectedLinkButtonForeColor(gvDecisionStates, "lnkSelect", Color.White);
        }
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundDS(Object sender, GridViewRowEventArgs e)
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
        Label lblDefinition = (Label)gvr.FindControl("lblDefinition");
        Label lblActive = (Label)gvr.FindControl("lblActive");
        Label lblDefault = (Label)gvr.FindControl("lblDefault");

        if (lnkSelect == null
            || lblDefinition == null
            || lblActive == null
            || lblDefault == null)
        {
            return;
        }

        lnkSelect.Text = dr["ds_label"].ToString();
        lblDefinition.Text = dr["ds_definition_label"].ToString();
        lblActive.Text = dr["is_active_label"].ToString();
        lblDefault.Text = dr["is_default_label"].ToString();
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
    protected void OnSortingDS(object sender, GridViewSortEventArgs e)
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

        DataView dv = DecisionStates.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        DecisionStates = dv.ToTable();

        RebindAndSelect();
    }
}