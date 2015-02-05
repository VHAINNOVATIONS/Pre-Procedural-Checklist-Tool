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
/// Summary description for CWardData
/// </summary>
public class CWardData : CData
{
    public CWardData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }


    /// <summary>
    /// saves a Ward
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lTeamID"></param>
    /// <param name="strTeamLabel"></param>
    /// <returns></returns>
    public CStatus SaveWard(long lXferSystemID,
                                 long lWardID,
                                 string strWardLabel)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nWardID", lWardID);
        pList.AddInputParameter("pi_vWardLabel", strWardLabel);

        return DBConn.ExecuteOracleSP("PCK_WARD.SaveWard",
                                                 pList);
    }

    /// <summary>
    /// Gets a dataset of patients matching a Ward id
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetWardPatientsDS(long lWardID,
                                          out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSWardPatients(lWardID, out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nWardID", lWardID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_WARD.GetPatientWardRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// get a dataset of all specialties, used for patient lookups etc...
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetWardDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSWards(out lCount);
            if (!status.Status)
            {
                return status;
            }
        }
        
        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_WARD.GetWardRS",
                                       pList,
                                       out ds);
        return status;
    }
}