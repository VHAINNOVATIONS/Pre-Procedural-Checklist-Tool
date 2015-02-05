using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;


/// <summary>
/// Methods that pertain to a Patient Checklist
/// </summary>
public class CPatChecklistData : CData
{
    //Constructor
    public CPatChecklistData(CData Data)
        : base(Data)
	{

	}

    /// <summary>
    /// Generates the TIU text for a patient checklist
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatCLID"></param>
    /// <param name="strText"></param>
    /// <returns></returns>
    public CStatus GetTIUText(string strPatientID,
                              long lPatCLID,
                              out string strNoteTitleTag,
                              out string strText)
    {
        strText = string.Empty;
        strNoteTitleTag = string.Empty;

        CStatus status = new CStatus();

        //patient data - get the di just in case we need more than the blurb
        CPatientDataItem diPat = new CPatientDataItem();
        CPatientData patData = new CPatientData(this);
        status = patData.GetPatientDI(strPatientID, out diPat);

        //get the patient blurb
        string strBlurb = String.Empty;
        patData.GetPatientBlurb(strPatientID, out strBlurb);
                
        //build the TIU note text...
        
        //legend
        strText += "Definitions:\r\n";
        
        //ts
        strText += CDataUtils.DelimitString("TS = The temporal state of an attribute defines whether the patient has had the test or event within a given time period", 
                                            "\r\n", 
                                            80);
        strText += "\r\n";

        //os
        strText += CDataUtils.DelimitString("OS = The outcome state of an attribute defines the resultant state of an attribute (e.g. normal, abnormal, problem/decision required)",
                                            "\r\n",
                                            80);
        strText += "\r\n";

        //ds
        strText += CDataUtils.DelimitString("DS = The decision state of an attribute defines a rule-based state of an attribute (e.g. Go, No-Go)",
                                            "\r\n",
                                            80);

        strText += "\r\n";


        strText += "\r\n";

        DateTime dtNoteDate = DateTime.Now;
        string strNoteDate = CDataUtils.GetDateTimeAsString(dtNoteDate);

        strText += "Date: " + strNoteDate;
        strText += "\r\n\r\n";

        //--demographics
        strText += CDataUtils.DelimitString(strBlurb, "\r\n", 80);
        strText += "\r\n";

        //patient checklist data
        CPatChecklistDataItem diPatChecklist = new CPatChecklistDataItem();
        status = GetPatChecklistDI(lPatCLID, out diPatChecklist);

        //checklist data
        CChecklistDataItem diChecklist = new CChecklistDataItem();
        CChecklistData clData = new CChecklistData(this);
        status = clData.GetCheckListDI(diPatChecklist.ChecklistID, out diChecklist);

        //get the note title tag for the checklist, this is used to
        //write the correct note to MDWS
        strNoteTitleTag = diChecklist.NoteTitleTag;

        //--Checklist Name
        strText += "Checklist: ";
        strText += CDataUtils.DelimitString(diChecklist.ChecklistLabel, "\r\n", 80);
        strText += "\r\n";

        //--Procedure Date
        strText += "Procedure Date: ";
        if (!CDataUtils.IsDateNull(diPatChecklist.ProcedureDate))
        {
            strText += CDataUtils.GetDateAsString(diPatChecklist.ProcedureDate);
        }
        else
        {
            strText += "None";
        }
        strText += "\r\n\r\n";
        
        //patient checklist items and overall state
        long lColTSStateID = 0;
        long lColOSStateID = 0;
        long lColDSStateID = 0;
        long lSummaryStateID = 0;
        DataSet dsItems = null;
        CPatChecklistItemData diCLI = new CPatChecklistItemData(this);
        status = diCLI.GetPatCLItemsByPatCLIDDS(lPatCLID,
                                                out lColTSStateID,
                                                out lColOSStateID,
                                                out lColDSStateID,
                                                out lSummaryStateID,
                                                out dsItems);
        //--overall Checklist state
        string strOverallState = "Unknown";
        switch (lSummaryStateID)
        {
            case (long)k_STATE_ID.Bad:
                strOverallState = "Bad";
                break;
            case (long)k_STATE_ID.Good:
                strOverallState = "Good";
                break;
        }

        strText += "Overall Checklist State: ";
        strText += strOverallState;
        strText += "\r\n\r\n";

        strText += "Checklist Items:";
        strText += "\r\n\r\n";

        //loop over checklist items
        foreach (DataTable table in dsItems.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                CPatChecklistItemDataItem itm = new CPatChecklistItemDataItem(dr);
                if (itm != null)
                {
                    //get the data for the item
                    CItemDataItem idi = new CItemDataItem();
                    CItemData itmData = new CItemData(this);

                    itmData.GetItemDI(itm.ItemID, out idi);
                    strText += CDataUtils.DelimitString("* " + idi.ItemLabel, "\r\n", 80);
                    strText += "\r\n";
                                           
                    //temporal state
                    CTemporalStateDataItem diTSi = new CTemporalStateDataItem();
                    CTemporalStateData tsdi = new CTemporalStateData(this);
                    tsdi.GetTemporalStateDI(itm.TSID, out diTSi);
                    strText += "TS: ";
                    strText += CDataUtils.DelimitString(diTSi.TSLabel, "\r\n", 80);
                    strText += "  ";

                    //outcome state
                    COutcomeStateDataItem diOSi = new COutcomeStateDataItem();
                    COutcomeStateData osdi = new COutcomeStateData(this);
                    osdi.GetOutcomeStateDI(itm.OSID, out diOSi);
                    strText += "OS: ";
                    strText += CDataUtils.DelimitString(diOSi.OSLabel, "\r\n", 80);
                    strText += " ";

                    //decision state
                    CDecisionStateDataItem diDSi = new CDecisionStateDataItem();
                    CDecisionStateData dsdi = new CDecisionStateData(this);
                    dsdi.GetDecisionStateDI(itm.DSID, out diDSi);

                    string strDS = String.Empty;
                    strDS += "DS: ";
                    strDS += diDSi.DSLabel;
                    
                    //if decision state is overriden pull out the 
                    //last comment 
                    if (itm.IsOverridden == k_TRUE_FALSE_ID.True)
                    {
                        DataSet dsComments = null;
                        
                        //todo: override history is now stored in a diff table
                        //this is obsolete will delete after testing
                        //status = diCLI.GetPatientItemCommmentDS(
                        //    itm.PatCLID,
                        //    itm.ItemID,
                        //    out dsComments);

                        status = diCLI.GetPatItemOverrideCommmentDS(itm.PatCLID,
                                                                    itm.ChecklistID,
                                                                    itm.ItemID,
                                                                    out dsComments);
                        //first record is the newest comment
                        if (!CDataUtils.IsEmpty(dsComments))
                        {
                            //string strComment = CDataUtils.GetDSStringValue(dsComments, "comment_text");
                            //DateTime dtComment = CDataUtils.GetDSDateTimeValue(dsComments, "comment_date");
                            //
                            string strComment = CDataUtils.GetDSStringValue(dsComments, "override_comment");
                            DateTime dtComment = CDataUtils.GetDSDateTimeValue(dsComments, "override_date");
                            long lCommentUserID = CDataUtils.GetDSLongValue(dsComments, "user_id");

                            DataSet dsUser = null;
                            CUserData ud = new CUserData(this);
                            ud.GetUserDS(lCommentUserID, out dsUser);
                            string strUser = String.Empty;
                            if (!CDataUtils.IsEmpty(dsUser))
                            {
                                strUser = CDataUtils.GetDSStringValue(dsUser, "name");
                            }

                            strDS += " Overridden ";
                            strDS += CDataUtils.GetDateAsString(dtComment);
                            strDS += " ";
                            strDS += strUser;
                            strDS += "\r\n\r\n";

                            strDS += strComment;
                        }
                    }

                    //ds
                    strText += CDataUtils.DelimitString(strDS, "\r\n", 80);

                    strText += "\r\n\r\n";
                }
            }
        }
        
