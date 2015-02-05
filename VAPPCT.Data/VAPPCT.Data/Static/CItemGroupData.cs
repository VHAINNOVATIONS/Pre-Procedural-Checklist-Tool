using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Resources;
using VAPPCT.DA;

/// <summary>
/// Methods that pertain to an item group
/// </summary>
public class CItemGroupData : CData
{
    //Constructor
    public CItemGroupData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

    /// <summary>
    /// Used insert an item group
    /// </summary>
    /// <param name="igdi"></param>
    /// <param name="lItemGroupID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertItemGroup(CItemGroupDataItem igdi, out long lItemGroupID)
    {
        //initialize parameters
        lItemGroupID = 0;
      
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
        pList.AddInputParameter("pi_vItemGroupLabel", igdi.ItemGroupLabel);
        pList.AddInputParameter("pi_nIsActive", (long)((igdi.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));
        pList.AddOutputParameter("po_nItemGroupID", igdi.ItemGroupID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_VARIABLE.InsertItemGroup", pList);

        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lItemGroupID = pList.GetParamLongValue("po_nItemGroupID");
        }

        return status;
    }

    /// <summary>
    /// Used to update an item group
    /// </summary>
    /// <param name="igdi"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus UpdateItemGroup(CItemGroupDataItem igdi)
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
        pList.AddInputParameter("pi_nItemGroupID", igdi.ItemGroupID);
        pList.AddInputParameter("pi_vItemGroupLabel", igdi.ItemGroupLabel);
        pList.AddInputParameter("pi_nIsActive", (long)((igdi.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_VARIABLE.UpdateItemGroup", pList);
    }

   /// <summary>
    /// get a dataset of item groups
   /// </summary>
   /// <param name="lActiveFilter"></param>
   /// <param name="ds"></param>
   /// <param name="lStatusCode"></param>
   /// <param name="strStatus"></param>
   /// <returns></returns>
    public CStatus GetItemGroupDS(k_ACTIVE_ID lActiveFilter, out DataSet ds)
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

        pList.AddInputParameter("pi_nActiveFilter", (long)lActiveFilter);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_VARIABLE.GetItemGroupRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// Used to get a dataset holding 1 item group by id
    /// </summary>
    /// <param name="lItemGroupID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetItemGroupDI(long lItemGroupID, out CItemGroupDataItem di)
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

        pList.AddInputParameter("pi_nItemGroupID", lItemGroupID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(
            DBConn,
            "PCK_VARIABLE.GetItemGroupIDRS",
            pList,
            out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new CItemGroupDataItem(ds);

        return new CStatus();
    }
}
