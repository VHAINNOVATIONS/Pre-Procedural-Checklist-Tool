using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using VAPPCT.DA;

/// <summary>
/// Methods that pertain to an Item Component
/// </summary>
public class CItemComponentData : CData
{
    //Constructor
    public CItemComponentData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

    /// <summary>
    /// Used to get a dataset of item components that pertain
    /// to an item
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetItemComponentDS(
        long lItemID,
        k_ACTIVE_ID lActiveFilter,
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

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_nActiveFilter", (long)lActiveFilter);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COMPONENT.GetItemComponentRS",
            pList,
            out ds);
    }

    public CStatus GetItemComponentDI(
        long lItemID,
        string strItemComponentLabel,
        out CItemComponentDataItem di)
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

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vItemComponentLabel", strItemComponentLabel);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COMPONENT.GetItemComponentDIRS",
            pList,
            out ds);

        if (!status.Status)
        {
            return status;
        }

        di = new CItemComponentDataItem(ds);

        return status;
    }

    /// <summary>
    /// Used to get a dataset outer joining item components, item component states
    /// and item component ranges
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetItemComponentOJDS(
        long lItemID,
        k_ACTIVE_ID lActiveFilter,
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

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_nActiveFilter", (long)lActiveFilter);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COMPONENT.GetItemComponentOJRS",
            pList,
            out ds);
    }


    /// <summary>
    /// Used to insert an item component
    /// </summary>
    /// <param name="icComponent"></param>
    /// <param name="lItemComponentID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertItemComponent(CItemComponentDataItem icComponent, out long lItemComponentID)
    {
        //initialize parameters
        lItemComponentID = 0;
        
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", icComponent.ItemID);
        pList.AddInputParameter("pi_vItemComponentLabel", icComponent.ItemComponentLabel);
        pList.AddInputParameter("pi_nSortOrder", icComponent.SortOrder);
        pList.AddInputParameter("pi_nActiveID", (long)icComponent.ActiveID);

        pList.AddOutputParameter("po_nItemComponentID", lItemComponentID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.InsertItemComponent", pList);
        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lItemComponentID = pList.GetParamLongValue("po_nItemComponentID");
        }

        return status;
    }

    /// <summary>
    /// Used to delete item components for an item
    /// <param name="lItemID"></param>
    /// <param name="strItemComponentIDs"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus DeleteItemComponents(long lItemID, string strItemComponentIDs)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vItemComponentIDs", strItemComponentIDs);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.DeleteItemComponents", pList);
    }

    /// <summary>
    /// Used to update an item component
    /// </summary>
    /// <param name="icComponent"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus UpdateItemComponent(CItemComponentDataItem icComponent)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", icComponent.ItemID);
        pList.AddInputParameter("pi_vItemComponentLabel", icComponent.ItemComponentLabel);
        pList.AddInputParameter("pi_nSortOrder", icComponent.SortOrder);
        pList.AddInputParameter("pi_nActiveID", (long)icComponent.ActiveID);
        pList.AddInputParameter("pi_nItemComponentID", icComponent.ItemComponentID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.UpdateItemComponent", pList);
    }

    /// <summary>
    /// Used to get an item component state
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="lItemComponentID"></param>
    /// <param name="icsState"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetICStateDI(
        long lItemID,
        long lItemComponentID,
        out CICStateDataItem icsState)
    {
        //initialize parameters
        icsState = null;
        
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_nItemComponentID", lItemComponentID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COMPONENT.GetICStateIDRS",
            pList,
            out ds);

        if(!status.Status)
        {
            return status;
        }

        icsState = new CICStateDataItem(ds);

        return status;
    }

    /// <summary>
    /// Used to insert and item component state
    /// </summary>
    /// <param name="icsState"></param>
    /// <param name="lICStateID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertICState(CICStateDataItem icsState, out long lICStateID)
    {
        //initialize parameters
        lICStateID = 0;
        
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", icsState.ItemID);
        pList.AddInputParameter("pi_nItemComponentID", icsState.ItemComponentID);
        pList.AddInputParameter("pi_nStateID", icsState.StateID);

        pList.AddOutputParameter("po_nICStateID", lICStateID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.InsertICState", pList);

        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lICStateID = pList.GetParamLongValue("po_nICStateID");
        }

        return status;
    }

    /// <summary>
    /// Used to delete item components that pertain to an item/item component
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="strItemComponentIDs"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus DeleteItemComponentStates(long lItemID, string strItemComponentIDs)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vItemComponentIDs", strItemComponentIDs);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.DeleteItemComponentStates", pList);
    }

    /// <summary>
    /// Used to delete ranges that pertain to an item/item component
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="strItemComponentIDs"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus DeleteItemComponentRanges(long lItemID, string strItemComponentIDs)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_vItemComponentIDs", strItemComponentIDs);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.DeleteItemComponentRanges", pList);
    }

    /// <summary>2
    /// Used to update an item component state
    /// </summary>
    /// <param name="icsState"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus UpdateICState(CICStateDataItem icsState)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", icsState.ItemID);
        pList.AddInputParameter("pi_nItemComponentID", icsState.ItemComponentID);
        pList.AddInputParameter("pi_nStateID", icsState.StateID);
        pList.AddInputParameter("pi_nICStateID", icsState.ICStateID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.UpdateICState", pList);

    }

    /// <summary>
    /// Used to get an item component range
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="lItemComponentID"></param>
    /// <param name="icrRange"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetICRangeDI(
        long lItemID,
        long lItemComponentID,
        out CICRangeDataItem icrRange)
    {
        //initialize parameters
        icrRange = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", lItemID);
        pList.AddInputParameter("pi_nItemComponentID", lItemComponentID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COMPONENT.GetICRangeIDRS",
            pList,
            out ds);

        if(!status.Status)
        {
            return status;
        }

        icrRange = new CICRangeDataItem(ds);

        return status;
    }

    /// <summary>
    /// Used to insert an item component range
    /// </summary>
    /// <param name="icrRange"></param>
    /// <param name="lICRangeID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertICRange(CICRangeDataItem icrRange, out long lICRangeID)
    {
        //initialize parameters
        lICRangeID = 0;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", icrRange.ItemID);
        pList.AddInputParameter("pi_nItemComponentID", icrRange.ItemComponentID);
        pList.AddInputParameter("pi_vUnits", icrRange.Units);
        pList.AddInputParameter("pi_nLegalMin", icrRange.LegalMin);
        pList.AddInputParameter("pi_nCriticalLow", icrRange.CriticalLow);
        pList.AddInputParameter("pi_nLow", icrRange.Low);
        pList.AddInputParameter("pi_nHigh", icrRange.High);
        pList.AddInputParameter("pi_nCriticalHigh", icrRange.CriticalHigh);
        pList.AddInputParameter("pi_nLegalMax", icrRange.LegalMax);

        pList.AddOutputParameter("po_nICRangeID", lICRangeID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.InsertICRange", pList);

        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lICRangeID = pList.GetParamLongValue("po_nICRangeID");
        }

        return status;
    }

    /// <summary>
    /// Used to update an item component range
    /// </summary>
    /// <param name="icrRange"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus UpdateICRange(CICRangeDataItem icrRange)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemID", icrRange.ItemID);
        pList.AddInputParameter("pi_nItemComponentID", icrRange.ItemComponentID);
        pList.AddInputParameter("pi_vUnits", icrRange.Units);
        pList.AddInputParameter("pi_nLegalMin", icrRange.LegalMin);
        pList.AddInputParameter("pi_nCriticalLow", icrRange.CriticalLow);
        pList.AddInputParameter("pi_nLow", icrRange.Low);
        pList.AddInputParameter("pi_nHigh", icrRange.High);
        pList.AddInputParameter("pi_nCriticalHigh", icrRange.CriticalHigh);
        pList.AddInputParameter("pi_nLegalMax", icrRange.LegalMax);
        pList.AddInputParameter("pi_nICRangeID", icrRange.ICRangeID);

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_ITEM_COMPONENT.UpdateICRange", pList);
    }
}
