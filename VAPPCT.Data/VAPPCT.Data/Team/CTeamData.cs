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
/// Summary description for CTeamData
/// </summary>
public class CTeamData : CData
{
    public CTeamData(CData data)
        : base(data)
	{
        //constructors are not inherited in c#!
	}
        
    /// <summary>
    /// saves a team to the database
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lTeamID"></param>
    /// <param name="strTeamLabel"></param>
    /// <returns></returns>
    public CStatus SaveTeam(long lXferSystemID, 
                            long lTeamID, 
                            string strTeamLabel)
    {
        CStatus status = new CStatus();
        
        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nTeamID", lTeamID);
        pList.AddInputParameter("pi_vTeamLabel", strTeamLabel);
        
        return base.DBConn.ExecuteOracleSP("PCK_TEAM.SaveTeam",
                                            pList);
    }

    /// <summary>
    /// Gets a dataset of patients matching a team id
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetTeamPatientsDS( long lTeamID,
                                      out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (base.MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSTeamPatients(lTeamID, out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);
        
        pList.AddInputParameter("pi_nTeamID", lTeamID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_TEAM.GetPatientTeamRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// get a dataset of all teams, used for patient lookups etc...
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetTeamDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (base.MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSTeams(out lCount);
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
                                       "PCK_TEAM.GetTeamRS",
                                       pList,
                                       out ds);
        return status;
    }

}
