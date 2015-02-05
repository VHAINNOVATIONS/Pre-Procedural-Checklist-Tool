using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucTIUNote : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _NoteSaved;
    /// <summary>
    /// property
    /// adds/removes event handlers to the select event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> NoteSaved
    {
        add { _NoteSaved += new EventHandler<CAppUserControlArgs>(value); }
        remove { _NoteSaved -= value; }
    }

    /// <summary>
    /// US:1956 US:885 property
    /// stores a patient id for the page
    /// </summary>
    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientID"];
            return (obj != null) ? obj.ToString() : "-1";
        }
        set { ViewState[ClientID + "PatientID"] = value; }
    }

    /// <summary>
    /// US:1956 US:885 note title of the note we need to write
    /// </summary>
    public string NoteTitle
    {
        get
        {
            object obj = ViewState[ClientID + "NoteTitle"];
            return (obj != null) ? obj.ToString() : "-1";
        }
        set { ViewState[ClientID + "NoteTitle"] = value; }
    }

    /// <summary>
    /// US:1956 US:885 note title ien of the note we need to write
    /// </summary>
    public string NoteTitleIEN
    {
        get
        {
            object obj = ViewState[ClientID + "NoteTitleIEN"];
            return (obj != null) ? obj.ToString() : "-1";
        }
        set { ViewState[ClientID + "NoteTitleIEN"] = value; }
    }

    /// <summary>
    /// US:1956 US:885 property
    /// stores a patient checklist id for the user control
    /// </summary>
    public long PatChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "PatChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "PatChecklistID"] = value; }
    }

    /// <summary>
    /// US:1956 US:885 page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Title = "TIU Note";
        }
    }

    /// <summary>
    /// US:1956 US:885 load the control with data
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        CStatus status = CClinic.LoadClinicDLL(BaseMstr.BaseData, ddlClinics);
        if (!status.Status)
        {
            return status;
        }

        //load the tiu note
        string strTIU = String.Empty;
        string strNoteTitle = String.Empty;
   
        CPatChecklistData clData = new CPatChecklistData(BaseMstr.BaseData);
        status = clData.GetTIUText(
            PatientID,
            PatChecklistID,
            out strNoteTitle,
            out strTIU);
        if (!status.Status)
        {
            return status;
        }

        //keep the note title as a property
        NoteTitle = strNoteTitle;

        //select the note title default clinic from the checklist
        CPatChecklistDataItem pdi = new CPatChecklistDataItem();
        status = clData.GetPatChecklistDI(PatChecklistID, out pdi);
        if (!status.Status)
        {
            return status;
        }

        CChecklistDataItem cli = new CChecklistDataItem();
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        status = cld.GetCheckListDI(pdi.ChecklistID, out cli);
        if (!status.Status)
        {
            return status;
        }

        if (cli.NoteTitleClinicID > 0)
        {
            ddlClinics.SelectedValue = cli.NoteTitleClinicID.ToString();
        }

        //show the note title at the top of the popup
        lblNoteTitle.Text = strNoteTitle;

        //the note title tag is the title of the note, but we need the ien
        //to write the note...
        long lNoteTitleIEN = 0;
        CNoteTitleData nd = new CNoteTitleData(BaseMstr.BaseData);
        status = nd.GetNoteTitleIEN(strNoteTitle, out lNoteTitleIEN);
        if (!status.Status)
        {
            return status;
        }

        NoteTitleIEN = Convert.ToString(lNoteTitleIEN);
        
        //set the text for the note
        txtTIU.Text = strTIU;
        
        return new CStatus();
    }

    /// <summary>
    /// US:885 validate user input
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CStatus status = new CStatus();

        if (ddlClinics.SelectedIndex < 1)
        {
            plistStatus.AddInputParameter("ERROR_CLINIC", "Please select a clinic");
        }

        if (plistStatus.Count > 0)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        return status;
    }
    
    /// <summary>
    /// US:885 save the control
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CMDWSOps ops = new CMDWSOps(this.BaseMstr.BaseData);
        CStatus status = ops.WriteNote(
            PatientID,
            this.BaseMstr.UserID.ToString(),
            txtSign.Text,
            ddlClinics.SelectedValue,
            NoteTitleIEN,
            txtTIU.Text);
        if (!status.Status)
        {
            return status;
        }

        //fire the NoteSaved event
        if (_NoteSaved != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                "1");

            _NoteSaved(this, args);
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1956 US:885 save and sign the TIU note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
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

        ShowParentMPE();
    }

    /// <summary>
    /// US:1956 US:885 cancel the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ShowParentMPE();
    }
}
