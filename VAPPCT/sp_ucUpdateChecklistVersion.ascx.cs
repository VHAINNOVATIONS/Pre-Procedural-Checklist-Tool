using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

//our data and ui libraries
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucUpdateChecklistVersion : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _UpdateVersion;
    /// <summary>
    /// property
    /// adds/removes event handlers to the select event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> UpdateVersion
    {
        add { _UpdateVersion += new EventHandler<CAppUserControlArgs>(value); }
        remove { _UpdateVersion -= value; }
    }

     /// <summary>
    /// patient id property used by sp_single_patient.aspx
    /// </summary>
    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientID"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "PatientID"] = value; }
    }


    /// <summary>
    /// property
    /// stores a the checklist id to check
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

    protected void Page_Load(object sender, EventArgs e)
    {
        //Set the title
        Title = "Update Checklist Version";

        if (!IsPostBack)
        {


        }


    }
    
    /// <summary>
    /// loop and update the users checklists
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        bool bUpdated = false;

        foreach (GridViewRow gvr in gvOutOfDateCL.Rows)
        {
            CheckBox cb = gvr.FindControl("chkSelect") as CheckBox;
            if (cb != null)
            {
                if (cb.Checked)
                {

                    CStatus status = new CStatus();
                    CPatChecklistData dta = new CPatChecklistData(BaseMstr.BaseData);
                    dta.UpdatePatCLVersion(Convert.ToInt32(gvOutOfDateCL.DataKeys[gvr.RowIndex].Value));
                    bUpdated = true;
                }
            }
        }

        if (bUpdated)
        {
            if (_UpdateVersion != null)
            {
                CAppUserControlArgs args = new CAppUserControlArgs(
                    k_EVENT.UPDATE,
                    k_STATUS_CODE.Success,
                    string.Empty,
                    "1");

                _UpdateVersion(this, args);
            }
        }
    }


    /// <summary>
    /// user cancelled the popup
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();
        CStatus status = new CStatus();
        return status;

    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = new CStatus();
        return status;
    }

    /// <summary>
    /// override
    /// sets focus
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        CStatus status = new CStatus();
        CPatChecklistData dta = new CPatChecklistData(BaseMstr.BaseData);
        DataSet dsCL = null;
        dta.GetOutOfDatePatCLDS(PatientID, ChecklistID, out dsCL);
        gvOutOfDateCL.DataSource = dsCL.Tables[0];
        gvOutOfDateCL.DataBind();


        return status;
    }
}
