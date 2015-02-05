using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an outcome state entry
/// </summary>
public class COutcomeStateDataItem
{
    public string OSLabel { get; set; }
    public long OSDefinitionID { get; set; }
    public long OSID { get; set; }

    private k_TRUE_FALSE_ID m_lIsActive;
    public bool IsActive
    {
        get { return (m_lIsActive == k_TRUE_FALSE_ID.True) ? true : false; }
        set { m_lIsActive = (value == true) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False; }
    }

    private k_TRUE_FALSE_ID m_lIsDefault;
    public bool IsDefault
    {
        get { return (m_lIsDefault == k_TRUE_FALSE_ID.True) ? true : false; }
        set { m_lIsDefault = (value == true) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False; }
    }

    public COutcomeStateDataItem()
    {
    }

    public COutcomeStateDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            OSLabel = CDataUtils.GetDSStringValue(ds, "OS_LABEL");
            OSDefinitionID = CDataUtils.GetDSLongValue(ds, "OS_DEFINITION_ID");
            OSID = CDataUtils.GetDSLongValue(ds, "OS_ID");
            IsActive = (CDataUtils.GetDSLongValue(ds, "IS_ACTIVE") == (long)k_TRUE_FALSE_ID.True) ? true : false;
            IsDefault = (CDataUtils.GetDSLongValue(ds, "IS_DEFAULT") == (long)k_TRUE_FALSE_ID.True) ? true : false;
        }
    }
}
