using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CServiceData
/// </summary>
public class CServiceData : CData
{
    /// <summary>
    /// constructor
    /// initializes the base class
    /// </summary>
    /// <param name="data"></param>
    public CServiceData(CData data)
        : base(data)
	{
	}

    /// <summary>
    /// method
    /// retrieves a single service from the database
    /// </summary>
    /// <param name="lServiceID"></param>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus GetServiceDI(long lServiceID, out CServiceDataItem di)
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
        pList.AddInputParameter("pi_nServiceID", lServiceID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_VARIABLE.GetServiceDI",
                                      pList,
                                      out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new CServiceDataItem(ds);

        return status;
    }

    /// <summary>
    /// method
    /// retrieves all the services in the database that match the active filter
    /// </summary>
    /// <param name="lActiveFilter"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetServiceDS(k_ACTIVE_ID lActiveFilter, out DataSet ds)
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
        pList.AddInputParameter("pi_nActiveFilter", (long)lActiveFilter);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                    "PCK_VARIABLE.GetServiceRS",
                                    pList,
                                    out ds);
    }

    /// <summary>
    /// method
    /// inserts a service record into the database
    /// </summary>
    /// <param name="di"></param>
    /// <param name="lServiceID"></param>
    /// <returns></returns>
    public CStatus InsertService(CServiceDataItem di, out long lServiceID)
    {
        //initialize parameters
        lServiceID = 0;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        //add the rest of the parameters
        pList.AddInputParameter("pi_vServiceLabel", di.ServiceLabel);
        pList.AddInputParameter("pi_nIsActive", (long)((di.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));
        pList.AddOutputParameter("po_nServiceID", di.ServiceID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_VARIABLE.InsertService", pList);

        if (status.Status)
        {
            lServiceID = pList.GetParamLongValue("po_nServiceID");
        }

        return status;
    }

    /// <summary>
    /// method
    /// updates the specified service record in the database
    /// </summary>
    /// <param name="di"></param>
    /// <returns></returns>
    public CStatus UpdateService(CServiceDataItem di)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        //add the rest of the parameters
        pList.AddInputParameter("pi_nServiceID", di.ServiceID);
        pList.AddInputParameter("pi_vServiceLabel", di.ServiceLabel);
        pList.AddInputParameter("pi_nIsActive", (long)((di.IsActive) ? k_TRUE_FALSE_ID.True : k_TRUE_FALSE_ID.False));

        //execute the SP
        return DBConn.ExecuteOracleSP("PCK_VARIABLE.UpdateService", pList);
    }
}
