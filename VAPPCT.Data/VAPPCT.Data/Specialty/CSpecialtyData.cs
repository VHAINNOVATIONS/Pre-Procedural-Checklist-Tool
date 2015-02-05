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
/// Summary description for CSpecialtyData
/// </summary>
public class CSpecialtyData : CData
{
    public CSpecialtyData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }


    /// <summary>
    /// saves a specialty
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lTeamID"></param>
    /// <param name="strTeamLabel"></param>
    /// <returns></returns>
    public CStatus SaveSpecialty(long lXferSystemID,
                                 long lSpecialtyID,
                                 string strSpecialtyLabel)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nSpecialtyID", lSpecialtyID);
        pList.AddInputParameter("pi_vSpecialtyLabel", strSpecialtyLabel);

        return DBConn.ExecuteOracleSP("PCK_SPECIALTY.SaveSpecialty",
                                                 pList);
    }

    /// <summary>
    /// Gets a dataset of patients matching a specialty id
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetSpecialtyPatientsDS(long lSpecialtyID,
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
            status = ops.GetMDWSSpecialtyPatients(lSpecialtyID, out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nSpecialtyID", lSpecialtyID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet( base.DBConn,
                                       "PCK_SPECIALTY.GetPatientSpecialtyRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// get a dataset of all specialties, used for patient lookups etc...
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetSpecialtyDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSSpecialties(out lCount);
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
                                       "PCK_SPECIALTY.GetSpecialtyRS",
                                       pList,
                                       out ds);
        return status;
    }
}