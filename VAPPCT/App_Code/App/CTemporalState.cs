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

public static class CTemporalState
{
    /// <summary>
    /// method
    /// Load temporal state data into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadTSDropDownList(CData Data, DropDownList ddl)
    {
        ddl.Items.Clear();

        //get the dataset
        DataSet dsTS = null;
        CTemporalStateData tsd = new CTemporalStateData(Data);
        CStatus status = tsd.GetTemporalStateDS((long)k_ACTIVE_ID.All, out dsTS);
        if(!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsTS,
            ddl,
            "TS_LABEL",
            "TS_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
