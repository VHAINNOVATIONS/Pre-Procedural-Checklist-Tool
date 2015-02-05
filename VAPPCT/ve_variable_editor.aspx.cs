using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// codebehind for the variable editor screen
/// </summary>
public partial class ve_variable_editor : System.Web.UI.Page
{
    /// <summary>
    /// handle page load events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucTemporalState.BaseMstr = Master;
        ucOutcomeState.BaseMstr = Master;
        ucDecisionState.BaseMstr = Master;
        ucItemGroup.BaseMstr = Master;
        ucService.BaseMstr = Master;

        //load data on the 1st visit to the form
        if (!IsPostBack)
        {
            Master.PageTitle = "Variable Editor";

            //load all the gridviews with data
            CStatus status = ucTemporalState.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
            }

            status = ucOutcomeState.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
            }

            status = ucDecisionState.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
            }

            status = ucItemGroup.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
            }

            status = ucService.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                Master.ShowStatusInfo(status);
            }
        }
        
        //todo: testing...
        //pulls out 44 at a time...
        //CMDWSOps ops = new CMDWSOps(Master.BaseData);
        //ops.GetMDWSLabTests("C1 ESTERASE INHIBIT.");
        //ops.GetMDWSLabTests("CHEMISTRY FLUIDS");
        //CStatus status = new CStatus();
        //DataSet dsLabTests = null;
        //CLabData ld = new CLabData(Master.BaseData);
        //status = ld.GetLabTestDS("CHEMISTRY FLUIDS", out dsLabTests);
        
        //CMDWSTransfer t = new CMDWSTransfer(Master.BaseData);
        //t.LoopPatientRads();
        //t.LoopPatientLabs();

        //CMDWSOps ops = new CMDWSOps(Master.BaseData);
        //string strRPCs = String.Empty;
        //ops.GetRPCs(out strRPCs);

        CMDWSTransfer xfer = new CMDWSTransfer(Master.BaseData);
        //xfer.LoopPatientRads();
        //xfer.LoopPatientMeds();
        //xfer.LoopPatientICD();
        //xfer.LoopOrders();
        
       }
}