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
/// Summary description for CPatientItemData
/// </summary>
public class CPatientItemData : CData
{
    public CPatientItemData(CData Data)
        : base(Data)
	{
	}

    public CStatus GetMostRecentPICSummaryStateID(
        string strPatientID,
        long lItemID,
        out long lSummaryStateID)
    {
        //initialize parameters
        lSummaryStateID = -1;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(
            base.SessionID,
            base.ClientIP,
            base.UserID);

        //add the rest of the parameters
        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddOutputParameter("po_nSummaryStateID", lSummaryStateID);

        //execute the SP
        status = base.DBConn.ExecuteOracleSP("PCK_PAT_ITEM.GetMostRecentPICSummaryStateID", pList);
        if (!status.Status)
        {
            return status;
        }

        //get the ID returned from the SP call
        lSummaryStateID = pList.GetParamLongValue("po_nSummaryStateID");
        return status;
    }

    /// <summary>
    /// insert a patient item
    /// </summary>
    /// <param name="di"></param>
    /// <param name="lPatItemID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    private CStatus InsertPatientItem(CPatientItemDataItem di, out long lPatItemID)
    {
        //initialize parameters
        lPatItemID = -1;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        
        //load the paramaters list
        CParameterList pList = new CParameterList( base.SessionID,
                                                   base.ClientIP,
                                                   base.UserID);
        //add the rest of the parameters
        pList.AddInputParameter("pi_vPatientID", di.PatientID);
        pList.AddInputParameter("pi_nItemID", di.ItemID);
        pList.AddInputParameter("pi_dtEntryDate", di.EntryDate);
        pList.AddInputParameter("pi_nSourceTypeID", di.SourceTypeID);
        pList.AddOutputParameter("po_nPatItemID", lPatItemID);

        //execute the SP
        status = base.DBConn.ExecuteOracleSP("PCK_PAT_ITEM.InsertPatItem", pList);
        if (!status.Status)
        {
            return status;
        }

        //get the ID returned from the SP call
        lPatItemID = pList.GetParamLongValue("po_nPatItemID");
        return status;
    }


    /// <summary>
    /// insert a patient item component
    /// </summary>
    /// <param name="di"></param>
    /// <param name="lPatItemComponentID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    private CStatus InsertPatientItemComponent(CPatientItemComponentDataItem di)
    {
        //status information returned from the call
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        
        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);
        //add the rest of the parameters
        pList.AddInputParameter("pi_vPatientID", di.PatientID);
        pList.AddInputParameter("pi_nItemID", di.ItemID);
        pList.AddInputParameter("pi_nPatItemID", di.PatItemID);
        pList.AddInputParameter("pi_nComponentID", di.ComponentID);
        pList.AddInputParameter("pi_vComponentValue", di.ComponentValue);

