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
public class CLabData : CData
{
    public CLabData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    ///US:852 saves a lab test to the database
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="nLabTestID"></param>
    /// <param name="strName"></param>
    /// <param name="strHIREF"></param>
    /// <param name="strLOREF"></param>
    /// <param name="strREFRANGE"></param>
    /// <param name="strUnits"></param>
    /// <param name="strDescription"></param>
    /// <returns></returns>
    public CStatus SaveLabTest( long lXferSystemID,
                                string strLabTestID,
                                string strName,
                                string strHIREF,
                                string strLOREF,
                                string strREFRANGE,
                                string strUnits,
                                string strDescription,
                                string strLOINC)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_vLabTestID", strLabTestID);
        pList.AddInputParameter("pi_vName", strName);
        pList.AddInputParameter("pi_vHIREF", strHIREF);
        pList.AddInputParameter("pi_vLOREF", strLOREF);
        pList.AddInputParameter("pi_vREFRANGE", strREFRANGE);
        pList.AddInputParameter("pi_vUnits", strUnits);
        pList.AddInputParameter("pi_vDescription", strDescription);
        pList.AddInputParameter("pi_vLOINC", strLOINC);
       
        return DBConn.ExecuteOracleSP("PCK_LAB.SaveLabTest",
                                       pList);
    }

    /// <summary>
    ///US:852 gets lab tests matching search criteria
    /// </summary>
    /// <param name="strSearch"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetLabTestDS( string strSearch, 
                                 out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSLabTests(strSearch);
            if (!status.Status)
            {
                return status;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);
        
        pList.AddInputParameter("pi_vSearch", strSearch);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_LAB.GetLabTestRS",
                                       pList,
                                       out ds);

        //todo: testing
        /*foreach (DataTable table in ds.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                string str = Convert.ToString(dr["lab_test_name"]);
                str += "";
            }

        }*/

        return status;
    }

}