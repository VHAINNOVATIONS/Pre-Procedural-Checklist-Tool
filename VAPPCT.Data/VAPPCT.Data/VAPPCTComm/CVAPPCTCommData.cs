using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

public class CVAPPCTCommData : CData
{
    public CVAPPCTCommData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
	}

    /// <summary>
    /// US:834 save a communicator event
    /// </summary>
    /// <param name="strEventName"></param>
    /// <param name="strEventDetails"></param>
    /// <returns></returns>
    public CStatus SaveCommEvent(string strEventName,
                                 string strEventDetails)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vEventName", strEventName);
        pList.AddInputParameter("pi_vEventDetails", strEventDetails);

        return base.DBConn.ExecuteOracleSP("PCK_VAPPCTCOMM.SaveCommEvent",
                                                pList);
    }

    /// <summary>
    /// US:834 gets a list of all checklist items for all open checklists
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetOpenPatChecklistItemDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_VAPPCTCOMM.GetOpenPatChecklistItemRS",
                                       pList,
                                       out ds);
        return status;
    }


    /// <summary>
    /// US:834 gets a list of all checklist collection items for all open checklists
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetOpenPatCLCollectionItemDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_VAPPCTCOMM.GetOpenPatCLCollectionItemRS",
                                       pList,
                                       out ds);
        return status;
    }

    /// <summary>
    /// US:834 Gets a dataset of all items for a single patient checklist
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatChecklistID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatChecklistItemDS( long lPatChecklistID,
                                          out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nPatChecklistID", lPatChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_VAPPCTCOMM.GetPatientChecklistItemRS",
                                       pList,
                                       out ds);
        return status;
    }


    /// <summary>
    /// US:834 Gets a dataset of all collection items for a single patient checklist
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatChecklistID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetPatientCLCollectionItemDS(long lPatChecklistID,
                                                out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nPatChecklistID", lPatChecklistID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_VAPPCTCOMM.GetPatientCLCollectionItemRS",
                                       pList,
                                       out ds);
        return status;
    }

}
