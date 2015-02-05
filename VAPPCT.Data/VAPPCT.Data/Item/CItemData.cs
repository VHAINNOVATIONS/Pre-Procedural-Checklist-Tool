using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Resources;
using VAPPCT.DA;

public class CItemData : CData
{
    public CItemData(CData Data)
        : base(Data)
    {
    }

    /// <summary>
    /// method
    /// used to get an individual item matching the item id specified
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="iItem"></param>
    /// <returns></returns>
    public CStatus GetItemDI(long lItemID, out CItemDataItem iItem)
    {
        //initialize parameters
        iItem = null;
        
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

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM.GetItemIDRS",
            pList,
            out ds);
        if(!status.Status)
        {
            return status;
        }

        iItem = new CItemDataItem(ds);

        return status;
    }

    /// <summary>
    /// method
    /// used to get an individual item matching the label specified
    /// </summary>
    /// <param name="strItemLabel"></param>
    /// <param name="iItem"></param>
    /// <returns></returns>
    public CStatus GetItemDI(string strItemLabel, out CItemDataItem iItem)
    {
        //initialize parameters
        iItem = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_vItemLabel", strItemLabel);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM.GetItemDIRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        iItem = new CItemDataItem(ds);

        return status;
    }

    /// <summary>
    /// method
    /// Used to get a dataset of items that meet the filters specified
    /// </summary>
    /// <param name="strLabel"></param>
    /// <param name="lTypeID"></param>
    /// <param name="lGroupID"></param>
    /// <param name="lActiveFilter"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetItemDS(
        string strLabel,
        long lTypeID,
        long lGroupID,
        long lActiveFilter,
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
        pList.AddInputParameter("pi_vFilterLabel", strLabel);
        pList.AddInputParameter("pi_nFilterTypeID", lTypeID);
        pList.AddInputParameter("pi_nFilterGroupID", lGroupID);
        pList.AddInputParameter("pi_nActiveFilter", lActiveFilter);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM.GetItemRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// gets all items in a checklist
    /// </summary>
    /// <param name="strLabel"></param>
    /// <param name="lTypeID"></param>
    /// <param name="lGroupID"></param>
    /// <param name="lActiveFilter"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetItemDS(
        string strLabel,
        long lTypeID,
        long lGroupID,
        long lActiveFilter, 
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
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_vFilterLabel", strLabel);
        pList.AddInputParameter("pi_nFilterTypeID", lTypeID);
        pList.AddInputParameter("pi_nFilterGroupID", lGroupID);
        pList.AddInputParameter("pi_nActiveFilter", lActiveFilter);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_ITEM.GetCLItemRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// inserts and item with the values of the item passed in
    /// </summary>
    /// <param name="iItem"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus InsertItem(CItemDataItem iItem, out long lItemID)
    {
        //initialize parameters
        lItemID = 0;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        // procedure specific parameters
        pList.AddInputParameter("pi_nItemTypeID", iItem.ItemTypeID);
        pList.AddInputParameter("pi_nItemGroupID", iItem.ItemGroupID);
        pList.AddInputParameter("pi_vItemLabel", iItem.ItemLabel);
        pList.AddInputParameter("pi_vItemDescription", iItem.ItemDescription);
        pList.AddInputParameter("pi_nLookbackTime", iItem.LookbackTime);
        pList.AddInputParameter("pi_nActiveID", (long)iItem.ActiveID);
        pList.AddInputParameter("pi_vMapID", iItem.MapID);

        pList.AddOutputParameter("po_nItemID", lItemID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM.InsertItem", pList);
        if (!status.Status)
        {
            return status;
        }

        lItemID = pList.GetParamLongValue("po_nItemID");

        return new CStatus();
    }

    /// <summary>
    /// method
    /// updates an item with the values of the item specified
    /// </summary>
    /// <param name="iItem"></param>
    /// <returns></returns>
    public CStatus UpdateItem(CItemDataItem iItem)
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
        pList.AddInputParameter("pi_nItemID", iItem.ItemID);
        pList.AddInputParameter("pi_nItemTypeID", iItem.ItemTypeID);
        pList.AddInputParameter("pi_nItemGroupID", iItem.ItemGroupID);
        pList.AddInputParameter("pi_vItemLabel", iItem.ItemLabel);
        pList.AddInputParameter("pi_vItemDescription", iItem.ItemDescription);
        pList.AddInputParameter("pi_nLookbackTime", iItem.LookbackTime);
        pList.AddInputParameter("pi_nActiveID", (long)iItem.ActiveID);
        pList.AddInputParameter("pi_vMapID", iItem.MapID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_ITEM.UpdateItem", pList);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1945 US:1883
    /// method
    /// deletes all the component/collection records that are not supported by the item's type
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="lItemTypeID"></param>
    /// <param name="strItemComponentIDs"></param>
    /// <param name="strItemIDs"></param>
    /// <returns></returns>
    public CStatus DeleteChildren(long lItemID, long lItemTypeID, string strItemComponentIDs, string strItemIDs)
    {
        CItemComponentData ic = new CItemComponentData(this);
        CItemCollectionData itemCollection = new CItemCollectionData(this);
        CStatus status = null;
        switch (lItemTypeID)
        {
            case (long)k_ITEM_TYPE_ID.Laboratory:
                status = ic.DeleteItemComponentStates(lItemID, string.Empty);
                if (!status.Status)
                {
                    return status;
                }
                break;
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                status = ic.DeleteItemComponentRanges(lItemID, string.Empty);
                if (!status.Status)
                {
                    return status;
                }
                break;
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
            case (long)k_ITEM_TYPE_ID.NoteTitle:
            case (long)k_ITEM_TYPE_ID.Collection:
                status = ic.DeleteItemComponentRanges(lItemID, string.Empty);
                if (!status.Status)
                {
                    return status;
                }

                status = ic.DeleteItemComponentStates(lItemID, string.Empty);
                if (!status.Status)
                {
                    return status;
                }
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        status = itemCollection.DeleteItemCollection(lItemID, strItemIDs);
        if (!status.Status)
        {
            return status;
        }

        status = ic.DeleteItemComponents(lItemID, strItemComponentIDs);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
