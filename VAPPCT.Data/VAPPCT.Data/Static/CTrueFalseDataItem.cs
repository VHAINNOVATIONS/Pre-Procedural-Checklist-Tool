using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CTrueFalseDataItem
/// </summary>
public class CTrueFalseDataItem
{
    public long TrueFalseID { get; set; }
    public string TrueLabel { get; set; }
    public string ActiveLabel { get; set; }
    public string DefaultLabel { get; set; }
    public string YesLabel { get; set; }
    public string EnableLabel { get; set; }
    public string OverrideLabel { get; set; }
    public string SelectedLabel { get; set; }

    public CTrueFalseDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            TrueFalseID = CDataUtils.GetDSLongValue(ds, "TRUE_FALSE_ID");
            TrueLabel = CDataUtils.GetDSStringValue(ds, "TRUE_LABEL");
            ActiveLabel = CDataUtils.GetDSStringValue(ds, "ACTIVE_LABEL");
            DefaultLabel = CDataUtils.GetDSStringValue(ds, "DEFAULT_LABEL");
            YesLabel = CDataUtils.GetDSStringValue(ds, "YES_LABEL");
            EnableLabel = CDataUtils.GetDSStringValue(ds, "ENABLE_LABEL");
            OverrideLabel = CDataUtils.GetDSStringValue(ds, "OVERRIDE_LABEL");
            SelectedLabel = CDataUtils.GetDSStringValue(ds, "SELECTED_LABEL");
        }
    }
}
