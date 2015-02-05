using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an item entry
/// </summary>
public class CItemDataItem
{
    public long ItemID { get; set; }
    public long ItemTypeID { get; set; }
    public long ItemGroupID { get; set; }
    public string ItemLabel { get; set; }
    public string ItemDescription { get; set; }
    public long LookbackTime { get; set; }
    public k_ACTIVE_ID ActiveID { get; set; }
    public string MapID { get; set; }

    public CItemDataItem()
	{
	}

    public CItemDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            ItemTypeID = CDataUtils.GetDSLongValue(ds, "ITEM_TYPE_ID");
            ItemGroupID = CDataUtils.GetDSLongValue(ds, "ITEM_GROUP_ID");
            ItemLabel = CDataUtils.GetDSStringValue(ds, "ITEM_LABEL");
            ItemDescription = CDataUtils.GetDSStringValue(ds, "ITEM_DESCRIPTION");
            LookbackTime = CDataUtils.GetDSLongValue(ds, "LOOKBACK_TIME");
            ActiveID = (k_ACTIVE_ID)CDataUtils.GetDSLongValue(ds, "ACTIVE_ID");
            MapID = CDataUtils.GetDSStringValue(ds, "MAP_ID");
        }
    }
}
