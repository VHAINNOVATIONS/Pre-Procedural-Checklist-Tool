using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ve_ucItemGroupsEdit : CAppUserControlPopup
{
    private event EventHandler<CAppUserControlArgs> _Save;
    /// <summary>
    /// event
    /// fires when a temporal state is saved
    /// </summary>
    public event EventHandler<CAppUserControlArgs> Save
    {
        add { _Save += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Save -= value; }
    }

    /// <summary>
    /// property to keep the origianl label we were loade with
    /// so we can check for dupes later
    /// </summary>
    public string OriginalLabel
    {
        get
        {
            object obj = ViewState[ClientID + "OriginalLabel"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "OriginalLabel"] = value; }
    }

    /// <summary>
    /// page load, set the title for the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Item Group";
    }
    
    /// <summary>
    /// load the control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        //cache the editmode for postbacks
        EditMode = lEditMode;

        //clear/reset the controls
        txtItemGroupLabel.Text = string.Empty;
        txtItemGroupLabel.Focus();
        chkItemGroupActive.Checked = false;

        //default active check to true
        if (lEditMode == k_EDIT_MODE.INSERT)
        {
            chkItemGroupActive.Checked = true;
        }
        //load data to the controls if we are in update mode
        else if (lEditMode == k_EDIT_MODE.UPDATE)
        {
            CItemGroupDataItem di;
            CItemGroupData ig = new CItemGroupData(BaseMstr.BaseData);
            CStatus status = ig.GetItemGroupDI(LongID, out di);
            if(!status.Status)
            {
                return status;
            }

            //load the controls with data
            txtItemGroupLabel.Text = di.ItemGroupLabel;
            OriginalLabel = di.ItemGroupLabel;
            chkItemGroupActive.Checked = di.IsActive;
        }

        return new CStatus();
    }

    /// <summary>
    /// save the control
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = new CStatus();
        switch (EditMode)
        {
            case k_EDIT_MODE.INSERT:
                long lItemGroupID = -1;
                status = InsertItemGroup(out lItemGroupID);
                if (status.Status)
                {
                    LongID = lItemGroupID;
                    EditMode = k_EDIT_MODE.UPDATE;
                }
                break;
            case k_EDIT_MODE.UPDATE:
                status = UpdateItemGroup(LongID);
                break;
            default:
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = Resources.ErrorMessages.ERROR_INVALID_EDITMODE;
                break;
        }

        if (!status.Status)
        {
            return status;
        }

        if (_Save != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.INSERT,
                k_STATUS_CODE.Success,
                string.Empty,
                LongID.ToString());

            _Save(this, args);
        }

        return new CStatus();
    }
    
    /// <summary>
    /// validate the user input
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CStatus status = new CStatus();

        //label
        if (txtItemGroupLabel.Text.Length < 1)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            plistStatus.AddInputParameter("ERROR_IG_LABEL", Resources.ErrorMessages.ERROR_IG_LABEL);
        }

        //active - nothing to check

        //if we are inserting make sure the row 
        //does not already esist.
        if (EditMode == k_EDIT_MODE.INSERT
            || EditMode == k_EDIT_MODE.UPDATE && txtItemGroupLabel.Text != OriginalLabel)
        {
            if (CGridView.CellValueExists(GView, 1, txtItemGroupLabel.Text))
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                plistStatus.AddInputParameter("ERROR_DATA_EXISTS", Resources.ErrorMessages.ERROR_DATA_EXISTS);
            }
        }

        return status;
    }

    /// <summary>
    /// method
    /// creates a new item group data item and loads it with the values from the dialog
    /// </summary>
    /// <returns></returns>
    private CItemGroupDataItem LoadNewDataItem()
    {
        CItemGroupDataItem di = new CItemGroupDataItem();
        di.ItemGroupID = -1;
        di.ItemGroupLabel = txtItemGroupLabel.Text;
        di.IsActive = chkItemGroupActive.Checked;
        return di;
    }

    /// <summary>
    /// helper to insert an item group
    /// </summary>
    /// <param name="lItemGroupID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    private CStatus InsertItemGroup(out long lItemGroupID)
    {
        lItemGroupID = -1;

        CItemGroupDataItem igdi = LoadNewDataItem();

        //save the data item
        CItemGroupData igd = new CItemGroupData(BaseMstr.BaseData);
        CStatus status = igd.InsertItemGroup(igdi, out lItemGroupID);
        if (status.Status)
        {
            LongID = lItemGroupID;
        }

        return status;
    }

   /// <summary>
    /// update an item group
   /// </summary>
   /// <param name="lItemGroupID"></param>
   /// <param name="lStatusCode"></param>
   /// <param name="strStatusComment"></param>
   /// <returns></returns>
    private CStatus UpdateItemGroup(long lItemGroupID)
    {
        //create a new data item and load it
        CItemGroupDataItem igdi = LoadNewDataItem();
        igdi.ItemGroupID = lItemGroupID;

        CItemGroupData igData = new CItemGroupData(BaseMstr.BaseData);
        return igData.UpdateItemGroup(igdi);
    }

    /// <summary>
    /// user pressed the save button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSave(object sender, EventArgs e)
    {
        CParameterList plistStatus = null;
        CStatus status = ValidateUserInput(out plistStatus);
        if (!status.Status)
        {
            ShowMPE();
            ShowStatusInfo(status.StatusCode, plistStatus);
            return;
        }

        status = SaveControl();
        if (!status.Status)
        {
            ShowMPE();
            ShowStatusInfo(status);
            return;
        }
    }

    /// <summary>
    /// user pressed the cancel button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {

    }
}