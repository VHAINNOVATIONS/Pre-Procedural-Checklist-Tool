using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// Summary description for CPatientChecklist
/// </summary>
public class CPatientChecklist
{
	public CPatientChecklist()
	{

	}

    /// <summary>
    /// US:864 US:876 loads a dropdown list with patient checklists, will screen out 
    /// check lists that the user does not have view permissions for.
    /// </summary>
    /// <param name="BaseMastr"></param>
    /// <param name="strPatientID"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public CStatus LoadPatientChecklists(CBaseMaster BaseMstr, string strPatientID, DropDownList ddl)
    {
        CPatientChecklistLogic pcl = new CPatientChecklistLogic(BaseMstr.BaseData);
        CStatus status = pcl.RunLogic(strPatientID);
        if(!status.Status)
        {
            return status;
        }

        CPatChecklistData pc = new CPatChecklistData(BaseMstr.BaseData);
        DataSet ds = null;
        status = pc.GetPatChecklistDS(strPatientID, out ds);
        if (!status.Status)
        {
            return status;
        }

        //remove records from the ds that the user is not allowed to view
        foreach (DataTable table in ds.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                long lChecklistID = CDataUtils.GetDSLongValue(dr, "CHECKLIST_ID");

                CChecklistPermissionsDataItem pdi = null;
                CChecklistData clData = new CChecklistData(BaseMstr.BaseData);
                clData.GetCheckListPermissionsDI(lChecklistID, out pdi);

                //is the user allowed to view this checklist
                if (!pdi.HasPermission(BaseMstr.AppUser, k_CHECKLIST_PERMISSION.Viewable))
                {
                    dr.Delete();
                }
            }
        }
        ds.AcceptChanges();

        //render the dataset
        status = CDropDownList.RenderDataSet(
            ds,
            ddl,
            "CHECKLIST_LABEL",
            "PAT_CL_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
