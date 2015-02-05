using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

public class CPlaceHolderDataItem
{
    public long PlaceHolderID { get; set; }
    public string PlaceHolderLabel { get; set; }
    public string PlaceHolderSyntax { get; set; }
    public long PHParentID { get; set; }
    public bool IsGroup { get; set; }

    public CPlaceHolderDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            PlaceHolderID = CDataUtils.GetDSLongValue(ds, "PLACE_HOLDER_ID");
            PlaceHolderLabel = CDataUtils.GetDSStringValue(ds, "PLACE_HOLDER_LABEL");
            PlaceHolderSyntax = CDataUtils.GetDSStringValue(ds, "PLACE_HOLDER_SYNTAX");
            PHParentID = CDataUtils.GetDSLongValue(ds, "PH_PARENT_ID");
            IsGroup = (CDataUtils.GetDSLongValue(ds, "IS_GROUP") == (long)k_TRUE_FALSE_ID.True) ? true : false;
        }
    }
}
