using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// Helper class for loading ui controls with service data
/// </summary>
public static class CService
{
    /// <summary>
    /// Load services label data into dropdownlist
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ActiveFilter"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadServiceDDL(
        CData Data,
        k_ACTIVE_ID ActiveFilter,
        DropDownList ddl)
    {
        ddl.Items.Clear();

        DataSet dsService = null;
        CServiceData data = new CServiceData(Data);
        CStatus status = data.GetServiceDS(ActiveFilter, out dsService);
        if (!status.Status)
        {
            return status;
        }

        status = CDropDownList.RenderDataSet(
            dsService,
            ddl,
            "SERVICE_LABEL",
            "SERVICE_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
