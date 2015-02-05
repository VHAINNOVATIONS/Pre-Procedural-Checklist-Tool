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
/// Summary description for CSecurityKeyData
/// </summary>
public class CSecurityKeyData : CData
{
    public CSecurityKeyData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    /// saves a security key
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lSecurityKeyID"></param>
    /// <param name="strSecurityKeyName"></param>
    /// <returns></returns>
    public CStatus SaveSecurityKey( long lXferSystemID,
                                    long lUserID,
                                    long lSecurityKeyID,
                                    string strSecurityKeyName)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nSecurityKeyID", lSecurityKeyID);
        pList.AddInputParameter("pi_vSecurityKeyName", strSecurityKeyName);

        return DBConn.ExecuteOracleSP( "PCK_SECURITY_KEY.SaveSecurityKey",
                                       pList);
    }

    /// <summary>
    /// does the user have a security keys by key name
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public bool HasSecurityKey(long lUserID,
                               string strKeyName)
    {
        //initialize parameters
        DataSet ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSSecurityKeys(lUserID,
                                             true,
                                             out lCount);
            if (!status.Status)
            {
                return false;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vSecurityKeyName", strKeyName.Trim().ToUpper());
       
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_SECURITY_KEY.GetSecurityKeyByNameRS",
                                       pList,
                                       out ds);

        if (!status.Status)
        {
            return false;
        }

        string strKey = CDataUtils.GetDSStringValue(ds, "security_key_name");
        if (!String.IsNullOrEmpty(strKey))
        {
            if (strKey.Trim().ToLower() == strKeyName.Trim().ToLower())
            {
                return true;
            }
        }
        
        return false;
    }
}