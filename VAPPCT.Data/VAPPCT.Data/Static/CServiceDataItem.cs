using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CServiceDataItem
/// </summary>
public class CServiceDataItem
{
    public long ServiceID { get; set; }
    public string ServiceLabel { get; set; }
    public bool IsActive { get; set; }

	public CServiceDataItem()
	{
	}

    public CServiceDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            ServiceID = CDataUtils.GetDSLongValue(ds, "SERVICE_ID");
            ServiceLabel = CDataUtils.GetDSStringValue(ds, "SERVICE_LABEL");
            IsActive = (CDataUtils.GetDSLongValue(ds, "IS_ACTIVE") == (long)k_TRUE_FALSE_ID.True) ? true : false;
        }
    }
}
