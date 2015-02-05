using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Resources;
using VAPPCT.DA;

/// <summary>
/// Methods that pertain to a checklist item
/// </summary>
public class CChecklistItemData : CData
{
    //Constructor
    public CChecklistItemData(CData Data)
        : base(Data)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    /// Used to get a dataset of temporal states for a particular
    /// checklist/item combination
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetTemporalStateDS(
        long lChecklistID,
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
        CParameterList pList = new CParameterList(
            SessionID,
            ClientIP,
            UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_CHECKLIST_ITEM.GetTemporalStateRS",
            pList,
            out ds);
    }

    /// <summary>
    /// Used to get a dataset of outcome states for a particular
    /// checklist/item combination
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetOutcomeStateDS(
        long lChecklistID,
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
        CParameterList pList = new CParameterList(
            SessionID,
            ClientIP,
            UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_CHECKLIST_ITEM.GetOutcomeStateRS",
            pList,
            out ds);
    }

    /// <summary>
    /// Used to get a dataset of decision states for a particular
    /// checklist/item combination
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetDecisionStateDS(
        long lChecklistID,
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
        CParameterList pList = new CParameterList(
            SessionID,
            ClientIP,
            UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_CHECKLIST_ITEM.GetDecisionStateRS",
            pList,
            out ds);
    }

    /// <summary>
    /// Used to save the items selected temporal states
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strTSIDs"></param>
    /// <param name="lTSIDCount"></param>
    /// <returns></returns>
    public CStatus SaveTemporalStates(
        long lChecklistID,
        long lItemID,
        string strTSIDs,
        long lTSIDCount)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vTSIDs", strTSIDs);
        pList.AddInputParameter("pi_nTSCount", lTSIDCount);
        
        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST_ITEM.SaveTemporalStates", pList);
    }

    /// <summary>
    /// Used to save the items selected outcome states
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strOSIDs"></param>
    /// <param name="lOSIDCount"></param>
    /// <returns></returns>
    public CStatus SaveOutcomeStates(
        long lChecklistID,
        long lItemID,
        string strOSIDs,
        long lOSIDCount)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vOSIDs", strOSIDs);
        pList.AddInputParameter("pi_nOSCount", lOSIDCount);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST_ITEM.SaveOutcomeStates", pList);
    }

    /// <summary>
    /// Used to save the items selected decision states
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strDSIDs"></param>
    /// <param name="lDSIDCount"></param>
    /// <returns></returns>
    public CStatus SaveDecisionStates(
        long lChecklistID,
        long lItemID,
        string strDSIDs,
        long lDSIDCount)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vDSIDs", strDSIDs);
        pList.AddInputParameter("pi_nDSCount", lDSIDCount);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST_ITEM.SaveDecisionStates", pList);
    }

    /// <summary>
    /// Used to get a checklists item entries
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetChecklistItemsDS(long lChecklistID, out DataSet ds)
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
        CParameterList pList = new CParameterList(
            SessionID,
            ClientIP,
            UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_CHECKLIST_ITEM.GetChecklistItemsRS",
            pList,
            out ds);
    }

    /// <summary>
    /// method
    /// retrieves a single checklist item from the database
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus GetCLItemDI(
        long lChecklistID,
        long lItemID,
        out CChecklistItemDataItem di)
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
        CParameterList pList = new CParameterList(
            SessionID,
            ClientIP,
            UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_CHECKLIST_ITEM.GetCLItemDIRS",
            pList,
            out ds);

        if (status.Status)
        {
            di = new CChecklistItemDataItem(ds);
        }

        return status;
    }

    /// <summary>
    /// US:865 gets a checklist item permission data item
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="diPermissions"></param>
    /// <returns></returns>
    public CStatus GetCLPermissionItem(
        long lChecklistID,
        long lItemID,
        out CChecklistItemPermissionsDataItem diPermissions)
    {
        diPermissions = new CChecklistItemPermissionsDataItem();
        DataSet dsRoles = null;
        CStatus status = GetCLItemDSRolesDS(
            lChecklistID,
            lItemID,
            out dsRoles);
        if (!status.Status)
        {
            return status;
        }

        status = diPermissions.LoadRoles(dsRoles);

        return status;
    }

    /// <summary>
    /// Used to get the user roles that have access to the decision state
    /// of a checklist item
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetCLItemDSRolesDS(
        long lChecklistID,
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
        CParameterList pList = new CParameterList(
            SessionID,
            ClientIP,
            UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_CHECKLIST_ITEM.GetCLItemDSRolesRS",
            pList,
            out ds);
    }

    
    /// <summary>
    /// Used to insert a checklist item
    /// </summary>
    /// <param name="cli"></param>
    /// <returns></returns>
    public CStatus InsertChecklistItem(CChecklistItemDataItem cli)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", cli.ChecklistID);
        pList.AddInputParameter("pi_nItemID", cli.ItemID);
        pList.AddInputParameter("pi_nTimePeriod", cli.CLITSTimePeriod);
        pList.AddInputParameter("pi_nTimeUnit", (long)cli.TimeUnitID);
        pList.AddInputParameter("pi_nSortOrder", cli.SortOrder);
        pList.AddInputParameter("pi_nIsActive", (long)cli.ActiveID);
        pList.AddInputParameter("pi_vLogic", cli.Logic);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST_ITEM.InsertChecklistItem", pList);
    }

    /// <summary>
    /// Used to update a checklist item
    /// </summary>
    /// <param name="cli"></param>
    /// <returns></returns>
    public CStatus UpdateChecklistItem(CChecklistItemDataItem cli)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", cli.ChecklistID);
        pList.AddInputParameter("pi_nItemID", cli.ItemID);
        pList.AddInputParameter("pi_nTimePeriod", cli.CLITSTimePeriod);
        pList.AddInputParameter("pi_nTimeUnit", (long)cli.TimeUnitID);
        pList.AddInputParameter("pi_nSortOrder", cli.SortOrder);
        pList.AddInputParameter("pi_nIsActive", (long)cli.ActiveID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST_ITEM.UpdateChecklistItem", pList);
    }

    public CStatus UpdateChecklistItemLogic(
        long lChecklistID,
        long lItemID,
        string strLogic)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vLogic", strLogic);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST_ITEM.UpdateChecklistItemLogic", pList);
    }

    /// <summary>
    /// Used to insert a decision state role that has access to a checklist/item 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public CStatus InsertCLItemDSRole(CCLIDSEditDataItem data)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", data.ChecklistID);
        pList.AddInputParameter("pi_nItemID", data.ItemID);
        pList.AddInputParameter("pi_nUserRoleID", data.UserRoleID);

        //execute the SP
        return DBConn.ExecuteOracleSP( "PCK_CHECKLIST_ITEM.InsertCLItemDSRole", pList);
    }

    /// <summary>
    /// Used to delete all checklist item decision state roles
    /// to be repopulated on save.
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus DeleteAllCLItemDSRoles(long lChecklistID, long lItemID)
    {
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

        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST_ITEM.DeleteAllCLItemDSRoles", pList);
    }
}