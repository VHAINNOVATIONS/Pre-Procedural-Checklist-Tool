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
/// Methods that pertains to the User
/// </summary>
public class CUserData : CData
{
    public CUserData(CData Data)
        : base(Data)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    /// audits page access and determines whether the user is allowed access
    /// </summary>
    /// <param name="strPageName"></param>
    /// <returns></returns>
    public CStatus AuditPageAccess(string strPageName)
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
        pList.AddInputParameter("pi_vPageName", strPageName);
        
        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_FX_SECURITY.AuditPageAccess", pList);
       
        return status;
    }

    /// <summary>
    /// gets a dataset of user by last name and first name search criteria
    /// </summary>
    /// <param name="strLastName"></param>
    /// <param name="strFirstName"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetUserDS(string strLastName,
                             string strFirstName,
                             out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (base.MDWSTransfer)
        {
            string strSearch = "";
            strSearch += strLastName;
            if (!String.IsNullOrEmpty(strFirstName))
            {
                strSearch += "," + strFirstName;
            }

            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSUsers(strSearch);
            
            //todo
            //if (!status.Status)
            //{
            //    return status;
            //}
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vLastName", strLastName);
        pList.AddInputParameter("pi_vFirstName", strFirstName);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_USR.GetUserRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// gets a dataset of user roles
    /// </summary>
    /// <param name="strLastName"></param>
    /// <param name="strFirstName"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetUserRolesDS(long lUserID, out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  lUserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_USR.GetUserRolesRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// US:836 create a session record in the database
    /// </summary>
    /// <param name="strFXSessionID"></param>
    /// <returns></returns>
    public CStatus CreateFXSession(out string strFXSessionID)
    {
        strFXSessionID = String.Empty;

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
        pList.AddOutputParameter("po_vFXSessionID", strFXSessionID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_USR.CreateFXSession", pList);
        if (status.Status)
        {
            //get the TS_ID returned from the SP call
            strFXSessionID = pList.GetParamStringValue("po_vFXSessionID");
        }

        return status;

    }

    /// <summary>
    /// US:836 checks for a valid fx_session record
    /// </summary>
    /// <returns></returns>
    public CStatus CheckFXSession()
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
        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_USR.CheckFXSession", pList);
        return status;
    }

    /// <summary>
    /// US:836 clears the database session record, called when the user logs off
    /// </summary>
    /// <returns></returns>
    public CStatus ClearFXSession()
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
        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_USR.ClearFXSession", pList);
       
        return status;

    }

    /// <summary>
    /// logs the user in and returns info about the user. TODO: this will
    /// probably be heavily modified later...
    /// </summary>
    /// <param name="strUserName"></param>
    /// <param name="strPassword"></param>
    /// <param name="ds"></param>
    /// <param name="lLoginUserID"></param>
    /// <param name="lloginRoleID"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetLoginUserDS(string strUserName,
                                string strPassword,
                                out DataSet ds,
                                out long lLoginUserID,
                                out long lloginRoleID)
    {
        //initialize parameters
        ds = null;

        lLoginUserID = 0;
        lloginRoleID = 0;
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list

        //load standard paramaters
        CParameterList pList = new CParameterList(SessionID,
                                                   ClientIP,
                                                   UserID);

        //load additional paramaters
        pList.AddInputParameter("pi_vUserName", strUserName);
        pList.AddInputParameter("pi_vPassword", strPassword);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(DBConn,
                                       "PCK_USR.GetLoginUserRS",
                                       pList,
                                       out ds);

        if (status.StatusCode != k_STATUS_CODE.Success)
        {
            return status;
        }

        if (ds == null)
        {
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "Invalid Username/Password";
            status.Status = false;

            return status;
        }

        if (status.Status)
        {
            lLoginUserID = CDataUtils.GetDSLongValue(ds, "USER_ID");
            lloginRoleID = CDataUtils.GetDSLongValue(ds, "USER_ROLE_ID");
        }

        if (lLoginUserID < 1)
        {
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "Invalid Username/Password";
            status.Status = false;

            return status;
        }

        return status;

    }

    /// <summary>
    /// looks up a user by user id
    /// </summary>
    /// <param name="lUserID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetUserDS(long lUserID, out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load standard paramaters
        CParameterList pList = new CParameterList(SessionID,
                                                   ClientIP,
                                                   UserID);

        //load additional paramaters
        pList.AddInputParameter("pi_nLookupUserID", lUserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(DBConn,
                                       "PCK_USR.GetUserRS",
                                       pList,
                                       out ds);
        if (status.Status)
        {
            if (CDataUtils.IsEmpty(ds))
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = "Could not retrieve user information!";
            }
        }

        return status;
    }
}
