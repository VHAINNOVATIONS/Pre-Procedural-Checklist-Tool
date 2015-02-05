using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CPatientItemComponentDataItem
/// </summary>
public class CPatientItemComponentDataItem
{
    public string PatientID { get; set; }
    public long PatItemID { get; set; }
    public long ItemID { get; set; }
    public long ComponentID { get; set; }
    public string ComponentValue { get; set; }
    public DateTime EntryDate { get; set; }
    public long SourceTypeID { get; set; }

	public CPatientItemComponentDataItem()
	{
    }

    public CPatientItemComponentDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            PatientID = CDataUtils.GetDSStringValue(ds, "PATIENT_ID");
            EntryDate = CDataUtils.GetDSDateTimeValue(ds, "ENTRY_DATE");
            ItemID = CDataUtils.GetDSLongValue(ds, "ITEM_ID");
            PatItemID = CDataUtils.GetDSLongValue(ds, "PAT_ITEM_ID");
            SourceTypeID = CDataUtils.GetDSLongValue(ds, "SOURCE_TYPE_ID");
            ComponentValue = CDataUtils.GetDSStringValue(ds, "COMPONENT_VALUE");
            ComponentID = CDataUtils.GetDSLongValue(ds, "ITEM_COMPONENT_ID");
        }
    }

    /// <summary>
    /// loads the component from a data row
    /// </summary>
    /// <param name="dr"></param>
    /// <returns></returns>
    public CStatus Load(DataRow dr)
    {
        if (dr == null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "Could not load data row!");
        }

        PatientID = CDataUtils.GetDSStringValue(dr, "PATIENT_ID");
        EntryDate = CDataUtils.GetDSDateTimeValue(dr, "ENTRY_DATE");
        ItemID = CDataUtils.GetDSLongValue(dr, "ITEM_ID");
        PatItemID = CDataUtils.GetDSLongValue(dr, "PAT_ITEM_ID");
        SourceTypeID = CDataUtils.GetDSLongValue(dr, "SOURCE_TYPE_ID");
        ComponentValue = CDataUtils.GetDSStringValue(dr, "COMPONENT_VALUE");
        ComponentID = CDataUtils.GetDSLongValue(dr, "ITEM_COMPONENT_ID");

        return new CStatus();
    }
}
