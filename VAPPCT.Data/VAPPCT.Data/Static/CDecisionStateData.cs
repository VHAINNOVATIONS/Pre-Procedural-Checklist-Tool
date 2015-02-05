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
/// Methods that pertain to an decision state
/// </summary>
public class CDecisionStateData : CData
{
    /// <summary>
    /// constructor
    /// initializes base class
    /// </summary>
    /// <param name="data"></param>
	public CDecisionStateData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

   /// <summary>
   /// Used to insert a decision state
   /// </summary>
   /// <param name="dsdi"></param>
   /// <param name="lDSID"></param>
   /// <param name="lStatusCode"></param>
   /// <param name="strStatus"></param>
   /// <returns></returns>
    public CStatus InsertDecisionState( CDecisionStateDataItem dsdi, out long lDSID)
    {
        //initialize parameters
        lDSID = 0;
        
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
        pList.AddInputParameter("pi_vDSLabel", dsdi.DSLabel);
        pList.AddInputParameter("pi_nDSDefinitionID", dsdi.DSDefinitionID);
        pList.AddInputParameter("pi_nIsActive", (long)((dsdi.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));
        pList.AddOutputParameter("po_nDSID", dsdi.DSID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_VARIABLE.InsertDecisionState", pList);

        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lDSID = pList.GetParamLongValue("po_nDSID");
        }

        return status;
    }

    /// <summary>
    /// Used to update a decision state
    /// </summary>
    /// <param name="dsdi"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus UpdateDecisionState(CDecisionStateDataItem dsdi)
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
        pList.AddInputParameter("pi_nDSID", dsdi.DSID);
        pList.AddInputParameter("pi_vDSLabel", dsdi.DSLabel);
        pList.AddInputParameter("pi_nDSDefinitionID", dsdi.DSDefinitionID);
        pList.AddInputParameter("pi_nIsActive", (long)((dsdi.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_VARIABLE.UpdateDecisionState", pList);
    }

    /// <summary>
    /// Used to get a dataset of decision states.
    /// </summary>
    /// <param name="lActiveFilter"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetDecisionStateDS(k_ACTIVE_ID lActiveFilter, out DataSet ds)
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

        pList.AddInputParameter("pi_nActiveFilter", Convert.ToInt64(lActiveFilter));
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_VARIABLE.GetDecisionStateRS",
            pList,
            out ds);
    }


    /// <summary>
    /// Used to get a dataset of decision states.
    /// </summary>
    /// <param name="lDSID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetDecisionStateDI(long lDSID, out CDecisionStateDataItem di)
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

        pList.AddInputParameter("pi_nDSID", lDSID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetDecisionStateIDRS",
                                      pList,
                                      out ds);

        if (!status.Status)
        {
            return status;
        }

        di = new CDecisionStateDataItem(ds);

        return status;
    }

    public CStatus GetDecisionStateDI(string strDSLabel, out CDecisionStateDataItem di)
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

        pList.AddInputParameter("pi_vDSLabel", strDSLabel);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetDecisionStateDIRS",
                                      pList,
                                      out ds);

        if (!status.Status)
        {
            return status;
        }

        di = new CDecisionStateDataItem(ds);

        return status;
    }

    public CStatus GetDefaultDSByStateID(long lStateID, out CDecisionStateDataItem di)
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

        pList.AddInputParameter("pi_nStateID", lStateID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetDefaultDSByStateIDRS",
                                      pList,
                                      out ds);

        if (!status.Status)
        {
            return status;
        }

        di = new CDecisionStateDataItem(ds);

        return status;
    }
}
