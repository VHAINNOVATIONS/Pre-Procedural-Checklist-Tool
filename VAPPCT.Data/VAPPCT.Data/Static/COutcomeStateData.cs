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

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Methods that pertain to an outcome state
/// </summary>
public class COutcomeStateData  : CData
{
    /// <summary>
    /// constructor
    /// initializes base class
    /// </summary>
    /// <param name="data"></param>
    public COutcomeStateData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

   /// <summary>
   /// Used to insert an outcome state
   /// </summary>
   /// <param name="osdi"></param>
   /// <param name="lOSID"></param>
   /// <param name="lStatusCode"></param>
   /// <param name="strStatus"></param>
   /// <returns></returns>
    public CStatus InsertOutcomeState(COutcomeStateDataItem osdi, out long lOSID)
    {
        //initialize parameters
        lOSID = 0;

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
        pList.AddInputParameter("pi_vOSLabel", osdi.OSLabel);
        pList.AddInputParameter("pi_nOSDefinitionID", osdi.OSDefinitionID);
        pList.AddInputParameter("pi_nIsActive", (long)((osdi.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));
        pList.AddOutputParameter("po_nOSID", osdi.OSID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_VARIABLE.InsertOutcomeState", pList);
        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lOSID = pList.GetParamLongValue("po_nOSID");
        }

        return status;
    }

  /// <summary>
  /// Used to update an outcome state
  /// </summary>
  /// <param name="osdi"></param>
  /// <param name="lStatusCode"></param>
  /// <param name="strStatus"></param>
  /// <returns></returns>
    public CStatus UpdateOutcomeState(COutcomeStateDataItem osdi)
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
        pList.AddInputParameter("pi_nOSID", osdi.OSID);
        pList.AddInputParameter("pi_vOSLabel", osdi.OSLabel);
        pList.AddInputParameter("pi_nOSDefinitionID", osdi.OSDefinitionID);
        pList.AddInputParameter("pi_nIsActive", (long)((osdi.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_VARIABLE.UpdateOutcomeState", pList);
    }

    /// <summary>
    /// Used to get a dataset of outcome states.
    /// </summary>
    /// <param name="lActiveFilter"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetOutcomeStateDS(long lActiveFilter, out DataSet ds)
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

        pList.AddInputParameter("pi_nActiveFilter", lActiveFilter);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_VARIABLE.GetOutcomeStateRS",
                                     pList,
                                     out ds);
    }


    /// <summary>
    /// Used to get a dataset of outcome states.
    /// </summary>
    /// <param name="lOSID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetOutcomeStateDI(long lOSID, out COutcomeStateDataItem di)
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
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        pList.AddInputParameter("pi_nOSID", lOSID);


        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetOutcomeStateIDRS",
                                      pList,
                                      out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new COutcomeStateDataItem(ds);

        return status;
    }

    public CStatus GetOutcomeStateDI(string strOSLabel, out COutcomeStateDataItem di)
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
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);

        pList.AddInputParameter("pi_vOSLabel", strOSLabel);


        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetOutcomeStateDIRS",
                                      pList,
                                      out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new COutcomeStateDataItem(ds);

        return status;
    }
}
