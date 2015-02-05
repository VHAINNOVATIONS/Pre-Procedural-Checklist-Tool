using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Properties that pertain to a Patient Checklist
/// </summary>
public class CPatChecklistDataItem
{
    public string PatientID { get; set; }   
    public long ChecklistID { get; set; }   
    public DateTime AssignmentDate { get; set; }   
    public DateTime ProcedureDate { get; set; }   
    public k_STATE_ID StateID { get; set; }   
    public k_CHECKLIST_STATE_ID ChecklistStateID { get; set; }   
    public long PatCLID { get; set; }   

	public CPatChecklistDataItem()
	{
	}

    public CPatChecklistDataItem(DataSet ds)
    {
        if (!CDataUtils.IsEmpty(ds))
        {
            PatientID = CDataUtils.GetDSStringValue(ds, "PATIENT_ID");
            ChecklistID = CDataUtils.GetDSLongValue(ds, "CHECKLIST_ID");
            AssignmentDate = CDataUtils.GetDSDateTimeValue(ds, "ASSIGNMENT_DATE");
            ProcedureDate = CDataUtils.GetDSDateTimeValue(ds, "PROCEDURE_DATE");
            StateID = (k_STATE_ID)CDataUtils.GetDSLongValue(ds, "STATE_ID");
            ChecklistStateID = (k_CHECKLIST_STATE_ID)CDataUtils.GetDSLongValue(ds, "CHECKLIST_STATE_ID");
            PatCLID = CDataUtils.GetDSLongValue(ds, "PAT_CL_ID");
        }
    }
}
