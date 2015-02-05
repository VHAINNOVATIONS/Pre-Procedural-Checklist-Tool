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

public partial class ve_ucTemporalState : CAppUserControl
{
    private long TemporalStateID
    {
        get
        {
            object obj = ViewState[ClientID + "TemporalStateID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "TemporalStateID"] = value; }
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

    private DataTable TemporalStates
    {
        get
        {
            object obj = ViewState[ClientID + "TemporalStates"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "TemporalStates"] = value; }
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
    /// load the temporal state list 
    /// </summary>
    /// <returns></returns>
    private CStatus LoadTemporalStates()
    {
        //get the data
        DataSet ds = null;
        CTemporalStateData tsd = new CTemporalStateData(BaseMstr.BaseData);
        CStatus status = tsd.GetTemporalStateDS((long)k_ACTIVE_ID.All, out ds);
        if (!status.Status)
        {
            return status;
        }

        TemporalStates = ds.Tables[0];
        gvTemporalStates.DataSource = TemporalStates;
        gvTemporalStates.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// event
    /// enables/diables the edit button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        GridViewRow gvr = gvTemporalStates.SelectedRow;
        if (gvr != null)
        {
            Label lblDefault = (Label)gvr.FindControl("lblDefault");
            if (lblDefault != null)
            {
                btnPopupTSEdit.Enabled = (lblDefault.Text != DefaultLabel) ? true : false;
            }
        }
        else if (TemporalStateID < 1)
        {
            btnPopupTSEdit.Enabled = false;
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
        ucTemporalStateEdit.BaseMstr = BaseMstr;
        ucTemporalStateEdit.MPE = mpeTSEdit;
        ucTemporalStateEdit.GView = gvTemporalStates;

        RegisterJavaScript();

        Page.LoadComplete += new EventHandler(Page_LoadComplete);
    }

    /// <summary>
    /// override
    /// loads the temporal states into the grid view
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        CStatus status = LoadTemporalStates();
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
    /// a temporal state was updated
    /// </summary>
    /// <param name="args"></param>
    protected void OnSaveTemporalSave(object sender, CAppUserControlArgs e)
    {
        //reload the temporal states
        CStatus status = LoadTemporalStates();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        //select the item we just updated
        TemporalStateID = Convert.ToInt64(e.EventData);
        CGridView.SetSelectedRow(gvTemporalStates, e.EventData);
        CGridView.SetSelectedLinkButtonForeColor(gvTemporalStates, "lnkSelect", Color.White);
    }

    /// <summary>
    /// edit a temporal state
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickEdit(object sender, EventArgs e)
    {
        if (TemporalStateID < 1)
        {
            return;
        }

        //pass the basemaster and popupextender to the contol 
        //and load it in "new" mode
        ucTemporalStateEdit.LongID = TemporalStateID;
        CStatus status = ucTemporalStateEdit.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucTemporalStateEdit.ShowMPE();
    }

    /// <summary>
    /// insert a new temporal state
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickNew(object sender, EventArgs e)
    {
        CStatus status = ucTemporalStateEdit.LoadControl(k_EDIT_MODE.INSERT);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucTemporalStateEdit.ShowMPE();
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and selects any selected items
    /// </summary>
    private void RebindAndSelect()
    {
        gvTemporalStates.DataSource = TemporalStates;
        gvTemporalStates.DataBind();

        CGridView.SetSelectedRow(gvTemporalStates, TemporalStateID);
        CGridView.SetSelectedLinkButtonForeColor(gvTemporalStates, "lnkSelect", Color.White);
    }

    /// <summary>
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedTS(object sender, EventArgs e)
    {
        ShowMPE();

        foreach (GridViewRow gvr in gvTemporalStates.Rows)
        {
            LinkButton lnkSelect = (LinkButton)gvr.FindControl("lnkSelect");
            if (lnkSelect == null)
            {
                return;
            }

            lnkSelect.ForeColor = Color.Blue;
        }

        DataKey dk = gvTemporalStates.SelectedDataKey;
        if (dk != null)
        {
            TemporalStateID = Convert.ToInt64(dk.Value);
            CGridView.SetSelectedLinkButtonForeColor(gvTemporalStates, "lnkSelect", Color.White);
        }
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundTS(Object sender, GridViewRowEventArgs e)
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

        lnkSelect.Text = dr["ts_label"].ToString();
        lblDefinition.Text = dr["ts_definition_label"].ToString();
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
    protected void OnSortingTS(object sender, GridViewSortEventArgs e)
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

        DataView dv = TemporalStates.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        TemporalStates = dv.ToTable();

        RebindAndSelect();
    }
}