using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// Summary description for CPatientItem
/// </summary>
public static class CPatientItem
{
    /// <summary>
    /// method
    /// loads all of the patient's items into the specified ddl
    /// for the specified item
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadPatientItemsDDL(
        CData Data,
        DropDownList ddl,
        string strPatientID,
        long lItemID)
    {
        CPatientItemData dta = new CPatientItemData(Data);
        DataSet dsItems = null;
        CStatus status = dta.GetPatientItemDS(
            strPatientID,
            lItemID,
            out dsItems);
        if (!status.Status)
        {
            return status;
        }

        status = CDropDownList.RenderDataSet(
            dsItems,
            ddl,
            "ENTRY_DATE",
            "PAT_ITEM_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
