using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// class
/// contains helper methods to load static data into controls
/// </summary>
public static class CSTAT
{
    /// <summary>
    /// method
    /// loads all the checklist states into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadChecklistStateDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet ds = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetChecklistStateDS(out ds);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            ds,
            ddl,
            "CHECKLIST_STATE_LABEL",
            "CHECKLIST_STATE_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the ts definitions into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadTSDefinitionDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsTS = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetTSDefinitionDS(out dsTS);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsTS,
            ddl,
            "TS_DEFINITION_LABEL",
            "TS_DEFINITION_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the os definitions into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadOSDefinitionDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsOS = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetOSDefinitionDS(out dsOS);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsOS,
            ddl,
            "OS_DEFINITION_LABEL",
            "OS_DEFINITION_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the ds defintions into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadDSDefinitionDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsDS = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetDSDefinitionDS(out dsDS);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsDS,
            ddl,
            "DS_DEFINITION_LABEL",
            "DS_DEFINITION_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the item types into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadItemTypeDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsItemTypes = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetItemTypeDS(out dsItemTypes);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsItemTypes,
            ddl,
            "ITEM_TYPE_LABEL",
            "ITEM_TYPE_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the active options into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadActiveDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsActive = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetActiveDS(out dsActive);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsActive,
            ddl,
            "ACTIVE_LABEL",
            "ACTIVE_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the state options into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadStateDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsState = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetStateDS(out dsState);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsState,
            ddl,
            "STATE_LABEL",
            "STATE_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// loads all the unit options into a drop down list
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="ddl"></param>
    /// <returns></returns>
    public static CStatus LoadUnitDDL(CData Data, DropDownList ddl)
    {
        //get the dataset
        DataSet dsUnit = null;
        CSTATData data = new CSTATData(Data);
        CStatus status = data.GetUnitDS(out dsUnit);
        if (!status.Status)
        {
            return status;
        }

        //render the dataset
        status = CDropDownList.RenderDataSet(
            dsUnit,
            ddl,
            "TIME_UNIT_LABEL",
            "TIME_UNIT_ID");
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }
}