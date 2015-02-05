using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Text;
using VAPPCT.UI;
using VAPPCT.DA;

public partial class ce_ucOutcomeStateSelector : CAppUserControl
{
    public long ChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistID"] = value; }
    }

    public long ChecklistItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistItemID"] = value; }
    }

    private string OutcomeStateIDs
    {
        get
        {
            object obj = ViewState[ClientID + "OutcomeStateIDs"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "OutcomeStateIDs"] = value; }
    }

    private DataTable OutcomeStates
    {
        get
        {
            object obj = ViewState[ClientID + "OutcomeStates"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "OutcomeStates"] = value; }
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

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        //get the data
        DataSet ds = null;
        COutcomeStateData osd = new COutcomeStateData(BaseMstr.BaseData);
        CStatus status = osd.GetOutcomeStateDS((long)k_ACTIVE_ID.Active, out ds);
        if (!status.Status)
        {
            return status;
        }

        OutcomeStates = ds.Tables[0];
        gvOS.DataSource = OutcomeStates;
        gvOS.DataBind();

        //get the cli data and check the checkboxes 
        CChecklistItemData itemData = new CChecklistItemData(BaseMstr.BaseData);
        DataSet dsOS = null;
        status = itemData.GetOutcomeStateDS(ChecklistID, ChecklistItemID, out dsOS);
        if (!status.Status)
        {
            return status;
        }

        string strOSIDs = ",";
        foreach (DataRow dr in dsOS.Tables[0].Rows)
        {
            strOSIDs += dr["os_id"].ToString() + ",";
        }

        OutcomeStateIDs = strOSIDs;
        CGridView.SetCheckedRows(
            gvOS,
            OutcomeStateIDs,
            "chkSelect");

        return new CStatus();
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    public override CStatus SaveControl()
    {
        //get the checked outcome states
        string strOSIDs = CGridView.GetCheckedRows(
            gvOS,
            "chkSelect");

        long lOSCount = strOSIDs.Split(',').Count();

        //save the outcome states
        CChecklistItemData itm = new CChecklistItemData(BaseMstr.BaseData);
        CStatus status = itm.SaveOutcomeStates(
            ChecklistID,
            ChecklistItemID,
            strOSIDs,
            lOSCount);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            ShowMPE();
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:931
    /// rebinds the page's gridview and rechecks any checked items
    /// </summary>
    private void RebindAndCheck()
    {
        gvOS.DataSource = OutcomeStates;
        gvOS.DataBind();

        CGridView.SetCheckedRows(
            gvOS,
            OutcomeStateIDs,
            "chkSelect");
    }

    /// <summary>
    /// event
    /// sets focus on the selected row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedOS(object sender, EventArgs e)
    {
        ShowMPE();

        GridViewRow gvr = gvOS.SelectedRow;
        if (gvr != null)
        {
            CheckBox chkSelect = (CheckBox)gvr.FindControl("chkSelect");
            if (chkSelect != null)
            {
                chkSelect.Focus();
            }
        }
    }

    /// <summary>
    /// event
    /// sets the text for the link button in the grid view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnRowDataBoundOS(Object sender, GridViewRowEventArgs e)
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
        Label lblDefinition = (Label)gvr.FindControl("lblDefinition");

        if (chkSelect == null || lblDefinition == null)
        {
            return;
        }

        chkSelect.Text = dr["os_label"].ToString();
        chkSelect.Enabled = (Convert.ToInt64(dr["is_default"]) == (long)k_TRUE_FALSE_ID.True) ? false : true;
        lblDefinition.Text = dr["os_definition_label"].ToString();
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
    protected void OnSortingOS(object sender, GridViewSortEventArgs e)
    {
        ShowMPE();
        OutcomeStateIDs = CGridView.GetCheckedRows(
            gvOS,
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

        DataView dv = OutcomeStates.DefaultView;
        dv.Sort = SortExpression + ((SortDirection == SortDirection.Ascending) ? " ASC" : " DESC");
        OutcomeStates = dv.ToTable();

        RebindAndCheck();
    }
}