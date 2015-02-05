using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an item group entry
/// </summary>
public class CItemGroupDataItem
{
    public string ItemGroupLabel { get; set; }
    public long ItemGroupID { get; set; }
    public bool IsActive { get; set; }

	public CItemGroupDataItem()
	{

	}

    public CItemGroupDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            ItemGroupLabel = CDataUtils.GetDSStringValue(ds, "ITEM_GROUP_LABEL");
            ItemGroupID = CDataUtils.GetDSLongValue(ds, "ITEM_GROUP_ID");
            IsActive = (CDataUtils.GetDSLongValue(ds, "IS_ACTIVE") == (long)k_TRUE_FALSE_ID.True) ? true : false;
        }
    }
}
