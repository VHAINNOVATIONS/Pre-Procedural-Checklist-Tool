using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CPatientItemDataItem
/// </summary>
public class CPatientItemDataItem
{
    public string PatientID { get; set; }
    public long PatItemID { get; set; }
    public long ItemID { get; set; }
    public DateTime EntryDate { get; set; }
    public long SourceTypeID { get; set; }
    public string ItemLabel { get; set; }
    public string ItemDescription { get; set; }
    public long LookbackTime { get; set; }
    public long ItemTypeID { get; set; }
    public long ItemGroupID { get; set; }

    public CPatientItemDataItem()
    {
    }

    public CPatientItemDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            PatientID = CDataUtils.GetDSStringValue(ds, "PATIENT_ID");
            EntryDate = CDataUtils.GetDSDateTimeValue(ds, "ENTRY_DATE");
            ItemDescription = CDataUtils.GetDSStringValue(ds, "ITEM_DESCRIPTION");
            ItemGroupID = CDataUtils.GetDSLongValue(ds, "ITEM_GROUP_ID");
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            ItemLabel = CDataUtils.GetDSStringValue(ds, "ITEM_LABEL");
            ItemTypeID = CDataUtils.GetDSLongValue(ds, "ITEM_TYPE_ID");
            LookbackTime = CDataUtils.GetDSLongValue(ds, "LOOKBACK_TIME");
            PatItemID = CDataUtils.GetDSLongValue(ds, "PAT_ITEM_ID");
            SourceTypeID = CDataUtils.GetDSLongValue(ds, "SOURCE_TYPE_ID");
        }
    }
}