        //execute the SP
        return base.DBConn.ExecuteOracleSP("PCK_PAT_ITEM.InsertPatItemComponent", pList);
    }

    /// <summary>
    /// method
    /// inserts a patient item and all of its components
    /// if the inserts are successful
    /// </summary>
    /// <param name="diPatItem"></param>
    /// <param name="PatItemCompList"></param>
    /// <param name="lPatItemID"></param>
    /// <returns></returns>
    public CStatus InsertPatientItem(CPatientItemDataItem diPatItem, CPatientItemCompList PatItemCompList, out long lPatItemID)
    {
        CStatus status = InsertPatientItem(diPatItem, out lPatItemID);
        if (!status.Status)
        {
            return status;
        }

        foreach (CPatientItemComponentDataItem diPatItemComp in PatItemCompList)
        {
            diPatItemComp.PatItemID = lPatItemID;
            status = InsertPatientItemComponent(diPatItemComp);
            if (!status.Status)
            {
                return status;
            }
        }

        return status;
    }

    /// <summary>
    /// Gets a patient item and components
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemTypeID"></param>
    /// <param name="lItemID"></param>
    /// <param name="dtEntryDate"></param>
    /// <param name="dsItem"></param>
    /// <returns></returns>
    public CStatus GetPatItemCompDS(string strPatientID,
                                    long lItemID,
                                    DateTime dtEntryDate,
                                    out DataSet ds)
    {
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_dtEntryDate", dtEntryDate);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetPatItemCompRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }
      
        return status;
    }

    /// <summary>
    /// US:888 gets all components for a patient item used for trending
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatItemCompDS(string strPatientID,
                                   long lItemID,
                                   out DataSet ds)
    {
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetAllPatItemCompRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }

        return status;
    }

    /// <summary>
    /// update a patient item and re-run logic
    /// </summary>
    /// <param name="diPatItem"></param>
    /// <param name="PatItemCompList"></param>
    /// <returns></returns>
    public CStatus UpdatePatientItem(CPatientItemDataItem diPatItem, 
                                     CPatientItemCompList PatItemCompList)
    {
        CStatus status = new CStatus();
        
        //really no reason to update the patient item
        //status = UpdatePatientItem(diPatItem);
        //if (!status.Status)
        //{
        //    return status;
        // }

        //loop and update each component
        foreach (CPatientItemComponentDataItem diPatItemComp in PatItemCompList)
        {
            status = UpdatePatientItemComponent(diPatItemComp);
            if (!status.Status)
            {
                return status;
            }
        }

        return status;
    }

    /// <summary>
    /// insert a pat item comment
    /// </summary>
    /// <param name="lPatItemID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strCommentText"></param>
    /// <returns></returns>
    public CStatus InsertPatientItemComment(long lPatItemID,
                                            long lItemID,
                                            string strCommentText)
    {
        //status information returned from the call
        CStatus status = new CStatus();

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
        //add the rest of the parameters
        pList.AddInputParameter("pi_nPatItemID", lPatItemID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vCommentText", strCommentText);

        //execute the SP
        status = base.DBConn.ExecuteOracleSP("PCK_PAT_ITEM.InsertPatItemComment",
                                              pList);
        return status;
    }


    /// <summary>
    /// updates a patient item component
    /// </summary>
    /// <param name="diPatItemComp"></param>
    /// <returns></returns>
    public CStatus UpdatePatientItemComponent(CPatientItemComponentDataItem diPatItemComp)
    {
        //status information returned from the call
        CStatus status = new CStatus();

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

        pList.AddInputParameter("pi_nPatItemID", diPatItemComp.PatItemID);
        pList.AddInputParameter("pi_nItemID", diPatItemComp.ItemID);
        pList.AddInputParameter("pi_nComponentID", diPatItemComp.ComponentID);
        pList.AddInputParameter("pi_vComponentValue", diPatItemComp.ComponentValue);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_PAT_ITEM.UpdatePatItemComponent", pList);
    }

    
    /// <summary>
    /// get the most recent item data item given the patient id and item id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <param name="itm"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatientItemDI( string strPatientID,
                                     long lItemID,
                                     long lPatItemID,
                                     out CPatientItemDataItem di)
    {
        //initialize parameters
        di = new CPatientItemDataItem();

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        
        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nPatItemID", lPatItemID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        
        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetPatItemDIRS",
                                       pList,
                                       out ds);
        if(!status.Status)
        {
            return status;
        }

        //load the data item
        di = new CPatientItemDataItem(ds);
        
        return status;
    }

    /// <summary>
    /// get the most recent item data item given the patient id and item id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus GetMostRecentPatientItemDI(
        string strPatientID,
        long lItemID,
        out CPatientItemDataItem di)
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
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nItemID", lItemID);


        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetMostRecentPatItemDIRS",
                                       pList,
                                       out ds);
        if(!status.Status)
        {
            return status;
        }

        di = new CPatientItemDataItem(ds);
        
        return status;
    }

    public CStatus GetMostRecentPatientItemCompDI(string strPatientID,
                                                  long lItemID,
                                                  long lItemComponentID,
                                                  out CPatientItemComponentDataItem di)
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
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_nItemComponentID", lItemComponentID);


        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetMostRecentPatItemCompDIRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new CPatientItemComponentDataItem(ds);

        return status;
    }

    /// <summary>
    /// gets an item coponent data item given the patient id, pat itme id and item id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatItemID"></param>
    /// <param name="lItemID"></param>
    /// <param name="lComponentID"></param>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus GetPatientItemComponentDI( string strPatientID,
                                              long lPatItemID,
                                              long lItemID,
                                              long lComponentID,
                                              out CPatientItemComponentDataItem di)
    {
        di = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nPatItemID", lPatItemID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_nComponentID", lComponentID);

        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet( base.DBConn,
                                       "PCK_PAT_ITEM.GetPatItemComponentDIRS",
                                       pList,
                                       out ds);
        if(!status.Status)
        {
            return status;
        }

        di = new CPatientItemComponentDataItem(ds);
      
        return status;
    }

    /// <summary>
    /// get the components for an item
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatItemID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatientItemComponentDS(string strPatientID,
                                             long lPatItemID,
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

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nPatItemID", lPatItemID);
        pList.AddInputParameter("pi_nItemID", lItemID);
       

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetPatItemComponentRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }

        return status;
    }

    /// <summary>
    /// get the comments for an item
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatItemID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatientItemCommmentDS( long lPatItemID,
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

        pList.AddInputParameter("pi_nPatItemID", lPatItemID);
        pList.AddInputParameter("pi_nItemID", lItemID);


        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetPatItemCommentRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }

        return status;
    }

       /// <summary>
    /// get a ds or all patient items by item id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatItemID"></param>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatientItemDS(string strPatientID,
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

        pList.AddInputParameter("pi_vPatientID", strPatientID);
        pList.AddInputParameter("pi_nItemID", lItemID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_PAT_ITEM.GetPatItemRS",
                                       pList,
                                       out ds);
        if (!status.Status)
        {
            return status;
        }

        return status;
    }
}
