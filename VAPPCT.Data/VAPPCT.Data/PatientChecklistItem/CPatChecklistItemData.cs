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
/// Methods that pertain to a Patient Checklist Item
/// </summary>
public class CPatChecklistItemData : CData
{
    /// <summary>
    /// constructor
    /// initializes base class
    /// </summary>
    /// <param name="data"></param>
    public CPatChecklistItemData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

    /// <summary>
    /// US:912
    /// method
    /// inserts a comment for a patient item
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strCommentText"></param>
    /// <returns></returns>
    public CStatus InsertPatientItemComment(long lPatCLID,
                                            long lChecklistID,
                                            long lItemID,
                                            string strCommentText)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);
        //add the rest of the parameters
        pList.AddInputParameter("pi_nPatCLID", lPatCLID);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vCommentText", strCommentText);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_PAT_CHECKLIST_ITEM.InsertPatCLIComment", pList);
    }

    /// <summary>
    /// gets all the override comments for an item  
    /// </summary>
    /// <param name="lPatItemID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatItemOverrideCommmentDS(long lPatChecklistID,
                                                long lChecklistID,
                                                long lItemID,
                                                out DataSet ds)
    {
        //status information returned from the call
        CStatus status = new CStatus();

        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nPatCLID", lPatChecklistID);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetPatItemOverrideCommentRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }

        return status;
    }

    /// <summary>
    /// US:912
    /// method
    /// gets all the comments for a patient item
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatientItemCommmentDS(long lPatCLID,
                                            long lItemID,
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
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);
        pList.AddInputParameter("pi_nPatCLID", lPatCLID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                       "PCK_PAT_CHECKLIST_ITEM.GetPatCLICommentRS",
                                       pList,
                                       out ds);
    }

    /// <summary>
    /// method
    /// updates the matching item in the database with the values of the item passed in
    /// </summary>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus UpdatePatChecklistItem(CPatChecklistItemDataItem di)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        //add the rest of the parameters
        pList.AddInputParameter("pi_vPatientID", di.PatientID);
        pList.AddInputParameter("pi_nChecklistID", di.ChecklistID);
        pList.AddInputParameter("pi_nItemID", di.ItemID);
        pList.AddInputParameter("pi_nOSID", di.OSID);
        pList.AddInputParameter("pi_nDSID", di.DSID);
        pList.AddInputParameter("pi_nTSID", di.TSID);
        pList.AddInputParameter("pi_nPatCLID", di.PatCLID);
        pList.AddInputParameter("pi_nIsOverridden", (long)di.IsOverridden);
        pList.AddInputParameter("pi_nIsEnabled", (long)di.IsEnabled);
        pList.AddInputParameter("pi_dtOverrideDate", di.OverrideDate);
        
        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_PAT_CHECKLIST_ITEM.UpdatePatChecklistItem", pList);
    }

    /// <summary>
    /// override DS state on a pat checklist item
    /// </summary>
    /// <param name="di"></param>
    /// <param name="strComment"></param>
    /// <returns></returns>
    public CStatus OverridePatChecklistItem(CPatChecklistItemDataItem di, string strComment)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        //add the rest of the parameters
        pList.AddInputParameter("pi_vPatientID", di.PatientID);
        pList.AddInputParameter("pi_nChecklistID", di.ChecklistID);
        pList.AddInputParameter("pi_nItemID", di.ItemID);
        pList.AddInputParameter("pi_nOSID", di.OSID);
        pList.AddInputParameter("pi_nDSID", di.DSID);
        pList.AddInputParameter("pi_nTSID", di.TSID);
        pList.AddInputParameter("pi_nPatCLID", di.PatCLID);
        pList.AddInputParameter("pi_nIsOverridden", (long)di.IsOverridden);
        pList.AddInputParameter("pi_nIsEnabled", (long)di.IsEnabled);
        pList.AddInputParameter("pi_dtOverrideDate", di.OverrideDate);
        pList.AddInputParameter("pi_vOverrideComment", strComment);
        
        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_PAT_CHECKLIST_ITEM.OverridePatChecklistItem", pList);
    }

    /// <summary>
    /// returns a dataset of patient checklist items
    /// filtered by patient checklist id (PAT_CL_ID)
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatCLItemsByPatCLIDDS(long lPatCLID,
                                            out long lColTSStateID,
                                            out long lColOSStateID,
                                            out long lColDSStateID,
                                            out long lSummaryStateID,
                                            out DataSet ds)
    {
        //initialize parameters
        lColTSStateID = 0;
        lColOSStateID = 0;
        lColDSStateID = 0;
        lSummaryStateID = 0;
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        long lOutColTSStateID = 0;
        long lOutColOSStateID = 0;
        long lOutColDSStateID = 0;
        long lOutSummaryStateID = 0;
        pList.AddInputParameter("pi_nPatCLID", lPatCLID);
        pList.AddOutputParameter("po_nColTSStateID", lOutColTSStateID);
        pList.AddOutputParameter("po_nColOSStateID", lOutColOSStateID);
        pList.AddOutputParameter("po_nColDSStateID", lOutColDSStateID);
        pList.AddOutputParameter("po_nSummaryStateID", lOutSummaryStateID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_PAT_CHECKLIST_ITEM.GetPatCLItemsByPatCLIDRS",
                                      pList,
                                      out ds);

        if (status.Status)
        {
            //get the out params from the SP call
            lColTSStateID = pList.GetParamLongValue("po_nColTSStateID");
            lColOSStateID = pList.GetParamLongValue("po_nColOSStateID");
            lColDSStateID = pList.GetParamLongValue("po_nColDSStateID");
            lSummaryStateID = pList.GetParamLongValue("po_nSummaryStateID");
        }

        return status;
    }

    /// <summary>
    /// returns a dataset of patient checklist items
    /// filtered by patient checklist id (PAT_CL_ID)
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatCLItemsByPatCLIDDS(long lPatCLID,
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
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_nPatCLID", lPatCLID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_PAT_CHECKLIST_ITEM.GetPatCLItemsByPatCLIDRS2",
                                      pList,
                                      out ds);

        if (!status.Status)
        {
            return status;
        }

        return status;
    }

    /// <summary>
    /// get checklist item data item
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus GetPatCLItemDI(long lPatCLID,
                                  long lItemID,
                                  out CPatChecklistItemDataItem di)
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
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet( DBConn,
                                       "PCK_PAT_CHECKLIST_ITEM.GetPatCLItemDIRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new CPatChecklistItemDataItem(ds);
        
        return status;
    }
}
