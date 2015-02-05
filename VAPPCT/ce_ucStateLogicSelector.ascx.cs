using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;

public partial class ce_ucStateLogicSelector : CAppUserControl
{
    private const int k_STATE_INDEX = 0;
    private const int k_LOGIC_INDEX = 1;

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

    /// <summary>
    /// property
    /// US:902
    /// gets/sets the text box that the user control will add logic to
    /// </summary>
    public TextBox LogicAddTarget
    {
        get { return ucLogicSelector.LogicAddTarget; }
        set { ucLogicSelector.LogicAddTarget = value; }
    }

    /// <summary>
    /// event
    /// sets user control properties
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucStateSelector.BaseMstr = BaseMstr;
        ucStateSelector.MPE = MPE;

        ucLogicSelector.BaseMstr = BaseMstr;
        ucLogicSelector.MPE = MPE;
    }

    /// <summary>
    /// event
    /// US:902
    /// loads the multi view for the selected catagory
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedSelector(object sender, EventArgs e)
    {
        ShowMPE();
        mvStateLogicSelector.ActiveViewIndex = rblSelector.SelectedIndex;
        switch (rblSelector.SelectedIndex)
        {
            case k_STATE_INDEX:
                LoadStateView();
                break;
            case k_LOGIC_INDEX:
                LoadLogicView();
                break;
        }

        string strScript = string.Format("document.getElementById('{0}_{1}').focus();", rblSelector.ClientID, rblSelector.SelectedIndex);

        if (ScriptManager.GetCurrent(Page) != null && ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            ScriptManager.RegisterStartupScript(rblSelector, typeof(RadioButtonList), rblSelector.ClientID, strScript, true);
        else
            Page.ClientScript.RegisterStartupScript(typeof(RadioButtonList), rblSelector.ClientID, strScript, true);
    }

    /// <summary>
    /// method
    /// US:902
    /// loads the state view in the multiview
    /// </summary>
    /// <returns></returns>
    protected CStatus LoadStateView()
    {
        //load the gridviews with static data
        ucStateSelector.ChecklistID = ChecklistID;
        ucStateSelector.ChecklistItemID = ChecklistItemID;
        CStatus status = ucStateSelector.LoadControl(EditMode);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// event
    /// US:902
    /// loads the logic view in the multiview
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected CStatus LoadLogicView()
    {
        ucLogicSelector.ChecklistID = ChecklistID;
        ucLogicSelector.ChecklistItemID = ChecklistItemID;
        CStatus status = ucLogicSelector.LoadControl(EditMode);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// override
    /// initializes the control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;
        rblSelector.SelectedIndex = k_STATE_INDEX;
        mvStateLogicSelector.ActiveViewIndex = k_STATE_INDEX;

        CStatus status = LoadStateView();
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// saves the selected states
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = ucStateSelector.SaveControl();
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// event
    /// does nothing
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }
}
