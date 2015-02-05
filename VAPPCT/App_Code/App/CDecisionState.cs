using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public static class CDecisionState
{
    /// <summary>
    /// US:912
    /// loads a dropdown list of decision states
    /// </summary>
    /// <returns></returns>
    public static CStatus LoadCLIDecisionStatesDDL(
        CData Data,
        long lChecklistID,
        long lItemID,
        DropDownList ddl)
    {
        ddl.Items.Clear();

        DataSet dsDS = null;
        CChecklistItemData cid = new CChecklistItemData(Data);
        CStatus status = cid.GetDecisionStateDS(lChecklistID, lItemID, out dsDS);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsDS,
            ddl,
            "DS_LABEL",
            "DS_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
