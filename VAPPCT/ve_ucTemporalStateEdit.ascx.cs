using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ve_ucTemporalStateEdit : CAppUserControlPopup
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
    /// page_load: set the title for the popup
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Temporal State";
    }

    /// <summary>
    /// load the temporal state definition drop down
    /// </summary>
    /// <returns></returns>
    private CStatus LoadTSDefinitionDropDown()
    {
        if (ddlTSDefinition.Items.Count < 1)
        {
            CStatus status = CSTAT.LoadTSDefinitionDDL(BaseMstr.BaseData, ddlTSDefinition);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// load the user control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        //cache the editmode for postbacks
        EditMode = lEditMode;

        //load the TS definition drop down
        CStatus status = LoadTSDefinitionDropDown();
        if (!status.Status)
        {
            return status;
        }

        //reset the controls
        txtTSLabel.Text = string.Empty;
        txtTSLabel.Focus();
        ddlTSDefinition.SelectedIndex = -1;
        chkTSActive.Checked = false;

        //default active check to true
        if (lEditMode == k_EDIT_MODE.INSERT)
        {
            chkTSActive.Checked = true;
        }
        //load data to the controls if we are in update mode
        else if (lEditMode == k_EDIT_MODE.UPDATE)
        {
            if (LongID < 1)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            //get a 1 record TS dataset
            CTemporalStateData tsData = new CTemporalStateData(BaseMstr.BaseData);
            CTemporalStateDataItem di = null;
            status = tsData.GetTemporalStateDI(LongID, out di);
            if(!status.Status)
            {
                return status;
            }
            
            //set the label text
            txtTSLabel.Text = di.TSLabel;
            OriginalLabel = txtTSLabel.Text;
            ddlTSDefinition.SelectedValue = di.TSDefinitionID.ToString();
            chkTSActive.Checked = di.IsActive;
        }
        
        return new CStatus();
    }

    /// <summary>
    /// validate our controls user input
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput( out CParameterList plistStatus)
    {
        CStatus status = new CStatus();
        plistStatus = new CParameterList();
  
        //label
        if (txtTSLabel.Text.Length < 1)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            plistStatus.AddInputParameter("ERROR_TS_LABEL", Resources.ErrorMessages.ERROR_TS_LABEL);
        }
            
        //definition
        if (ddlTSDefinition.SelectedItem == null || String.IsNullOrEmpty(ddlTSDefinition.SelectedItem.Text))
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            plistStatus.AddInputParameter("ERROR_TS_DEFINITION", Resources.ErrorMessages.ERROR_TS_DEFINITION);
        }
        
        //active - nothing to check
        
        //if we are inserting make sure the row 
        //does not already esist.
        if (EditMode == k_EDIT_MODE.INSERT
            || EditMode == k_EDIT_MODE.UPDATE && txtTSLabel.Text != OriginalLabel)
        {
            if (CGridView.CellValueExists(GView, 1, txtTSLabel.Text))
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                plistStatus.AddInputParameter("ERROR_DATA_EXISTS", Resources.ErrorMessages.ERROR_DATA_EXISTS);
            }
        }
        
        return status;
    }

    /// <summary>
    /// save the user control
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = new CStatus();
        switch (EditMode)
        {
            case k_EDIT_MODE.INSERT:
                long lTSID = -1;
                status = InsertTemporalState(out lTSID);
                if (status.Status)
                {
                    LongID = lTSID;
                    EditMode = k_EDIT_MODE.UPDATE;
                }
                break;
            case k_EDIT_MODE.UPDATE:
                status = UpdateTemporalState(LongID);
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
    /// creates a new temporal state data item and loads it with the values from the dialog
    /// </summary>
    /// <returns></returns>
    private CTemporalStateDataItem LoadNewDataItem()
    {
        CTemporalStateDataItem di = new CTemporalStateDataItem();
        di.TSID = -1;
        di.TSLabel = txtTSLabel.Text;
        di.TSDefinitionID = Convert.ToInt64(ddlTSDefinition.SelectedValue);
        di.IsActive = chkTSActive.Checked;
        return di;
    }

    /// <summary>
    /// update a temporal state in the db
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    private CStatus UpdateTemporalState(long lTSID)
    {
        CTemporalStateDataItem tsdi = LoadNewDataItem();
        tsdi.TSID = lTSID;

        CTemporalStateData tsd = new CTemporalStateData(BaseMstr.BaseData);
        return tsd.UpdateTemporalState(tsdi);
    }

    /// <summary>
    /// insert a new temporal state into the db
    /// </summary>
    /// <param name="lTSID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    private CStatus InsertTemporalState(out long lTSID)
    {
        lTSID = -1;

        //load a dataitem for the insert
        CTemporalStateDataItem tsdi = LoadNewDataItem();

        //insert the item
        CTemporalStateData tsd = new CTemporalStateData(BaseMstr.BaseData);
        CStatus status = tsd.InsertTemporalState(tsdi, out lTSID);
        if(!status.Status)
        {
            return status;
        }

        //cache the new TS_ID
        LongID = lTSID;

        return status;
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
    /// by doing nothing we effectivly "cancel" the popup,
    /// this allows us to put the cancel button on the control 
    /// instead of having a "default cancel" on the page with
    /// the control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickCancel(object sender, EventArgs e)
    {

    }
}
