using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to a checklist entry
/// </summary>
public class CChecklistDataItem
{
    public long ChecklistID { get; set; }
    public string ChecklistLabel { get; set; }
    public long ServiceID { get; set; }
    public string ChecklistDescription { get; set; }
    public k_ACTIVE_ID ActiveID { get; set; }
    public string NoteTitleTag { get; set; }
    public long NoteTitleClinicID { get; set; }

    public CChecklistDataItem()
    {
    }

    /// <summary>
    /// US:1951 US:1945 loads data item from data set
    /// </summary>
    /// <param name="ds"></param>
    public CChecklistDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            ChecklistID = CDataUtils.GetDSLongValue(ds, "CHECKLIST_ID");
            ChecklistLabel = CDataUtils.GetDSStringValue(ds, "CHECKLIST_LABEL");
            ServiceID = CDataUtils.GetDSLongValue(ds, "SERVICE_ID");
            ChecklistDescription = CDataUtils.GetDSStringValue(ds, "CHECKLIST_DESCRIPTION");
            NoteTitleTag = CDataUtils.GetDSStringValue(ds, "NOTE_TITLE_TAG");
            NoteTitleClinicID = CDataUtils.GetDSLongValue(ds, "NOTE_TITLE_CLINIC_ID");
            ActiveID = (k_ACTIVE_ID)CDataUtils.GetDSLongValue(ds, "ACTIVE_ID");
        }
    }
}
