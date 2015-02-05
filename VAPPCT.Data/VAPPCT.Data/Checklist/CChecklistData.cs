using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Methods that pertain to a checklist
/// </summary>
public class CChecklistData : CData
{
    //Constructor
    public CChecklistData(CData Data)
        : base(Data)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    /// US:864 US:876 gets a loaded checklist permissions data item
    /// </summary>
    /// <param name="lCheckListID"></param>
    /// <param name="cli"></param>
    /// <returns></returns>
    public CStatus GetCheckListPermissionsDI(long lCheckListID,
                                             out CChecklistPermissionsDataItem cli)
    {
        //initialize parameters
        cli = new CChecklistPermissionsDataItem();

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //get the viewable roles
        DataSet dsViewable = null;
        status = GetCLViewableRolesDS(lCheckListID, out dsViewable);
        
        //get the readonly roles
        DataSet dsReadOnly = null;
        status = GetCLReadOnlyRolesDS(lCheckListID, out dsReadOnly);
        
        //get the closeable roles
        DataSet dsCloseable = null;
        status = GetCLCloseableRolesDS(lCheckListID, out dsCloseable);

        //get the TIU roles
        DataSet dsTIU = null;
        status = GetCLTIURolesDS(lCheckListID, out dsTIU);

        //
                
        //load the dataitem from the datasets
        status = cli.LoadRoles(dsViewable, dsReadOnly, dsCloseable, dsTIU);
        
        return status;
    }
   

    /// <summary>
    /// Used to save a checklist as a new checklist with a new ID
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="strNewLabel"></param>
    /// <param name="lNewChecklistID"></param>
    /// <returns></returns>
    public CStatus SaveAs(long lChecklistID,
                          string strNewLabel,
                          out long lNewChecklistID)
    {
        //initialize parameters
        lNewChecklistID = 0;

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
        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_vNewLabel", strNewLabel);
        pList.AddInputParameter("po_nNewChecklistID", lNewChecklistID);
        
        //execute the SP
        status = DBConn.ExecuteOracleSP( "PCK_CHECKLIST.SaveAs", pList);
        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lNewChecklistID = pList.GetParamLongValue("po_nNewChecklistID");
        }

