using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucExistingChecklist : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _Continue;
    public event EventHandler<CAppUserControlArgs> Continue
    {
        add { _Continue += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Continue -= value; }
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

    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientID"];
            return (obj != null) ? obj.ToString() : string.Empty;
        }
        set { ViewState[ClientID + "PatientID"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Title = "Patient With Checklist Open";
        }
    }

    protected void OnClickContinue(object sender, EventArgs e)
    {
        Visible = false;

        if (_Continue != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                "1");

            _Continue(this, args);
        }
    }

    protected void OnClickCancel(object sender, EventArgs e)
    {
        Visible = false;
    }

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        //get the data
        CPatChecklistData cld = new CPatChecklistData(BaseMstr.BaseData);
        bool bHasPatCL = false;
        CStatus status = cld.HasPatientChecklist(
            ChecklistID,
            (long)k_CHECKLIST_STATE_ID.Open,
            PatientID,
            out bHasPatCL);
        if (!status.Status)
        {
            return status;
        }

        if (bHasPatCL)
        {
            ShowMPE();
        }
        else
        {
            if (_Continue != null)
            {
                CAppUserControlArgs args = new CAppUserControlArgs(
                    k_EVENT.SELECT,
                    k_STATUS_CODE.Success,
                    string.Empty,
                    "1");

                _Continue(this, args);
            }
        }

        return new CStatus();
    }

    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }
}
