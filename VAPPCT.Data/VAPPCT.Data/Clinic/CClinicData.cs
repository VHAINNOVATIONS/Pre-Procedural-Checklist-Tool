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
/// Summary description for CClinic
/// </summary>
public class CClinicData: CData
{
    public CClinicData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}

    /// <summary>
    /// saves a Clinic to the database
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lTeamID"></param>
    /// <param name="strTeamLabel"></param>
    /// <returns></returns>
    public CStatus SaveClinic(long lXferSystemID,
                              long lClinicID,
                              string strClinicLabel)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nClinicID", lClinicID);
        pList.AddInputParameter("pi_vClinicLabel", strClinicLabel);

        return DBConn.ExecuteOracleSP("PCK_CLINIC.SaveClinic",
                                                 pList);
    }

    /// <summary>
    /// Gets a dataset of patients matching a Clinic id
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetClinicPatientsDS( long lClinicID,
                                        DateTime dtApptFrom,
                                        DateTime dtApptTo,
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

            status = ops.GetMDWSClinicPatients(lClinicID, 
                                               dtApptFrom, 
                                               dtApptTo, 
                                               out lCount);

            if (!status.Status)
            {
                return status;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nClinicID", lClinicID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_CLINIC.GetPatientClinicRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// get a dataset of all specialties, used for patient lookups etc...
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetClinicDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSClinics(out lCount);
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
                                       "PCK_CLINIC.GetClinicRS",
                                       pList,
                                       out ds);
        return status;
    }
}
