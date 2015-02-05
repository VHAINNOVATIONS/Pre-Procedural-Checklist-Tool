using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ve_ucDecisionStateEdit : CAppUserControlPopup
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
    /// page load, set the title of the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Decision State";
    }

    /// <summary>
    /// load the decision state drop down list
    /// </summary>
    /// <returns></returns>
    private CStatus LoadDSDefinitionDropDown()
    {
        if (ddlDSDefinition.Items.Count < 1)
        {
            CStatus status = CSTAT.LoadDSDefinitionDDL(BaseMstr.BaseData, ddlDSDefinition);
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
        
        //load the DS definition drop down
        CStatus status = LoadDSDefinitionDropDown();
        if (!status.Status)
        {
            return status;
        }

        //clear/reset the controls
        ddlDSDefinition.SelectedIndex = -1;
        txtDSLabel.Text = string.Empty;
        chkDSActive.Checked = false;
        txtDSLabel.Focus();

        //default active check to true
        if (lEditMode == k_EDIT_MODE.INSERT)
        {
            chkDSActive.Checked = true;
        }
        else if (lEditMode == k_EDIT_MODE.UPDATE)
        {
            if (LongID < 1)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            //get a 1 record TS dataset
            CDecisionStateDataItem di = null;
            CDecisionStateData dsData = new CDecisionStateData(BaseMstr.BaseData);
            status = dsData.GetDecisionStateDI(LongID, out di);
            if(!status.Status)
            {
                //if the load failed, disable the save and return
                btnucDSESave.Enabled = false;
                ShowStatusInfo(status);
                return status;
            }

            //load the forms controls with data
            ddlDSDefinition.SelectedValue = di.DSDefinitionID.ToString();
            txtDSLabel.Text = di.DSLabel;
            OriginalLabel = txtDSLabel.Text;
            chkDSActive.Checked = di.IsActive;
        }

        return status;
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
        plistStatus = new CParameterList();
        CStatus status = new CStatus();

        //label
        if (txtDSLabel.Text.Length < 1)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            plistStatus.AddInputParameter("ERROR_DS_LABEL", Resources.ErrorMessages.ERROR_DS_LABEL);
        }

        //definition
        if (ddlDSDefinition.SelectedItem == null || String.IsNullOrEmpty(ddlDSDefinition.SelectedItem.Text))
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            plistStatus.AddInputParameter("ERROR_DS_DEFINITION", Resources.ErrorMessages.ERROR_DS_DEFINITION);
        }

        //active - nothing to check

        //if we are inserting make sure the row 
        //does not already esist.
        if (EditMode == k_EDIT_MODE.INSERT
            || EditMode == k_EDIT_MODE.UPDATE && txtDSLabel.Text != OriginalLabel)
        {
            if (CGridView.CellValueExists(GView, 1, txtDSLabel.Text))
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                plistStatus.AddInputParameter("ERROR_DATA_EXISTS", Resources.ErrorMessages.ERROR_DATA_EXISTS);
            }
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
        switch (EditMode)
        {
            case k_EDIT_MODE.INSERT:
                long lDSID = -1;
                status = InsertDecisionState(out lDSID);
                if (status.Status)
                {
                    LongID = lDSID;
                    EditMode = k_EDIT_MODE.UPDATE;
                }
                break;
            case k_EDIT_MODE.UPDATE:
                status = UpdateDecisionState(LongID);
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
    /// method
    /// creates a new decision state data item and loads it with the values from the dialog
    /// </summary>
    /// <returns></returns>
    private CDecisionStateDataItem LoadNewDataItem()
    {
        CDecisionStateDataItem di = new CDecisionStateDataItem();
        di.DSID = -1;
        di.DSLabel = txtDSLabel.Text;
        di.DSDefinitionID = Convert.ToInt64(ddlDSDefinition.SelectedValue);
        di.IsActive = chkDSActive.Checked;
        return di;
    }

    /// <summary>
    /// insert a decision state
    /// </summary>
    /// <param name="lDSID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    private CStatus InsertDecisionState(out long lDSID)
    {
        lDSID = -1;

        //create a new data item and load it
        CDecisionStateDataItem dsdi = LoadNewDataItem();

        //save the data item
        CDecisionStateData dsd = new CDecisionStateData(BaseMstr.BaseData);
        CStatus status = dsd.InsertDecisionState(dsdi, out lDSID);
        if (!status.Status)
        {
            return status;
        }

        //cache the new TS_ID
        LongID = lDSID;

        return status;
    }

    /// <summary>
    /// update a decision state
    /// </summary>
    /// <param name="lTSID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    private CStatus UpdateDecisionState(long lDSID)
    {
        //create a new data item and load it
        CDecisionStateDataItem dsdi = LoadNewDataItem();
        dsdi.DSID = lDSID;

        CDecisionStateData dsd = new CDecisionStateData(BaseMstr.BaseData);
        return dsd.UpdateDecisionState(dsdi);
    }

    /// <summary>
    /// user clicked the save button
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
    /// user clicked the cancel button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {

    }
}
