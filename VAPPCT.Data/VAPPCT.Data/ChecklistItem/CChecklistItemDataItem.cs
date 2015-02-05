using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

public class CChecklistItemDataItem
{
    public long ChecklistID { get; set; }
    public long ItemID { get; set; }
    public long CLITSTimePeriod { get; set; }
    public k_TIME_UNIT_ID TimeUnitID { get; set; }
    public long SortOrder { get; set; }
    public k_ACTIVE_ID ActiveID { get; set; }
    public string Logic { get; set; }

    public CChecklistItemDataItem()
    {
    }

    public CChecklistItemDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            ChecklistID = CDataUtils.GetDSLongValue(ds, "CHECKLIST_ID");
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            CLITSTimePeriod = CDataUtils.GetDSLongValue(ds, "CLI_TS_TIME_PERIOD");
            TimeUnitID = (k_TIME_UNIT_ID)CDataUtils.GetDSLongValue(ds, "TIME_UNIT_ID");
            SortOrder = CDataUtils.GetDSLongValue(ds, "SORT_ORDER");
            ActiveID = (k_ACTIVE_ID)CDataUtils.GetDSLongValue(ds, "ACTIVE_ID");
            Logic = CDataUtils.GetDSStringValue(ds, "LOGIC"); ;
        }
    }
}
