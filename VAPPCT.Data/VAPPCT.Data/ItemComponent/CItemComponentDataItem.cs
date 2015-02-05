using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an item component entry
/// </summary>
public class CItemComponentDataItem
{
    public long ItemID { get; set; }
    public long ItemComponentID { get; set; }
    public string ItemComponentLabel { get; set; }
    public long SortOrder { get; set; }
    public k_ACTIVE_ID ActiveID { get; set; }

    public CItemComponentDataItem()
    {
    }

    public CItemComponentDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            ItemComponentID = CDataUtils.GetDSLongValue(ds, "ITEM_COMPONENT_ID");
            ItemComponentLabel = CDataUtils.GetDSStringValue(ds, "ITEM_COMPONENT_LABEL");
            SortOrder = CDataUtils.GetDSLongValue(ds, "SORT_ORDER");
            ActiveID = (k_ACTIVE_ID)CDataUtils.GetDSLongValue(ds, "ACTIVE_ID");
        }
    }
}
