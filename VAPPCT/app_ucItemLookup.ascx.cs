using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.Data;

public partial class app_ucItemLookup : CAppUserControl
{
    private event EventHandler<EventArgs> _Search;
    /// <summary>
    /// event
    /// fires when an item is selected
    /// </summary>
    public event EventHandler<EventArgs> Search
    {
        add { _Search += new EventHandler<EventArgs>(value); }
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
    public DataTable ItemDataTable
    {
        get
        {
            object obj = ViewState[ClientID + "ItemDataTable"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "ItemDataTable"] = value; }
    }

    public k_ACTIVE_ID Activefilter
    {
        get
        {
            object obj = ViewState[ClientID + "Activefilter"];
            return (obj != null) ? (k_ACTIVE_ID)obj : k_ACTIVE_ID.All;
        }
        set { ViewState[ClientID + "Activefilter"] = value; }
    }

    private CStatus LoadFilterDDLs()
    {
        CStatus status = CSTAT.LoadItemTypeDDL(BaseMstr.BaseData, ddlFilterType);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return status;
        }

        status = CItem.LoadItemGroupDDL(BaseMstr.BaseData, ddlFilterByGroup);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return status;
        }

        return new CStatus();
    }

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        return LoadFilterDDLs();
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    protected void OnClickSearch(object sender, EventArgs e)
    {
        btnSearch.Focus();

        //get the values for the query
        string strFilterLabel = (chkFilterByName.Checked) ? txtFilterByName.Text : string.Empty;
        long lFilterType = (chkFilterType.Checked) ? CDataUtils.ToLong(ddlFilterType.SelectedValue) : -1;
        long lFilterGroup = (chkFilterByGroup.Checked) ? CDataUtils.ToLong(ddlFilterByGroup.SelectedValue) : -1;

        //get the data
        DataSet ds = null;
        CItemData ItemData = new CItemData(BaseMstr.BaseData);
        CStatus status = ItemData.GetItemDS(
            strFilterLabel,
            lFilterType,
            lFilterGroup,
            (long)Activefilter,
            out ds);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ItemDataTable = ds.Tables[0];

        if (_Search != null)
        {
            _Search(this, new EventArgs());
        }
    }

    protected void OnCheckedChangedName(object sender, EventArgs e)
    {
        ShowMPE();
        txtFilterByName.Enabled = chkFilterByName.Checked;
        chkFilterByName.Focus();
    }

    protected void OnCheckedChangedType(object sender, EventArgs e)
    {
        ShowMPE();
        ddlFilterType.Enabled = chkFilterType.Checked;
        chkFilterType.Focus();
    }

    protected void OnCheckedChangedGroup(object sender, EventArgs e)
    {
        ShowMPE();
        ddlFilterByGroup.Enabled = chkFilterByGroup.Checked;
        chkFilterByGroup.Focus();
    }

    protected void OnClickCollapseFilters(object sender, EventArgs e)
    {
        ShowMPE();
        pnlFilters.Visible = !pnlFilters.Visible;
        CFiltersCollapseEventArgs args = null;
        if (pnlFilters.Visible)
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
