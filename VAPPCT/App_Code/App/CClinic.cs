using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// Helper class for loading ui controls with clinic data
/// </summary>
public static class CClinic
{
    public static CStatus LoadClinicDLL(CData Data, DropDownList ddl)
    {
        ddl.Items.Clear();

        DataSet dsClinics = null;
        CClinicData cd = new CClinicData(Data);
        CStatus status = cd.GetClinicDS(out dsClinics);
        if (!status.Status)
        {
            return status;
        }

        status = CDropDownList.RenderDataSet(
            dsClinics,
            ddl,
            "CLINIC_LABEL",
            "CLINIC_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
