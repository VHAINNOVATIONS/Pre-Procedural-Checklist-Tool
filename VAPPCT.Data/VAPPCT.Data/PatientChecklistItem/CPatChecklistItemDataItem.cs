using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// class representation of usr_pat_cl_item
/// </summary>
public class CPatChecklistItemDataItem
{
    public string PatientID { get; set; }
    public long ChecklistID { get; set; }
    public long ItemID { get; set; }
    public long TSID { get; set; }
    public long OSID { get; set; }
    public long DSID { get; set; }
    public long TSStateID { get; set; }
    public long OSStateID { get; set; }
    public long DSStateID { get; set; }
    public long PatCLID { get; set; }
    public k_TRUE_FALSE_ID IsEnabled { get; set; }
    public k_TRUE_FALSE_ID IsOverridden { get; set; }
    public DateTime OverrideDate { get; set; }

	public CPatChecklistItemDataItem()
	{
	}

    public CPatChecklistItemDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            PatCLID = CDataUtils.GetDSLongValue(ds, "PAT_CL_ID");
            PatientID = CDataUtils.GetDSStringValue(ds, "PATIENT_ID");
            ChecklistID = CDataUtils.GetDSLongValue(ds, "CHECKLIST_ID");
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            TSID = CDataUtils.GetDSLongValue(ds, "TS_ID");
            TSStateID = CDataUtils.GetDSLongValue(ds, "TS_STATE_ID");
            OSID = CDataUtils.GetDSLongValue(ds, "OS_ID");
            OSStateID = CDataUtils.GetDSLongValue(ds, "OS_STATE_ID");
            DSID = CDataUtils.GetDSLongValue(ds, "DS_ID");
            DSStateID = CDataUtils.GetDSLongValue(ds, "DS_STATE_ID");
            IsOverridden = (k_TRUE_FALSE_ID)CDataUtils.GetDSLongValue(ds, "IS_OVERRIDDEN");
            IsEnabled = (k_TRUE_FALSE_ID)CDataUtils.GetDSLongValue(ds, "IS_ENABLED");
            OverrideDate = CDataUtils.GetDSDateTimeValue(ds, "OVERRIDE_DATE");
        }
    }

    public CPatChecklistItemDataItem(DataRow dr)
    {
        PatCLID = CDataUtils.GetDSLongValue(dr, "PAT_CL_ID");
        PatientID = CDataUtils.GetDSStringValue(dr, "PATIENT_ID");
        ChecklistID = CDataUtils.GetDSLongValue(dr, "CHECKLIST_ID");
        ItemID = CDataUtils.GetDSLongValue(dr, "ITEM_ID");
        TSID = CDataUtils.GetDSLongValue(dr, "TS_ID");
        TSStateID = CDataUtils.GetDSLongValue(dr, "TS_STATE_ID");
        OSID = CDataUtils.GetDSLongValue(dr, "OS_ID");
        OSStateID = CDataUtils.GetDSLongValue(dr, "OS_STATE_ID");
        DSID = CDataUtils.GetDSLongValue(dr, "DS_ID");
        DSStateID = CDataUtils.GetDSLongValue(dr, "DS_STATE_ID");
        IsOverridden = (k_TRUE_FALSE_ID)CDataUtils.GetDSLongValue(dr, "IS_OVERRIDDEN");
        IsEnabled = (k_TRUE_FALSE_ID)CDataUtils.GetDSLongValue(dr, "IS_ENABLE");
        OverrideDate = CDataUtils.GetDSDateTimeValue(dr, "OVERRIDE_DATE");
    }
}
