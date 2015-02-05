using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAPPCT.DA;

public class CItemCollectionData : CData
{
    public CItemCollectionData(CData Data)
        : base(Data)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    /// US:1883 insert an item collection
    /// </summary>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus InsertItemCollection(CItemCollectionDataItem di)
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
        pList.AddInputParameter("pi_nCollectionItemID", di.CollectionItemID);
        pList.AddInputParameter("pi_nItemID", di.ItemID);
        pList.AddInputParameter("pi_nSortOrder", di.SortOrder);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COLLECTION.InsertItemCollection", pList);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 get a collection dataset
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetItemCollectionDS(out DataSet ds)
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

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COLLECTION.GetItemCollectionRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 get an items collection dataset
    /// </summary>
    /// <param name="lCollectionItemID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetItemCollectionDS(long lCollectionItemID, out DataSet ds)
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

        pList.AddInputParameter("pi_nCollectionItemID", lCollectionItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COLLECTION.GetItemCollectionRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 get the most recent item
    /// </summary>
    /// <param name="lCollectionItemID"></param>
    /// <param name="strPatientID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetItemColMostRecentPatItemDS(long lCollectionItemID, string strPatientID, out DataSet ds)
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

        pList.AddInputParameter("pi_nCollectionItemID", lCollectionItemID);
        pList.AddInputParameter("pi_vPatientID", strPatientID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COLLECTION.GetItemColPatItemRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 get the patients most recent collection item
    /// </summary>
    /// <param name="lCollectionItemID"></param>
    /// <param name="strPatientID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetItemColMostRecentPatICDS(long lCollectionItemID, string strPatientID, out DataSet ds)
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
        pList.AddInputParameter("pi_nCollectionItemID", lCollectionItemID);
        pList.AddInputParameter("pi_vPatientID", strPatientID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COLLECTION.GetItemColPatICRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 get the item collection DI
    /// </summary>
    /// <param name="lCollectionItemID"></param>
    /// <param name="lItemID"></param>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus GetItemCollectionDI(long lCollectionItemID, long lItemID, out CItemCollectionDataItem di)
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
        pList.AddInputParameter("pi_nCollectionItemID", lCollectionItemID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM_COLLECTION.GetItemCollectionDI",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new CItemCollectionDataItem(ds);

        return new CStatus();
    }

    /// <summary>
    /// US:1883 update a collection item
    /// </summary>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus UpdateItemCollection(CItemCollectionDataItem di)
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
        pList.AddInputParameter("pi_nCollectionItemID", di.CollectionItemID);
        pList.AddInputParameter("pi_nItemID", di.ItemID);
        pList.AddInputParameter("pi_nSortOrder", di.SortOrder);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COLLECTION.UpdateItemCollection", pList);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 delete a collection item
    /// </summary>
    /// <param name="lCollectionItemID"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus DeleteItemCollection(long lCollectionItemID, long lItemID)
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
        pList.AddInputParameter("pi_nCollectionItemID", lCollectionItemID);
        pList.AddInputParameter("pi_nItemID", lItemID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COLLECTION.DeleteItemCollection", pList);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 delete item collection
    /// </summary>
    /// <param name="lCollectionItemID"></param>
    /// <returns></returns>
    public CStatus DeleteItemCollection(long lCollectionItemID)
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
        pList.AddInputParameter("pi_nCollectionItemID", lCollectionItemID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COLLECTION.DeleteItemCollection", pList);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1883 delete item collection
    /// </summary>
    /// <param name="lCollectionItemID"></param>
    /// <param name="strItemIDs"></param>
    /// <returns></returns>
    public CStatus DeleteItemCollection(long lCollectionItemID, string strItemIDs)
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
        pList.AddInputParameter("pi_nCollectionItemID", lCollectionItemID);
        pList.AddInputParameter("pi_vItemIDs", strItemIDs);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM_COLLECTION.DeleteItemCollection", pList);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