        return status;
    }

    /// <summary>
    /// US:1951 US:1945 User to insert a checkList
    /// </summary>
    /// <param name="cli"></param>
    /// <param name="lChecklistID"></param>
    /// <returns></returns>
    public CStatus InsertChecklist(CChecklistDataItem cli, out long lChecklistID)
    {
        //initialize parameters
        lChecklistID = 0;

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
        //add the rest of the parameters
        pList.AddInputParameter("pi_vChecklistLabel", cli.ChecklistLabel);
        pList.AddInputParameter("pi_nServiceID", cli.ServiceID);
        pList.AddInputParameter("pi_vChecklistDescription", cli.ChecklistDescription);
        pList.AddInputParameter("pi_nIsActive", (long)cli.ActiveID);
        pList.AddInputParameter("pi_vNoteTitleTag", cli.NoteTitleTag);
        pList.AddInputParameter("pi_nNoteTitleClinicID", cli.NoteTitleClinicID);

        pList.AddOutputParameter("po_nChecklistID", lChecklistID);
        
        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_CHECKLIST.InsertChecklist", pList);
        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lChecklistID = pList.GetParamLongValue("po_nChecklistID");
        }

        return status;
    }

    /// <summary>
    /// US:1951 US:1945 Used to update a checkList
    /// </summary>
    /// <param name="cli"></param>
    /// <returns></returns>
    public CStatus UpdateChecklist(CChecklistDataItem cli)
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
        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", cli.ChecklistID);
        pList.AddInputParameter("pi_vChecklistLabel", cli.ChecklistLabel);
        pList.AddInputParameter("pi_nServiceID", cli.ServiceID);
        pList.AddInputParameter("pi_vChecklistDescription", cli.ChecklistDescription);
        pList.AddInputParameter("pi_nActiveID", (long)cli.ActiveID);
        pList.AddInputParameter("pi_vNoteTitleTag", cli.NoteTitleTag);
        pList.AddInputParameter("pi_nNoteTitleClinicID", cli.NoteTitleClinicID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.UpdateChecklist", pList);
    }

    /// <summary>
    /// Used to get a dataset holding 1 item group by id
    /// </summary>
    /// <param name="lCheckListID"></param>
    /// <param name="cli"></param>
    /// <returns></returns>
    public CStatus GetCheckListDI(long lCheckListID, out CChecklistDataItem cli)
    {
        //initialize parameters
        cli = null;
        
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

        pList.AddInputParameter("pi_nCheckListID", lCheckListID);

        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_CHECKLIST.GetCheckListRS",
                                      pList,
                                      out ds);
        if (status.Status)
        {
            cli = new CChecklistDataItem(ds);
        }

        return status;
    }

    /// <summary>
    /// Used to get a dataset of checklists matching search criteria
    /// </summary>
    /// <param name="strChecklistName"></param>
    /// <param name="lServiceID"></param>
    /// <param name="bActiveChecklistsOnly"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetCheckListSearchDS(string strChecklistName,
                                        long lServiceID,
                                        bool bActiveChecklistsOnly,
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

        pList.AddInputParameter("pi_vChecklistName", strChecklistName);
        pList.AddInputParameter("pi_nServiceID", lServiceID);
        pList.AddInputParameter("pi_nActiveChecklistsOnly", (bActiveChecklistsOnly) ? 1 : 0);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                    "PCK_CHECKLIST.GetChecklistSearchRS",
                                    pList,
                                    out ds);
    }

    /// <summary>
    /// Used to get user roles allowed to change an items temporal state
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetChecklistTSChangeableDS(long lChecklistID, out DataSet ds)
    {
        //initialize parameters
        ds = null;
       
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

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                   "PCK_CHECKLIST.GetChecklistTSChangeableRS",
                                   pList,
                                   out ds);
    }


    /// <summary>
    /// Used to get user roles allowed to change an items outcome state
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetChecklistOSChangeableDS(long lChecklistID,
                                            out DataSet ds)
    {
        //initialize parameters
        ds = null;
        
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

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                     "PCK_CHECKLIST.GetChecklistOSChangeableRS",
                                     pList,
                                     out ds);
    }


    /// <summary>
    /// Used to get user roles allowed to change an items decision state
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetChecklistDSChangeableDS(long lChecklistID,
                                            out DataSet ds)
    {
        //initialize parameters
        ds = null;
        
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

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                   "PCK_CHECKLIST.GetChecklistDSChangeableRS",
                                   pList,
                                   out ds);
    }
    
    /// <summary>
    /// Used to get user roles that have access to view a checklist
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetCLViewableRolesDS(long lChecklistID, out DataSet ds)
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
        CParameterList pList = new CParameterList( SessionID,
                                                   ClientIP,
                                                   UserID);

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                   "PCK_CHECKLIST.GetCLViewableRolesRS",
                                   pList,
                                   out ds);
    }

    /// <summary>
    /// Used to get user roles that have read only access to a checklist
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetCLReadOnlyRolesDS(long lChecklistID,
                                      out DataSet ds)
    {
        //initialize parameters
        ds = null;
        
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

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                   "PCK_CHECKLIST.GetCLReadOnlyRolesRS",
                                   pList,
                                   out ds);
    }


   /// <summary>
    /// Used to get user roles that have access to close a checklist
   /// </summary>
   /// <param name="lChecklistID"></param>
   /// <param name="ds"></param>
   /// <param name="lStatusCode"></param>
   /// <param name="strStatus"></param>
   /// <returns></returns>
    public CStatus GetCLCloseableRolesDS(long lChecklistID,
                                       out DataSet ds)
    {
        //initialize parameters
        ds = null;
        
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

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                   "PCK_CHECKLIST.GetCLCloseableRolesRS",
                                   pList,
                                   out ds);
    }

    /// <summary>
    /// Used to get user roles that have access to create a TIU note
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetCLTIURolesDS(long lChecklistID,
                                       out DataSet ds)
    {
        //initialize parameters
        ds = null;

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

        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                   "PCK_CHECKLIST.GetCLTIURolesRS",
                                   pList,
                                   out ds);
    }

    /// <summary>
    /// Used to delete all viewable roles for a checklist to be repopulated
    /// when the checklist is saved.
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus DeleteAllCLViewableRoles(long lChecklistID)
    {
        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList( SessionID,
                                                   ClientIP,
                                                   UserID);
        //add the rest of the parameters
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.DeleteAllCLViewableRoles",
                                      pList);
    }


    /// <summary>
    /// Used to delete all read only roles for a checklist to be repopulated
    /// when the checklist is saved.
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus DeleteAllCLReadOnlyRoles(long lChecklistID)
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
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.DeleteAllCLReadOnlyRoles",
                                      pList); 
    }

    /// <summary>
    /// Used to delete all closeable roles for a checklist to be repopulated
    /// when the checklist is saved.
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus DeleteAllCLCloseableRoles(long lChecklistID)
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
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        
        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.DeleteAllCLCloseableRoles",
                                       pList);
    }

    /// <summary>
    /// removes all CL TIU roles
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <returns></returns>
    public CStatus DeleteAllCLTIURoles(long lChecklistID)
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
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.DeleteAllCLTIURoles",
                                       pList);
    }

    /// <summary>
    /// Used to insert viewable roles for a checklist
    /// </summary>
    /// <param name="cli"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertCLViewableRole(CCLViewableDataItem cli)
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
        pList.AddInputParameter("pi_nChecklistID", cli.ChecklistID);
        pList.AddInputParameter("pi_nUserRoleID", cli.UserRoleID);
       
        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.InsertCLViewableRole",
                                       pList);
    }

    /// <summary>
    /// Used to insert read only roles for a checklist
    /// </summary>
    /// <param name="cli"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertCLReadOnlyRole(CCLReadOnlyDataItem cli)
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
        pList.AddInputParameter("pi_nChecklistID", cli.ChecklistID);
        pList.AddInputParameter("pi_nUserRoleID", cli.UserRoleID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.InsertCLReadOnlyRole",
                                       pList);
    }

    /// <summary>
    /// Used to insert closeable roles for a checklist
    /// </summary>
    /// <param name="cli"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertCLCloseableRole(CCLCloseableDataItem cli)
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
        pList.AddInputParameter("pi_nChecklistID", cli.ChecklistID);
        pList.AddInputParameter("pi_nUserRoleID", cli.UserRoleID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.InsertCLCloseableRole",
                                      pList);
    }

    /// <summary>
    /// inserts a TIU role into the checklist database 
    /// </summary>
    /// <param name="cli"></param>
    /// <returns></returns>
    public CStatus InsertCLTIURole(CCLTIUDataItem cli)
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
        pList.AddInputParameter("pi_nChecklistID", cli.ChecklistID);
        pList.AddInputParameter("pi_nUserRoleID", cli.UserRoleID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_CHECKLIST.InsertCLTIURole",
                                      pList);
    }
}
