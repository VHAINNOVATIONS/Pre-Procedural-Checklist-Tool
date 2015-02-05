using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;
using VAPPCT.Data;

public partial class ce_ucStateLogicEditor : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _Save;
    /// <summary>
    /// property
    /// adds/removes event handlers to the select event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> Save
    {
        add { _Save += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Save -= value; }
    }

    /// <summary>
    /// US:1384
    /// property
    /// stores a checklist id in viewstate
    /// retrieves a checklist id from viewstate
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

    /// <summary>
    /// US:1384
    /// property
    /// stores a checklist item id in viewstate
    /// retrieves a checklist item id from viewstate
    /// </summary>
    public long ChecklistItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistItemID"] = value; }
    }

    /// <summary>
    /// US:1384
    /// event
    /// sets the title for the dialog
    /// maintains grid view selections on postback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucStateLogicSelector.BaseMstr = BaseMstr;
        ucStateLogicSelector.MPE = MPE;
        ucStateLogicSelector.LogicAddTarget = txtItemLogic;
    }

    /// <summary>
    /// US:1384
    /// override
    /// calls the grid view loads
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        Title = "State/Logic Editor";
        btnucSave.Focus();

        CItemData item = new CItemData(BaseMstr.BaseData);
        CItemDataItem di = null;
        CStatus status = item.GetItemDI(ChecklistItemID, out di);
        if (!status.Status)
        {
            return status;
        }

        hItem.InnerText = di.ItemLabel;

        //load the gridviews with static data
        ucStateLogicSelector.ChecklistID = ChecklistID;
        ucStateLogicSelector.ChecklistItemID = ChecklistItemID;
        status = ucStateLogicSelector.LoadControl(lEditMode);
        if (!status.Status)
        {
            return status;
        }

        CChecklistItemData dta = new CChecklistItemData(BaseMstr.BaseData);
        CChecklistItemDataItem diLogic = null;
        status = dta.GetCLItemDI(ChecklistID, ChecklistItemID, out diLogic);
        if(!status.Status)
        {
            return status;
        }

        txtItemLogic.Text = diLogic.Logic;

        return new CStatus();
    }

    /// <summary>
    /// US:1384
    /// override
    /// saves the user's selection for each grid view in the database
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        //save
        CStatus status = ucStateLogicSelector.SaveControl();
        if (!status.Status)
        {
            return status;
        }

        CChecklistItemData data = new CChecklistItemData(BaseMstr.BaseData);
        status = data.UpdateChecklistItemLogic(
            ChecklistID,
            ChecklistItemID,
            txtItemLogic.Text);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1384
    /// override
    /// makes sure the user selected a state for each type
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CExpressionList expListItem = new CExpressionList(BaseMstr.BaseData, string.Empty, -1, -1, -1);
        CStatus status = expListItem.Load(txtItemLogic.Text);
        if (!status.Status)
        {
            plistStatus.AddInputParameter("ERROR_LOAD", status.StatusComment);
        }

        status = expListItem.Validate();
        if (!status.Status)
        {
            plistStatus.AddInputParameter("ERROR_VALIDATE", status.StatusComment);
        }

        return status;
    }

    /// <summary>
    /// US:1384
    /// event
    /// validates the user's selections
    /// saves the user's selections if valid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnucSave_Click(object sender, EventArgs e)
    {
        CParameterList plistStatus = null;
        CStatus status = ValidateUserInput(out plistStatus);
        if(!status.Status)
        {
            ShowStatusInfo(status.StatusCode, plistStatus);
            ShowMPE();
            return;
        }

        //save the data
        status = SaveControl();
        if(!status.Status)
        {
            ShowStatusInfo(status);
            ShowMPE();
            return;
        }
    }

    /// <summary>
    /// US:1384
    /// event
    /// does nothing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnucCancel_Click(object sender, EventArgs e)
    {
        
    }

    /// <summary>
    /// event
    /// US:902
    /// validates the logic in the logic text boxes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickValidate(object sender, EventArgs e)
    {
        ShowMPE();
        CExpressionList ExpList = new CExpressionList(BaseMstr.BaseData, "-1", -1, -1, -1);
        CStatus status = ExpList.Load(txtItemLogic.Text);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        status = ExpList.Validate();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }
    }

    protected void OnClickRestore(object sender, EventArgs e)
    {
        ShowMPE();

        txtItemLogic.Text = CExpression.DefaultTemporalLogic
            + " " + CExpression.DefaultOutcomeLogic
            + " " + CExpression.DefaultDecisionLogic;
    }
}
