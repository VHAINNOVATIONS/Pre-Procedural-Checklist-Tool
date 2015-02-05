using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to an temportal state entry
/// </summary>
public class CTemporalStateDataItem
{
    public string TSLabel { get; set; }
    public long TSDefinitionID { get; set; }
    public long TSID { get; set; }

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

    public CTemporalStateDataItem()
    {
    }

    public CTemporalStateDataItem(DataSet ds)
	{
        if (!CDataUtils.IsEmpty(ds))
        {
            TSLabel = CDataUtils.GetDSStringValue(ds, "TS_LABEL");
            TSDefinitionID = CDataUtils.GetDSLongValue(ds, "TS_DEFINITION_ID");
            TSID = CDataUtils.GetDSLongValue(ds, "TS_ID");
            IsActive = (CDataUtils.GetDSLongValue(ds, "IS_ACTIVE") == (long)k_TRUE_FALSE_ID.True) ? true : false;
            IsDefault = (CDataUtils.GetDSLongValue(ds, "IS_DEFAULT") == (long)k_TRUE_FALSE_ID.True) ? true : false;
        }
	}
}
