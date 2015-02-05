using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ce_ucStateSelector : CAppUserControl
{
    public long ChecklistID
    {
        get { return ucTemporalStateSelector.ChecklistID; }
        set
        {
            ucTemporalStateSelector.ChecklistID = value;
            ucOutcomeStateSelector.ChecklistID = value;
            ucDecisionStateSelector.ChecklistID = value;
        }
    }

    public long ChecklistItemID
    {
        get { return ucTemporalStateSelector.ChecklistItemID; }
        set
        {
            ucTemporalStateSelector.ChecklistItemID = value;
            ucOutcomeStateSelector.ChecklistItemID = value;
            ucDecisionStateSelector.ChecklistItemID = value;
        }
    }

    /// <summary>
    /// event
    /// does nothing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucTemporalStateSelector.BaseMstr = BaseMstr;
        ucTemporalStateSelector.MPE = MPE;

        ucOutcomeStateSelector.BaseMstr = BaseMstr;
        ucOutcomeStateSelector.MPE = MPE;

        ucDecisionStateSelector.BaseMstr = BaseMstr;
        ucDecisionStateSelector.MPE = MPE;
    }

    /// <summary>
    /// override
    /// calls the grid view loads
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        // load the gridviews with static data
        CStatus status = ucTemporalStateSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            return status;
        }

        status = ucOutcomeStateSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            return status;
        }

        status = ucDecisionStateSelector.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// override
    /// makes sure the user selected a state for each type
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// override
    /// saves the user's selection for each grid view in the database
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = ucTemporalStateSelector.SaveControl();
        if (!status.Status)
        {
            return status;
        }

        status = ucOutcomeStateSelector.SaveControl();
        if (!status.Status)
        {
            return status;
        }

        status = ucDecisionStateSelector.SaveControl();
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