        return status;
    }

    /// <summary>
    /// select
    /// returns a ds of patient checklists for the specified patient
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatChecklistDS(string strPatientID, out DataSet ds)
    {
        //initialize parameters
        ds = null;
      
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);
        pList.AddInputParameter("pi_vPatientID", strPatientID);

        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_PAT_CHECKLIST.GetPatChecklistRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// method
    /// returns a dataset of all open checklists for a patient that contain the item specified
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatChecklistDS(string strPatientID, long lItemID, out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                     "PCK_PAT_CHECKLIST.GetPatChecklistByItemRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// get a list of patient checklist that are out of date
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetOutOfDatePatCLDS(string strPatientID, 
                                        long lChecklistID, 
                                        out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
      
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                     "PCK_PAT_CHECKLIST.GetOutOfDatePatCLRS",
                                     pList,
                                     out ds);
    }


    /// <summary>
    /// get a list of patient checklist that are out of date
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetOutOfDatePatCLDS( DateTime dtEventStart,
                                        DateTime dtEventEnd,
                                        long lChecklistID,
                                        long lChecklistStatusID,
                                        string strPatIDs,
                                        string strCLIDs,
                                        out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);

        pList.AddInputParameter("pi_dtEventStartDate", dtEventStart);
        pList.AddInputParameter("pi_dtEventEndDate", dtEventEnd);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nChecklistStatusID", lChecklistStatusID);
        pList.AddInputParameterCLOB("pi_vPatIDs", strPatIDs);
        pList.AddInputParameterCLOB("pi_vCLIDs", strCLIDs);
        
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                     "PCK_PAT_CHECKLIST.GetOutOfDateMultiPatCLRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// updates the patient checklist to the latest version
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <returns></returns>
    public CStatus UpdatePatCLVersion(long lPatCLID)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);

        pList.AddInputParameter("pi_nPatCLID", lPatCLID);

        return DBConn.ExecuteOracleSP( "PCK_PAT_CHECKLIST.UpdatePatCLVersion",
                                       pList);
    }

    /// <summary>
    /// Used to get a dataset holding 1 patient checklist
    /// by Patient Checklist ID
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="pci"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatChecklistDI(long lPatCLID, out CPatChecklistDataItem di)
    {
        //initialize parameters
        di = null;
       
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);
        pList.AddInputParameter("pi_nPatCLID", lPatCLID);

        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet( DBConn,
                                       "PCK_PAT_CHECKLIST.GetPatChecklistIDRS",
                                       pList,
                                       out ds);

        if(!status.Status)
        {
            return status;
        }

        di = new CPatChecklistDataItem(ds);

        return status;
    }

    /// <summary>
    /// method
    /// US:877
    /// get patient checklist by CLID
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lChecklistStateID"></param>
    /// <param name="strSelectedPatients"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatCLByCLIDCLSTATEDS(long lChecklistID,
                                           long lChecklistStateID,
                                           string strSelectedPatients,
                                           out DataSet ds)
    {
        //initialize parameters
        ds = null;
        
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nChecklistStateID", lChecklistStateID);
        pList.AddInputParameter("pi_vSelectedPatients", strSelectedPatients);


        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                   "PCK_PAT_CHECKLIST.GetPatCLByCLIDCLSTATERS",
                                   pList,
                                   out ds);
    }

    public CStatus HasPatientChecklist(
        long lChecklistID,
        long lChecklistStateID,
        string strPatientID,
        out bool bHasPatCL)
    {
        //initialize parameters
        bHasPatCL = false;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(
            SessionID,
            ClientIP,
            UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nChecklistStateID", lChecklistStateID);
        pList.AddInputParameter("pi_vPatientID", strPatientID);

        long lHasPatCL = 0;
        pList.AddOutputParameter("po_nHasPatCL", lHasPatCL);

        //get the dataset
        status = DBConn.ExecuteOracleSP("PCK_PAT_CHECKLIST.HasPatientChecklist", pList);
        if (!status.Status)
        {
            return status;
        }

        //get the TS_ID returned from the SP call
        bHasPatCL = (pList.GetParamLongValue("po_nHasPatCL") > 0) ? true : false;

        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:838
    /// Used to insert a pat checkList
    /// </summary>
   /// <param name="pci"></param>
   /// <param name="lPatCLID"></param>
   /// <returns></returns>
    public CStatus InsertPatChecklist(CPatChecklistDataItem pci, out long lPatCLID)
    {
        //initialize parameters
        lPatCLID = 0;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        //add the rest of the parameters
        pList.AddInputParameter("pi_vPatientID", pci.PatientID);
        pList.AddInputParameter("pi_nChecklistID", pci.ChecklistID);
        pList.AddInputParameter("pi_dtAssignmentDate", pci.AssignmentDate);
        pList.AddInputParameter("pi_dtProcedureDate", pci.ProcedureDate);
        pList.AddInputParameter("pi_nChecklistStateID", (long)pci.ChecklistStateID);
        pList.AddInputParameter("pi_nStateID", (long)pci.StateID);
        pList.AddOutputParameter("po_nPatCLID", lPatCLID);
        
        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_PAT_CHECKLIST.InsertPatChecklist", pList);
        if (!status.Status)
        {
            return status;
        }

        //get the TS_ID returned from the SP call
        lPatCLID = pList.GetParamLongValue("po_nPatCLID");

        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:878
    /// Used to update a pat checkList
    /// </summary>
    /// <param name="pci"></param>
    /// <returns></returns>
    public CStatus UpdatePatChecklist(CPatChecklistDataItem pci)
    {
        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //add the rest of the parameters
        pList.AddInputParameter("pi_nPatCLID", pci.PatCLID);
        pList.AddInputParameter("pi_dtAssignmentDate", pci.AssignmentDate);
        pList.AddInputParameter("pi_dtProcedureDate", pci.ProcedureDate);
        pList.AddInputParameter("pi_nChecklistStateID", (long)pci.ChecklistStateID);
        pList.AddInputParameter("pi_nStateID", (long)pci.StateID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_PAT_CHECKLIST.UpdatePatChecklist",
                                       pList);
    }
}
