using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CPatientDataItem
/// </summary>
public class CPatientDataItem
{
    public string PatientID { get; set; }
    public string FirstName { get; set; }
    public string FullName { get; set; }
    public string MI { get; set; }
    public string LastName { get; set; }
    public string SSN { get; set; }
    public string SSNLast4 { get; set; }
    public DateTime DOB { get; set; }
    public k_SEX Sex { get; set; }
    public string SexLabel { get; set; }
    public string SexAbbreviation { get; set; }
    public long Age { get; set; }

    public CPatientDataItem()
	{

    }

    public CPatientDataItem(DataSet ds)
    {
        if(!CDataUtils.IsEmpty(ds))
        {
            PatientID = CDataUtils.GetDSStringValue(ds, "PATIENT_ID");
            FirstName = CDataUtils.GetDSStringValue(ds, "FIRST_NAME");
            MI = CDataUtils.GetDSStringValue(ds, "MIDDLE_INITIAL");
            LastName = CDataUtils.GetDSStringValue(ds, "LAST_NAME");
            SSN = CDataUtils.GetDSStringValue(ds, "SSN");
            DOB = CDataUtils.GetDSDateTimeValue(ds, "DATE_OF_BIRTH");
            Sex = (k_SEX)CDataUtils.GetDSLongValue(ds, "SEX_ID");
            SexLabel = CDataUtils.GetDSStringValue(ds, "SEX_LABEL");
            SexAbbreviation = CDataUtils.GetDSStringValue(ds, "SEX_ABBREVIATION");
            SSNLast4 = CDataUtils.GetDSStringValue(ds, "SSN_LAST_4");
            Age = CDataUtils.GetDSLongValue(ds, "PATIENT_AGE");
        }
    }
}
