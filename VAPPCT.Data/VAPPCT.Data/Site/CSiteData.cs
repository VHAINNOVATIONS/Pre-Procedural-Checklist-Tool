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
/// Summary description for CLabData
/// </summary>
public class CSiteData : CData
{
    public CSiteData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }


    /// <summary>
    /// save a region
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lRegionID"></param>
    /// <param name="strRegionName"></param>
    /// <returns></returns>
    public CStatus SaveRegion( long lXferSystemID,
                               long lRegionID,
                               string strRegionName)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nRegionID", lRegionID);
        pList.AddInputParameter("pi_vRegionName", strRegionName);

        return DBConn.ExecuteOracleSP("PCK_SITE.SaveRegion",
                                       pList);
    }


    /// <summary>
    /// Save a site
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lRegionID"></param>
    /// <param name="lSiteID"></param>
    /// <param name="strSiteName"></param>
    /// <returns></returns>
    public CStatus SaveSite( long lXferSystemID,
                             long lRegionID,
                             long lSiteID,
                             string strSiteName)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nRegionID", lRegionID);
        pList.AddInputParameter("pi_nSiteID", lSiteID);
        pList.AddInputParameter("pi_vSiteName", strSiteName);

        return DBConn.ExecuteOracleSP("PCK_SITE.SaveSite",
                                       pList);
    }

    /// <summary>
    /// get a regions dataset
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetRegionDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSSites();
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
                                       "PCK_SITE.GetRegionRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// get a site dataset by egion
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetSiteDS( long lRegionID,
                              out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
     //   if (MDWSTransfer)
     //   {
     //       CMDWSOps ops = new CMDWSOps(this);
     //       status = ops.GetMDWSSites();
     //       if (!status.Status)
     //       {
     //           return status;
     //       }
     //   }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nRegionID", lRegionID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_SITE.GetSiteRS",
                                       pList,
                                       out ds);
        return status;
    }
    
}