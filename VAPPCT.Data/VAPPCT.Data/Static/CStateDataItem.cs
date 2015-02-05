using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CStateDataItem
/// </summary>
public class CStateDataItem
{
    public long StateID { get; set; }
    public string StateLabel { get; set; }

    public CStateDataItem(DataSet ds)
	{
        if (!CDataUtils.IsEmpty(ds))
        {
            StateID = CDataUtils.GetDSLongValue(ds, "STATE_ID");
            StateLabel = CDataUtils.GetDSStringValue(ds, "STATE_LABEL");
        }
	}
}
