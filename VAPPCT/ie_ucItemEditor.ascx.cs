using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// code behind for the item editor popup
/// </summary>
public partial class ie_ucItemEditor : CAppUserControlPopup
{
    /// <summary>
    /// US:1945 US:1883 page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Title = "Item Editor";
        }

        ucItemComponentEditor.BaseMstr = BaseMstr;
        ucItemComponentEditor.MPE = MPE;
        
        ucMapNoteTitle.BaseMstr = BaseMstr;
        ucMapNoteTitle.ParentMPE = MPE;
        ucMapNoteTitle.MPE = mpeMap;

        ucMapLabTest.BaseMstr = BaseMstr;
        ucMapLabTest.ParentMPE = MPE;
        ucMapLabTest.MPE = mpeMap;

        ucItemCollectionEditor.BaseMstr = BaseMstr;
        ucItemCollectionEditor.MPE = MPE;
    }

    /// <summary>
    /// US:852
    /// maps the selected lab test to the item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelectLabTest(object sender, CAppUserControlArgs e)
    {
        ShowMPE();

        if (string.IsNullOrEmpty(tbLabel.Text))
        {
            tbLabel.Text = ucMapLabTest.SelectedLabTestLabel;
        }

        hfMapID.Value = ucMapLabTest.SelectedLabTestID;
        tbMapID.Text = (hfMapID.Value != "-1") ? hfMapID.Value : string.Empty;

        ucItemComponentEditor.ItemTypeID = Convert.ToInt64(ddlType.SelectedValue);
        ucItemComponentEditor.Clear();

        CItemComponentDataItem di = new CItemComponentDataItem();
        di.ItemComponentLabel = "Value";
        di.SortOrder = ucItemComponentEditor.Count + 1;
        di.ActiveID = k_ACTIVE_ID.Active;

        CStatus status = ucItemComponentEditor.AddComponent(di);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucItemComponentEditor.AddEnabled = false;
    }

    /// <summary>
    /// US:1945 US:1880
    /// maps the selected note title to the item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelectNoteTitle(object sender, CAppUserControlArgs e)
    {
        ShowMPE();

        if (string.IsNullOrEmpty(tbLabel.Text))
        { 
            tbLabel.Text = ucMapNoteTitle.SelectedText;
        }

        hfMapID.Value = ucMapNoteTitle.SelectedValue;
        tbMapID.Text = (hfMapID.Value != "-1") ? hfMapID.Value : string.Empty;

        ucItemComponentEditor.ItemTypeID = Convert.ToInt64(ddlType.SelectedValue);
        ucItemComponentEditor.Clear();

        CItemComponentDataItem di = new CItemComponentDataItem();
        di.ItemComponentLabel = "Text";
        di.SortOrder = ucItemComponentEditor.Count + 1;
        di.ActiveID = k_ACTIVE_ID.Active;

        CStatus status = ucItemComponentEditor.AddComponent(di);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucItemComponentEditor.AddEnabled = false;
    }

    /// <summary>
    /// load dropdown lists
    /// </summary>
    protected void LoadItemDDLs()
    {
        CStatus status = CSTAT.LoadItemTypeDDL(BaseMstr.BaseData, ddlType);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        status = CItem.LoadItemGroupDDL(BaseMstr.BaseData, ddlGroup, k_ACTIVE_ID.Active);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
    }

    /// <summary>
    /// US:1945 US:1883
    /// save the control
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CItemDataItem item = new CItemDataItem();
        item.ItemTypeID = Convert.ToInt64(ddlType.SelectedValue);
        item.ItemGroupID = Convert.ToInt64(ddlGroup.SelectedValue);
        tbLabel.Text = tbLabel.Text.Trim();
        item.ItemLabel = tbLabel.Text;
        item.ItemDescription = tbDescription.Text;
        item.LookbackTime = Convert.ToInt64(tbLookback.Text);
        item.ActiveID = (chkActive.Checked) ? k_ACTIVE_ID.Active : k_ACTIVE_ID.Inactive;
        item.MapID = hfMapID.Value;

        CStatus status = new CStatus();
        CItemData ItemData = new CItemData(BaseMstr.BaseData);
        if (EditMode == k_EDIT_MODE.INSERT)
        {
            long lItemID = 0;
            status = ItemData.InsertItem(item, out lItemID);
            if (status.Status)
            {
                EditMode = k_EDIT_MODE.UPDATE;
                LongID = lItemID;
            }
        }
        else if (EditMode == k_EDIT_MODE.UPDATE)
        {
            item.ItemID = LongID;
            status = ItemData.UpdateItem(item);
        }

        if (status.Status)
        {
            switch (item.ItemTypeID)
            {
                case (long)k_ITEM_TYPE_ID.Laboratory:
                case (long)k_ITEM_TYPE_ID.NoteTitle:
                case (long)k_ITEM_TYPE_ID.QuestionFreeText:
                case (long)k_ITEM_TYPE_ID.QuestionSelection:
                    ucItemComponentEditor.ItemID = LongID;
                    status = ucItemComponentEditor.SaveControl();
                    break;
                case (long)k_ITEM_TYPE_ID.Collection:
                    ucItemCollectionEditor.LongID = LongID;
                    status = ucItemCollectionEditor.SaveControl();
                    break;
            }
        }

        if (status.Status)
        {
            if (ucItemComponentEditor.Count > 0 && hfMapID.Value != "-1")
            {
                ucItemComponentEditor.AddEnabled = false;
            }
        }
        
        btnCancel.Text = "Close";
        return status;
    }

    /// <summary>
    /// US:1945 US:1883
    /// load the control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;
        LoadItemDDLs();

        if (EditMode == k_EDIT_MODE.UPDATE)
        {
            CItemData ItemData = new CItemData(BaseMstr.BaseData);
            CItemDataItem iItem = null;
            CStatus status = ItemData.GetItemDI(LongID, out iItem);
            if(!status.Status)
            {
                return status;
            }

            ddlType.SelectedValue = Convert.ToString(iItem.ItemTypeID);
            tbLabel.Text = iItem.ItemLabel;
            tbDescription.Text = iItem.ItemDescription;
            tbLookback.Text = Convert.ToString(iItem.LookbackTime);
            chkActive.Checked = (iItem.ActiveID == k_ACTIVE_ID.Active) ? true : false;
            hfMapID.Value = iItem.MapID;
            tbMapID.Text = (hfMapID.Value != "-1") ? hfMapID.Value : string.Empty;

            try
            {
                ddlGroup.SelectedValue = Convert.ToString(iItem.ItemGroupID);
            }
            catch (Exception)
            {
                ddlGroup.ClearSelection();
            }

            ucItemComponentEditor.ItemID = LongID;
            ucItemComponentEditor.ItemTypeID = iItem.ItemTypeID;
            ucItemCollectionEditor.LongID = LongID;

            ShowHideUC(iItem.ItemTypeID);

            switch (iItem.ItemTypeID)
            {
                case (long)k_ITEM_TYPE_ID.Laboratory:
                case (long)k_ITEM_TYPE_ID.NoteTitle:
                case (long)k_ITEM_TYPE_ID.QuestionFreeText:
                case (long)k_ITEM_TYPE_ID.QuestionSelection:
                    status = ucItemComponentEditor.LoadControl(EditMode);
                    if (!status.Status)
                    {
                        return status;
                    }

                    if (ucItemComponentEditor.Count > 0 && hfMapID.Value != "-1")
                    {
                        ucItemComponentEditor.AddEnabled = false;
                    }
                    break;
                case (long)k_ITEM_TYPE_ID.Collection:
                    status = ucItemCollectionEditor.LoadControl(EditMode);
                    if (!status.Status)
                    {
                        return status;
                    }
                    break;
                default:
                    return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }
        }
        else if (EditMode == k_EDIT_MODE.INSERT)
        {
            chkActive.Checked = true;
            CStatus status = ucItemComponentEditor.LoadControl(EditMode);
            if (!status.Status)
            {
                return status;
            }
            ucItemComponentEditor.AddEnabled = false;

            status = ucItemCollectionEditor.LoadControl(EditMode);
            if (!status.Status)
            {
                return status;
            }
        }

        chkActive.Focus();
        return new CStatus();
    }

    /// <summary>
    /// US:1945
    /// validate user input
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();

        if (ddlType.SelectedIndex == 0)
        {
            plistStatus.AddInputParameter("ERROR_IE_ITEM_TYPE", Resources.ErrorMessages.ERROR_IE_ITEM_TYPE);
        }

        if (ddlGroup.SelectedIndex == 0)
        {
            plistStatus.AddInputParameter("ERROR_IE_ITEM_GROUP", Resources.ErrorMessages.ERROR_IE_ITEM_GROUP);
        }

        if (String.IsNullOrEmpty(tbLabel.Text))
        {
            plistStatus.AddInputParameter("ERROR_IE_IC_LABEL", Resources.ErrorMessages.ERROR_IE_IC_LABEL);
        }

        if (String.IsNullOrEmpty(tbDescription.Text))
        {
            plistStatus.AddInputParameter("ERROR_IE_IC_DESCRIPTION", Resources.ErrorMessages.ERROR_IE_IC_DESCRIPTION);
        }

        if (String.IsNullOrEmpty(tbLookback.Text) || Convert.ToInt32(tbLookback.Text) < 0)
        {
            plistStatus.AddInputParameter("ERROR_IE_IC_LOOKBACK", Resources.ErrorMessages.ERROR_IE_IC_LOOKBACK);
        }

        if (plistStatus.Count > 0)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, string.Empty);
        }

        CStatus status = null;
        switch (Convert.ToInt64(ddlType.SelectedValue))
        {
            case (long)k_ITEM_TYPE_ID.Laboratory:
            case (long)k_ITEM_TYPE_ID.NoteTitle:
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                status = ucItemComponentEditor.ValidateUserInput(out plistStatus);
                if (!status.Status)
                {
                    return status;
                }
                break;
            case (long)k_ITEM_TYPE_ID.Collection:
                status = ucItemCollectionEditor.ValidateUserInput(out plistStatus);
                if (!status.Status)
                {
                    return status;
                }
                break;
            default:
                plistStatus.AddInputParameter("TODO", "TODO");
                return new CStatus(false, k_STATUS_CODE.Failed, string.Empty);
        }

        return new CStatus();
    }

    /// <summary>
    /// user clicked the save button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSave(object sender, EventArgs e)
    {
        ShowMPE();
        CParameterList pList = null;

        CStatus status = ValidateUserInput(out pList);
        if(!status.Status)
        {
            ShowStatusInfo(status.StatusCode, pList);
            return;
        }

        status = SaveControl();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ShowStatusInfo(new CStatus());
    }

    /// <summary>
    /// US:1883 user clicked the cancel button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {
        btnCancel.Text = "Cancel";
        ddlType.ClearSelection();
        ddlType.Enabled = true;
        ddlGroup.ClearSelection();
        tbLabel.Text = string.Empty;
        tbDescription.Text = string.Empty;
        tbLookback.Text = string.Empty;
        chkActive.Checked = false;
        hfMapID.Value = "-1";
        tbMapID.Text = string.Empty;
        btnMap.Enabled = true;
        ucItemComponentEditor.ItemTypeID = -1;
        ucItemComponentEditor.Clear();
        ucItemCollectionEditor.ClearCollectionGridView();

        ShowParentMPE();
    }

    /// <summary>
    /// user selected a new type
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelChangeType(object sender, EventArgs e)
    {
        ShowMPE();

        long lItemTypeID = Convert.ToInt64(ddlType.SelectedValue);
        switch (lItemTypeID)
        {
            case (long)k_ITEM_TYPE_ID.Laboratory:
            case (long)k_ITEM_TYPE_ID.NoteTitle:
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                ucItemComponentEditor.MoveValuesFromGVtoDT();
                ucItemComponentEditor.ItemTypeID = lItemTypeID;
                ucItemComponentEditor.SourceAndBind();
                ucItemComponentEditor.AddEnabled = true;
                break;
            case (long)k_ITEM_TYPE_ID.Collection:
                CStatus status = ucItemCollectionEditor.LoadControl(k_EDIT_MODE.INITIALIZE);
                if (!status.Status)
                {
                    ShowStatusInfo(status);
                    return;
                }
                break;
            default:
                ShowStatusInfo(new CStatus(false, k_STATUS_CODE.Failed, string.Empty));
                return;
        }

        ShowHideUC(lItemTypeID);

        hfMapID.Value = "-1";
        tbMapID.Text = string.Empty;
    }

    /// <summary>
    /// US:1945 US:1883
    /// method
    /// shows/hides the component/collection editors
    /// </summary>
    /// <param name="lItemTypeID"></param>
    private void ShowHideUC(long lItemTypeID)
    {
        switch (lItemTypeID)
        {
            case (long)k_ITEM_TYPE_ID.NoteTitle:
            case (long)k_ITEM_TYPE_ID.Laboratory:
                ucItemComponentEditor.Visible = true;
                ucItemCollectionEditor.Visible = false;
                btnMap.Enabled = true;
                tbLookback.Enabled = true;
                break;
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                ucItemComponentEditor.Visible = true;
                ucItemCollectionEditor.Visible = false;
                btnMap.Enabled = false;
                tbLookback.Enabled = true;
                break;
            case (long)k_ITEM_TYPE_ID.Collection:
                ucItemComponentEditor.Visible = false;
                ucItemCollectionEditor.Visible = true;
                btnMap.Enabled = false;
                tbLookback.Text = "0";
                tbLookback.Enabled = false;
                break;
        }
    }

    /// <summary>
    /// US:1945 US:852 show the lab map dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickMap(object sender, EventArgs e)
    {
        ShowMPE();

        //start with the maps hidden
        ucMapNoteTitle.Visible = false;
        ucMapLabTest.Visible = false;

        if (ddlType.SelectedIndex < 1)
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_IE_ITEM_TYPE);
            return;
        }

        CStatus status = null;
        switch (Convert.ToInt32(ddlType.SelectedValue))
        {
            case (int)k_ITEM_TYPE_ID.NoteTitle:
                status = ucMapNoteTitle.LoadControl(k_EDIT_MODE.INITIALIZE);
                if (!status.Status)
                {
                    ShowStatusInfo(status);
                    return;
                }
                ucMapNoteTitle.ShowMPE();
                break;
            case (int)k_ITEM_TYPE_ID.Laboratory:
                // no loading necessary for lab dialog
                status = ucMapNoteTitle.LoadControl(k_EDIT_MODE.INITIALIZE);
                ucMapLabTest.ShowMPE();
                break;
        }
    }

    protected void OnClickCollapseFields(object sender, EventArgs e)
    {
        ShowMPE();
        pnlFields.Visible = !pnlFields.Visible;
        if (pnlFields.Visible)
        {
            btnCollapseFields.Text = "-";
            ucItemComponentEditor.Height = 185;
            ucItemCollectionEditor.Height = 185;
        }
        else
        {
            btnCollapseFields.Text = "+";
            ucItemComponentEditor.Height = 400;
            ucItemCollectionEditor.Height = 400;
        }
    }
}
