using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;

public partial class sp_single_patient : System.Web.UI.Page
{
    /// <summary>
    /// event
    /// initialize page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucPatientChecklist.BaseMstr = Master;

        if (!IsPostBack)
        {
            Master.PageTitle = "Single Patient";

            try
            {
                ucPatientChecklist.PatientID = PreviousPage.PatientID;
                CStatus status = ucPatientChecklist.LoadControl(k_EDIT_MODE.INITIALIZE);
                if (!status.Status)
                {
                    Master.ShowStatusInfo(status);
                }
                
            }
            catch (Exception)
            {
                Master.ShowStatusInfo(k_STATUS_CODE.Failed, Resources.ErrorMessages.ERROR_SP_PATIENT);
            }
        }
    }
}
