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
/// code behind for the outcome state add/edit popup
/// </summary>
public partial class ve_ucOutcomeStateEdit : CAppUserControlPopup
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
    /// stores the original text of the outcome state
    /// </summary>
    public string OriginalLabel
    {
        get { return ViewState[ClientID + "OriginalLabel"].ToString(); }
        private set { ViewState[ClientID + "OriginalLabel"] = value; }
    }

    /// <summary>
    /// page load, set the title of the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Outcome State";
    }
    
    /// <summary>
    /// load the outcome state definition drop down
    /// </summary>
    /// <returns></returns>
    protected CStatus LoadOSDefinitionDropDown()
    {
        if (ddlOSDefinition.Items.Count < 1)
        {
            CStatus status = CSTAT.LoadOSDefinitionDDL(BaseMstr.BaseData, ddlOSDefinition);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
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

        //load the OS definition drop down
        CStatus status = LoadOSDefinitionDropDown();
        if(!status.Status)
        {
            return status;
        }

        //clear/reset the controls
        txtOSLabel.Text = string.Empty;
        txtOSLabel.Focus();
        ddlOSDefinition.SelectedIndex = -1;
        chkOSActive.Checked = false;
        
        if (lEditMode == k_EDIT_MODE.INSERT)
        {
            chkOSActive.Checked = true;
        }
        else if (lEditMode == k_EDIT_MODE.UPDATE)
        {
            //get a 1 record TS dataset
            COutcomeStateDataItem di = null;
            COutcomeStateData osData = new COutcomeStateData(BaseMstr.BaseData);
            status = osData.GetOutcomeStateDI(LongID, out di);
            if(!status.Status)
            {
                return status;
            }

            txtOSLabel.Text = di.OSLabel;
            OriginalLabel = txtOSLabel.Text;
            ddlOSDefinition.SelectedValue = di.OSDefinitionID.ToString();
            chkOSActive.Checked = di.IsActive;
        }

        return status;
    }

    /// <summary>
    /// save the control
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = new CStatus();
        //user input is ok so save
        switch (EditMode)
        {
            case k_EDIT_MODE.INSERT:
                long lOSID = -1;
                status = InsertOutcomeState(out lOSID);
                if(status.Status)
                {
                    LongID = lOSID;
                    EditMode = k_EDIT_MODE.UPDATE;
                }
                break;

            case k_EDIT_MODE.UPDATE:
                status = UpdateOutcomeState(LongID);
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
    /// validate user input
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        //parameter list to hold status information
        plistStatus = new CParameterList();
        CStatus status = new CStatus();

        //label
        if (txtOSLabel.Text.Length < 1)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            plistStatus.AddInputParameter("ERROR_OS_LABEL", Resources.ErrorMessages.ERROR_OS_LABEL);
        }

        //definition
        if (ddlOSDefinition.SelectedItem == null ||
            String.IsNullOrEmpty(ddlOSDefinition.SelectedItem.Text))
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            plistStatus.AddInputParameter("ERROR_OS_DEFINITION", Resources.ErrorMessages.ERROR_OS_DEFINITION);
        }

        //active - nothing to check

        //if we are inserting make sure the row 
        //does not already esist.
        if (EditMode == k_EDIT_MODE.INSERT
            || EditMode == k_EDIT_MODE.UPDATE && txtOSLabel.Text != OriginalLabel)
        {
            if (GView != null)
            {
                if (CGridView.CellValueExists(GView, 1, txtOSLabel.Text))
                {
                    plistStatus.AddInputParameter("ERROR_DATA_EXISTS", Resources.ErrorMessages.ERROR_DATA_EXISTS);
                }
            }
        }

        return status;
    }

    /// <summary>
    /// method
    /// creates a new outcome state data item and loads it with the values from the dialog
    /// </summary>
    /// <returns></returns>
    private COutcomeStateDataItem LoadNewDataItem()
    {
        COutcomeStateDataItem di = new COutcomeStateDataItem();
        di.OSID = -1;
        di.OSLabel = txtOSLabel.Text;
        di.OSDefinitionID = Convert.ToInt64(ddlOSDefinition.SelectedValue);
        di.IsActive = chkOSActive.Checked;
        return di;
    }

    /// <summary>
    /// insert an outcome state
    /// </summary>
    /// <param name="lOSID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    private CStatus InsertOutcomeState(out long lOSID)
    {
        lOSID = -1;

        COutcomeStateDataItem osdi = LoadNewDataItem();

        COutcomeStateData osd = new COutcomeStateData(BaseMstr.BaseData);
        CStatus status = osd.InsertOutcomeState(osdi, out lOSID);
        if(!status.Status)
        {
            return status;
        }

        LongID = lOSID;

        return status;
    }
    
    /// <summary>
    /// update an outcome state
    /// </summary>
    /// <param name="lOSID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    private CStatus UpdateOutcomeState(long lOSID)
    {
        COutcomeStateDataItem osdi = LoadNewDataItem();
        osdi.OSID = lOSID;

        COutcomeStateData osd = new COutcomeStateData(BaseMstr.BaseData);
        return osd.UpdateOutcomeState(osdi);
    }

    /// <summary>
    /// user clicked the save button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSave(object sender, EventArgs e)
    {
        //validate user input
        CParameterList plistStatus = null;
        CStatus status = ValidateUserInput(out plistStatus);
        if (!status.Status)
        {
            ShowStatusInfo(status);
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

    /// <summary>
    /// user clicked the cancel button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {

    }
}
