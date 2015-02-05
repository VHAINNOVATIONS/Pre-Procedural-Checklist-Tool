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
/// Summary description for CSTATData
/// </summary>
public class CSTATData : CData
{
    //Constructor
    public CSTATData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

    /// <summary>
    /// Used to get a dataset of temporal state definitions
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetTSDefinitionDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList( SessionID,
                                                   ClientIP,
                                                   UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetTSDefinitionRS",
                                     pList,
                                     out ds);
    }

   /// <summary>
   /// Used to get a dataset of outcome state definitions
   /// </summary>
   /// <param name="ds"></param>
   /// <param name="lStatusCode"></param>
   /// <param name="strStatus"></param>
   /// <returns></returns>
    public CStatus GetOSDefinitionDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        //load the paramaters list
        CParameterList pList = new CParameterList( SessionID,
                                                   ClientIP,
                                                   UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetOSDefinitionRS",
                                     pList,
                                     out ds);
    }

   /// <summary>
   /// Used to get a dataset of decision state definitions
   /// </summary>
   /// <param name="ds"></param>
   /// <param name="lStatusCode"></param>
   /// <param name="strStatus"></param>
   /// <returns></returns>
    public CStatus GetDSDefinitionDS( out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetDSDefinitionRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// Used to get a dataset of item types
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetItemTypeDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetItemTypeRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// Used to get a dataset of active labels
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetActiveDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetActiveRS",
                                     pList,
                                     out ds);
    }


    /// <summary>
    /// Used to get a dataset of states
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetStateDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetStateRS",
                                     pList,
                                     out ds);
    }

    public CStatus GetStateDI(long lStateID, out CStateDataItem diState)
    {
        //initialize parameters
        diState = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
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
                                     "PCK_STAT.GetStateDIRS",
                                     pList,
                                     out ds);

        if (!status.Status)
        {
            return status;
        }

        //load the data item
        diState = new CStateDataItem(ds);

        return status;
    }

    /// <summary>
    /// Used to get a dataset of time units
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetUnitDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetUnitRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// gets a dataset of checklist states
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetChecklistStateDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetChecklistStateRS",
                                     pList,
                                     out ds);
    }

    /// <summary>
    /// Used to get a dataset of user roles
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetUserRolesDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet( DBConn,
                                     "PCK_STAT.GetUserRolesRS",
                                     pList,
                                     out ds);
    }
}
