using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// Summary description for CItem
/// </summary>
public static class CItem
{
    /// <summary>
    /// method
    /// loads all the item groups into a ddl
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadItemGroupDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsItemGroups = null;
        CItemGroupData data = new CItemGroupData(Data);
        CStatus status = data.GetItemGroupDS(k_ACTIVE_ID.All, out dsItemGroups);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsItemGroups,
            ddl,
            "ITEM_GROUP_LABEL",
            "ITEM_GROUP_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the item groups that satisfy the active filter into a ddl
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="ddl"></param>
    /// <param name="ActiveFilter"></param>
    /// <returns></returns>
    public static CStatus LoadItemGroupDDL(
        CData Data,
        DropDownList ddl,
        k_ACTIVE_ID ActiveFilter)
    {
        //get the dataset
        DataSet dsItemGroups = null;
        CItemGroupData data = new CItemGroupData(Data);
        CStatus status = data.GetItemGroupDS(ActiveFilter, out dsItemGroups);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsItemGroups,
            ddl,
            "ITEM_GROUP_LABEL",
            "ITEM_GROUP_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads items in the specified collection into the ddl
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="ddl"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public static CStatus LoadItemCollectionDDL(
        CData Data,
        DropDownList ddl,
        long lItemID)
    {
        CItemCollectionData coll = new CItemCollectionData(Data);
        DataSet dsColl = null;
        CStatus status = coll.GetItemCollectionDS(lItemID, out dsColl);
        if (!status.Status)
        {
            return status;
        }

        status = CDropDownList.RenderDataSet(
            dsColl,
            ddl,
            "item_label",
            "item_id");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}
