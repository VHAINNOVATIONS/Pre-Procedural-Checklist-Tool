using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an item component state entry
/// </summary>
public class CICStateDataItem
{
    public long ItemID { get; set; }
    public long ItemComponentID { get; set; }
    public long ICStateID { get; set; }
    public long StateID { get; set; }

    public CICStateDataItem()
    {
    }

    public CICStateDataItem(DataSet ds)
	{
        if (!CDataUtils.IsEmpty(ds))
        {
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            ItemComponentID = CDataUtils.GetDSLongValue(ds, "ITEM_COMPONENT_ID");
            ICStateID = CDataUtils.GetDSLongValue(ds, "IC_STATE_ID");
            StateID = CDataUtils.GetDSLongValue(ds, "STATE_ID");
        }
	}
}
