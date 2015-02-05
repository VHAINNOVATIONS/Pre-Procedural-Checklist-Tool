using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;

public partial class app_ucCancelProcessing : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _Cancel;
    public event EventHandler<CAppUserControlArgs> Cancel
    {
        add { _Cancel += new EventHandler<CAppUserControlArgs>(value); }
        remove { _Cancel -= value; }
    }

    public string Count
    {
        set { lblCount.Text = value; }
    }

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        throw new NotImplementedException();
    }

    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Title = "Processing Progress";
        }
    }

    protected void OnClickCancel(object sender, EventArgs e)
    {
        ShowParentMPE();

        if (_Cancel != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                "1");

            _Cancel(this, args);
        }
    }
}
