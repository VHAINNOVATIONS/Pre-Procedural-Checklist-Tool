using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ce_ucSaveAs : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _SaveAs;
    /// <summary>
    /// property
    /// adds/removes event handlers to the save event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> SaveAs
    {
        add { _SaveAs += new EventHandler<CAppUserControlArgs>(value); }
        remove { _SaveAs -= value; }
    }

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
    /// override
    /// loads the dialog's controls with the checklist label of the specified checklist
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        Title = "Copy Checklist";
        txtAs.Focus();

        //get the current checklist name and show it
        CChecklistDataItem di = new CChecklistDataItem();
        CChecklistData dta = new CChecklistData(BaseMstr.BaseData);
        CStatus status = dta.GetCheckListDI(ChecklistID, out di);
        if(!status.Status)
        {
            return status;
        }

        lblTarget.Text = di.ChecklistLabel;

        return new CStatus();
    }

    /// <summary>
    /// override
    /// saves the user's input
    /// raises an event with the new checklist id
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        //save 
        long lNewChecklistID = -1;
        CChecklistData data = new CChecklistData(BaseMstr.BaseData);
        CStatus status = data.SaveAs(
            ChecklistID,
            txtAs.Text,
            out lNewChecklistID);

        if (!status.Status)
        {
            return status;
        }

        //cache the new id
        LongID = lNewChecklistID;

        if (_SaveAs != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
            k_EVENT.INSERT,
            k_STATUS_CODE.Success,
            string.Empty,
            lNewChecklistID.ToString());

            _SaveAs(this, args);
        }

        return new CStatus();
    }


    /// <summary>
    /// override
    /// validates the user's input
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CStatus status = new CStatus();

        //make sure the user entered a new label
        if (String.IsNullOrEmpty(txtAs.Text))
        {
            plistStatus.AddInputParameter("ERROR_CL_SAVEAS_LABEL", Resources.ErrorMessages.ERROR_CL_SAVEAS_LABEL);
            status.StatusCode = k_STATUS_CODE.Failed;
            status.Status = false;
        }

        return status;
    }

    /// <summary>
    /// event
    /// validates the user's input
    /// saves the user's input if valid
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
        if (!status.Status)
        {
            ShowStatusInfo(status.StatusCode, status.StatusComment);
            ShowMPE();
            return;
        }

        ShowParentMPE();
    }

    /// <summary>
    /// event
    /// does nothing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnucCancel_Click(object sender, EventArgs e)
    {
        ShowParentMPE();
    }
}
