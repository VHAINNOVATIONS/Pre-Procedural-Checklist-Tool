using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucSinglePatientPopup : CAppUserControlPopup
{
    /// <summary>
    /// property
    /// stores a patient id for the page
    /// </summary>
    public string PatientID
    {
        get { return ucSinglePatientEditor.PatientID; }
        set { ucSinglePatientEditor.PatientID = value; }
    }

    /// <summary>
    /// property
    /// stores a checklist id for the user control
    /// </summary>
    public long ChecklistID
    {
        get { return ucSinglePatientEditor.ChecklistID; }
        set { ucSinglePatientEditor.ChecklistID = value; }
    }

    ///<summary>
    ///property
    /// stores a checklist label for the page
    /// </summary>
    public string ChecklistLabel
    {
        get { return ucSinglePatientEditor.ChecklistLabel; }
        set { ucSinglePatientEditor.ChecklistLabel = value; }
    }

    /// <summary>
    /// property
    /// stores a patient checklist id - Passed From MultiPatient View
    /// </summary>
    public long PatCLID
    {
        get { return ucSinglePatientEditor.PatCLID; }
        set { ucSinglePatientEditor.PatCLID = value; }
    }

    /// <summary>
    /// US:894
    /// Page Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //pass basemaster and mpe to the Single Patient Editor
        ucSinglePatientEditor.BaseMstr = BaseMstr;
        ucSinglePatientEditor.MPE = MPE;

        if (!IsPostBack)
        {
            Title = "Single Patient Editor";
        }
    }

    /// <summary>
    /// US:892
    /// US:894
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;
        CStatus status = ucSinglePatientEditor.LoadControl(EditMode);
        if(!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:892
    /// US:894
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// US:892
    /// US:894
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }
}
