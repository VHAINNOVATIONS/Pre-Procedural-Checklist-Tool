using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ve_ucServiceEdit : CAppUserControlPopup
{
    private event EventHandler<CAppUserControlArgs> _Save;
    /// <summary>
    /// event
    /// fires when a service is saved
    /// </summary>
    public event EventHandler<CAppUserControlArgs> Save
    {
        add { _Save += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Save -= value; }
    }

    /// <summary>
    /// property
    /// stores the original text of the service
    /// </summary>
    public string OriginalLabel
    {
        get { return ViewState[ClientID + "OriginalLabel"].ToString(); }
        private set { ViewState[ClientID + "OriginalLabel"] = value; }
    }

    /// <summary>
    /// event
    /// sets the dialog title
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Service";
    }

    /// <summary>
    /// override
    /// clears the dialog and loads the service specified with the
    /// long id property if the dialog is loaded in update mode
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        txtService.Text = string.Empty;
        txtService.Focus();
        chkActive.Checked = false;

        if (EditMode == k_EDIT_MODE.INSERT)
        {
            chkActive.Checked = true;
        }
        else if (EditMode == k_EDIT_MODE.UPDATE)
        {
            if (LongID < 1)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            CServiceData service = new CServiceData(BaseMstr.BaseData);
            CServiceDataItem di = null;
            CStatus status = service.GetServiceDI(LongID, out di);
            if (!status.Status)
            {
                return status;
            }

            txtService.Text = di.ServiceLabel;
            OriginalLabel = di.ServiceLabel;
            chkActive.Checked = di.IsActive;
        }

        return new CStatus();
    }

    /// <summary>
    /// override
    /// saves the values in the dialog and fires the save
    /// event if the save was successful
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CServiceDataItem di = new CServiceDataItem();
        di.ServiceLabel = txtService.Text;
        di.IsActive = chkActive.Checked;

        CServiceData service = new CServiceData(BaseMstr.BaseData);
        CStatus status = new CStatus();
        switch (EditMode)
        {
            case k_EDIT_MODE.INSERT:
                long lServiceID = -1;
                status = service.InsertService(di, out lServiceID);
                if (status.Status)
                {
                    LongID = lServiceID;
                    EditMode = k_EDIT_MODE.UPDATE;
                }
                break;
            case k_EDIT_MODE.UPDATE:
                di.ServiceID = LongID;
                status = service.UpdateService(di);
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
    /// override
    /// checks if the dialog values are valid
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        CStatus status = new CStatus();
        plistStatus = new CParameterList();
        if (string.IsNullOrEmpty(txtService.Text))
        {
            plistStatus.AddInputParameter("ERROR_VE_SERVICELABEL", Resources.ErrorMessages.ERROR_VE_SERVICELABEL);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        if (EditMode == k_EDIT_MODE.INSERT
            || EditMode == k_EDIT_MODE.UPDATE && txtService.Text != OriginalLabel)
        {
            if (CGridView.CellValueExists(GView, 1, txtService.Text))
            {
                plistStatus.AddInputParameter("ERROR_DATA_EXISTS", Resources.ErrorMessages.ERROR_DATA_EXISTS);
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
            }
        }

        return status;
    }

    /// <summary>
    /// event
    /// validates and saves if the validate is successful
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSave(object sender, EventArgs e)
    {
        CParameterList pList = null;
        CStatus status = ValidateUserInput(out pList);
        if (!status.Status)
        {
            ShowStatusInfo(status.StatusCode, pList);
            ShowMPE();
            return;
        }
            
        status = SaveControl();
        if (!status.Status)
        {
            ShowStatusInfo(status);
            ShowMPE();
            return;
        }
    }
}
