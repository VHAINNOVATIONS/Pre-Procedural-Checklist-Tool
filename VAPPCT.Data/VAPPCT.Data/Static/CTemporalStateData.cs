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
/// Methods that pertain to an temporal state
/// </summary>
public class CTemporalStateData : CData
{
    /// <summary>
    /// constructor
    /// initializes base class
    /// </summary>
    /// <param name="data"></param>
    public CTemporalStateData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

    /// <summary>
    /// Used to insert a temporal state
    /// </summary>
    /// <param name="tsdi"></param>
    /// <param name="lTSID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus InsertTemporalState(CTemporalStateDataItem tsdi, out long lTSID)
    {
        //initialize parameters
        lTSID = 0;

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
        pList.AddInputParameter("pi_vTSLabel", tsdi.TSLabel);
        pList.AddInputParameter("pi_nTSDefinitionID", tsdi.TSDefinitionID);
        pList.AddInputParameter("pi_nIsActive", (long)((tsdi.IsActive) ? k_TRUE_FALSE_ID.True: k_TRUE_FALSE_ID.False));
        pList.AddOutputParameter("po_nTSID", lTSID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_VARIABLE.InsertTemporalState", pList);

        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            lTSID = pList.GetParamLongValue("po_nTSID");
        }

        return status;
    }

    /// <summary>
    /// Used to update a temporal state
    /// </summary>
    /// <param name="tsdi"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus UpdateTemporalState(CTemporalStateDataItem tsdi)
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
        pList.AddInputParameter("pi_nTSID", tsdi.TSID);
        pList.AddInputParameter("pi_vTSLabel", tsdi.TSLabel);
        pList.AddInputParameter("pi_nTSDefinitionID", tsdi.TSDefinitionID);
        pList.AddInputParameter("pi_nIsActive", (long)((tsdi.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));
       
        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_VARIABLE.UpdateTemporalState", pList);
    }

    /// <summary>
    /// Used to get a dataset of temporal states.
    /// </summary>
    /// <param name="lActiveFilter"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetTemporalStateDS(long lActiveFilter, out DataSet ds)
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

        pList.AddInputParameter("pi_nActiveFilter", lActiveFilter);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_VARIABLE.GetTemporalStateRS",
                                     pList,
                                     out ds);
    }


    /// <summary>
    /// get a dataset of temporal states.
    /// </summary>
    /// <param name="lTSID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetTemporalStateDI(long lTSID, out CTemporalStateDataItem di)
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
        pList.AddInputParameter("pi_nTSID", lTSID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetTemporalStateIDRS",
                                      pList,
                                      out ds);

        if (!status.Status)
        {
            return status;
        }

        //load the data item
        di = new CTemporalStateDataItem(ds);

        return status;
    }

    public CStatus GetTemporalStateDI(string strTSLabel, out CTemporalStateDataItem di)
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

        pList.AddInputParameter("pi_vTSLabel", strTSLabel);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetTemporalStateDIRS",
                                      pList,
                                      out ds);

        if (!status.Status)
        {
            return status;
        }

        //load the data item
        di = new CTemporalStateDataItem(ds);

        return status;
    }
}
