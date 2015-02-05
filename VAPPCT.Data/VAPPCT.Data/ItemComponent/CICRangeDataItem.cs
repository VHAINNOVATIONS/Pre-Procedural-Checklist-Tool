using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an item component range entry
/// </summary>
public class CICRangeDataItem
{
    public long ItemID { get; set; }
    public long ItemComponentID { get; set; }
    public long ICRangeID { get; set; }
    public string Units { get; set; }
    public double LegalMin { get; set; }
    public double CriticalLow { get; set; }
    public double Low { get; set; }
    public double High { get; set; }
    public double CriticalHigh { get; set; }
    public double LegalMax { get; set; }

    public CICRangeDataItem()
    {
    }

    public CICRangeDataItem(DataSet ds)
	{
        if (!CDataUtils.IsEmpty(ds))
        {
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            ItemComponentID = CDataUtils.GetDSLongValue(ds, "ITEM_COMPONENT_ID");
            ICRangeID = CDataUtils.GetDSLongValue(ds, "IC_RANGE_ID");
            Units = CDataUtils.GetDSStringValue(ds, "UNITS");
            LegalMin = CDataUtils.GetDSDoubleValue(ds, "LEGAL_MIN");
            CriticalLow = CDataUtils.GetDSDoubleValue(ds, "CRITICAL_LOW");
            Low = CDataUtils.GetDSDoubleValue(ds, "LOW");
            High = CDataUtils.GetDSDoubleValue(ds, "HIGH");
            CriticalHigh = CDataUtils.GetDSDoubleValue(ds, "CRITICAL_HIGH");
            LegalMax = CDataUtils.GetDSDoubleValue(ds, "LEGAL_MAX");
        }
	}
}
