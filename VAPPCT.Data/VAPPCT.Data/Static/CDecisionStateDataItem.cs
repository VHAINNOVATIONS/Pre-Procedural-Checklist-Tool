using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an decision state entry
/// </summary>
public class CDecisionStateDataItem
{
    public string DSLabel { get; set; }
    public long DSDefinitionID { get; set; }
    public long DSID { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }

    public CDecisionStateDataItem()
    {
    }

    public CDecisionStateDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            DSLabel = CDataUtils.GetDSStringValue(ds, "DS_LABEL");
            DSDefinitionID = CDataUtils.GetDSLongValue(ds, "DS_DEFINITION_ID");
            DSID = CDataUtils.GetDSLongValue(ds, "DS_ID");
            IsActive = (CDataUtils.GetDSLongValue(ds, "IS_ACTIVE") == (long)k_TRUE_FALSE_ID.True) ? true : false;
            IsDefault = (CDataUtils.GetDSLongValue(ds, "IS_DEFAULT") == (long)k_TRUE_FALSE_ID.True) ? true : false;
        }
    }
}
