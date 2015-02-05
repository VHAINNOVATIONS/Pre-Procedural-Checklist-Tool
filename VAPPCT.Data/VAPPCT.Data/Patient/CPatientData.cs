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
/// contains methods and properties to retrieve and save patient data
/// </summary>
public class CPatientData : CData
{
    public CPatientData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    /// builds the patient blurb
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="strBlurb"></param>
    /// <returns></returns>
    public CStatus GetPatientBlurb(string strPatientID, out string strBlurb)
    {
        strBlurb = string.Empty;
        CPatientData p = new CPatientData(this);
        CPatientDataItem di = null;
        CStatus status = p.GetPatientDI(strPatientID, out di);
        if (status.Status)
        {
            strBlurb = di.FirstName + " " + di.LastName + " " + di.SSNLast4 + ", " + di.Age + " yo " + di.SexLabel;
        }

        return status;
    }

    /// <summary>
    /// saves a loaded patient to the database.
    /// pass in lUserID > 0 if you want this patient to
    /// belong to this provider
    /// </summary>
    /// <param name="pdi"></param>
    /// <returns></returns>
    public CStatus SavePatient(
        long lProviderUserID,
        long lTeamID,
        long lSpecialtyID,
        long lWardID,
        long lClinicID,
        long lXferSystemID,
        CPatientDataItem pdi)
    {
        if (pdi == null || String.IsNullOrEmpty(pdi.PatientID))
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "Could not save patient, invalid data!"); ;
        }

        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_vPatientID", pdi.PatientID);
        pList.AddInputParameter("pi_nProviderUserID", lProviderUserID);
        pList.AddInputParameter("pi_nTeamID", lTeamID);
        pList.AddInputParameter("pi_nSpecialtyID", lSpecialtyID);
        pList.AddInputParameter("pi_nWardID", lWardID);
        pList.AddInputParameter("pi_nClinicID", lClinicID);
        pList.AddInputParameter("pi_vSSN", pdi.SSN);
        pList.AddInputParameter("pi_dtDOB", pdi.DOB);
        pList.AddInputParameter("pi_vFirstName", pdi.FirstName);
        pList.AddInputParameter("pi_vFullName", pdi.FullName);
        pList.AddInputParameter("pi_vLastName", pdi.LastName);
        pList.AddInputParameter("pi_vMI", pdi.MI);
        pList.AddInputParameter("pi_nSex", (long)pdi.Sex);

        return DBConn.ExecuteOracleSP("PCK_PATIENT.SavePatient", pList);
    }


    /// <summary>
    /// method
    /// US:838
    /// retrieves a dataset of patients matching search criteria
    /// </summary>
    /// <param name="dtEventStartDate"></param>
    /// <param name="dtEventEndDate"></param>
    /// <param name="strLastName"></param>
    /// <param name="strLSSN"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lChecklistStatusID"></param>
    /// <param name="strTeamIDs"></param>
    /// <param name="strWardIDs"></param>
    /// <param name="strSpecialtyIDs"></param>
    /// <param name="strClinicIDs"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatientSearchDS(
        DateTime dtEventStartDate,
        DateTime dtEventEndDate,
        string strLastName,
        string strLSSN,
        long lChecklistID,
        long lChecklistStatusID,
        string strUsrID,
        string strTeamID,
        string strWardID,
        string strSpecialtyID,
        string strClinicID,
        long lServiceID, 
        out DataSet ds)
    {
        //initialize parameters
        ds = null;

        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //move patients from MDWS if needed
        if (MDWSTransfer)
        {
            string strMatch = String.Empty;
            if (!String.IsNullOrEmpty(strLSSN))
            {
                strMatch = strLSSN;
            }
            else
            {
                if (!String.IsNullOrEmpty(strLastName))
                {
                    strMatch = strLastName;
                }
            }

            if (!String.IsNullOrEmpty(strMatch))
            {
                long lCount = 0;
                CMDWSOps ops = new CMDWSOps(this);
                ops.GetMDWSMatchPatients(strMatch, out lCount);
            }
        }


        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_dtEventStartDate", dtEventStartDate);
        pList.AddInputParameter("pi_dtEventEndDate", dtEventEndDate);
        pList.AddInputParameter("pi_vLastName", strLastName);
        pList.AddInputParameter("pi_vLSSN", strLSSN);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nChecklistStatusID", lChecklistStatusID);
        pList.AddInputParameter("pi_vUsrID", strUsrID);
        pList.AddInputParameter("pi_vTeamID", strTeamID);
        pList.AddInputParameter("pi_vWardID", strWardID);
        pList.AddInputParameter("pi_vSpecialtyID", strSpecialtyID);
        pList.AddInputParameter("pi_vClinicID", strClinicID);
        pList.AddInputParameter("pi_nServiceID", lServiceID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_PATIENT.GetPatientSearchRS",
            pList,
            out ds);
    }


    /// <summary>
    /// method
    /// retrieves a dataset of patients matching the search criteria
    /// </summary>
    /// <param name="dtEventStartDate"></param>
    /// <param name="dtEventEndDate"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lChecklistStatusID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetMultiPatientSearchDS(
        DateTime dtEventStartDate,
        DateTime dtEventEndDate,
        long lChecklistID,
        long lChecklistStatusID,
        long lServiceID,
        out DataSet ds)
    {
        //initialize parameters
        ds = null;

        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_dtEventStartDate", dtEventStartDate);
        pList.AddInputParameter("pi_dtEventEndDate", dtEventEndDate);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nChecklistStatusID", lChecklistStatusID);
        pList.AddInputParameter("pi_nServiceID", lServiceID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            base.DBConn,
            "PCK_PATIENT.GetMultiPatientSearchRS",
            pList,
            out ds);
    }

    /// <summary>
    /// method
    /// retrieves a dataset of patient checklist ids matching the search criteria
    /// </summary>
    /// <param name="dtEventStartDate"></param>
    /// <param name="dtEventEndDate"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lChecklistStatusID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetMultiPatientPatCLIDSearchDS(
        DateTime dtEventStartDate,
        DateTime dtEventEndDate,
        long lChecklistID,
        long lChecklistStatusID,
        long lServiceID,
        out DataSet ds)
    {
        //initialize parameters
        ds = null;

        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_dtEventStartDate", dtEventStartDate);
        pList.AddInputParameter("pi_dtEventEndDate", dtEventEndDate);
        pList.AddInputParameter("pi_nChecklistID", lChecklistID);
        pList.AddInputParameter("pi_nChecklistStatusID", lChecklistStatusID);
        pList.AddInputParameter("pi_nServiceID", lServiceID);
        
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            base.DBConn,
            "PCK_PATIENT.GetMultiPatientPatCLIDSearchRS",
            pList,
            out ds);
    }

    /// <summary>
    /// get user patient ds
    /// </summary>
    /// <param name="lUserID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetUserPatientDS(long lUserID, out DataSet ds)
    {
        //initialize parameters
        ds = null;

        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //transfer patients from MDWS if we need to
        long lCount = 0;
        if (MDWSTransfer)
        {
            CMDWSOps MDWSOps = new CMDWSOps(this);
            status = MDWSOps.GetMDWSUserPatients(lUserID, out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_nProviderUserID", lUserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            DBConn,
            "PCK_PATIENT.GetUserPatientsRS",
            pList,
            out ds);
    }

    /// <summary>
    /// loads a patient data item by patient id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="itm"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatientDI(string strPatientID, out CPatientDataItem itm)
    {
        //initialize parameters
        itm = null;

        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_vPatientID", strPatientID);

        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            base.DBConn,
            "PCK_PATIENT.GetPatientIDRS",
            pList,
            out ds);

        if (!status.Status)
        {
            return status;
        }

        itm = new CPatientDataItem(ds);

        return status;
    }

    /// <summary>
    /// returns a dataset of patient item components
    /// filtered by patient checklist id (PAT_CL_ID)
    /// <param name="lPatCLID"></param>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetPatItemCompsByPatCLIDDS(long lPatCLID, out DataSet ds)
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

        pList.AddInputParameter("pi_nPatCLID", lPatCLID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            base.DBConn,
            "PCK_PATIENT.GetPatItemCompsByPatCLIDRS",
            pList,
            out ds);
    }

    /// <summary>
    /// returns a dataset of patient item components
    /// filtered by patient checklist id (PAT_CL_ID)
    public CStatus GetPatItemsByPatCLIDDS(long lPatCLID, out DataSet ds)
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

        pList.AddInputParameter("pi_nPatCLID", lPatCLID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(
            base.DBConn,
            "PCK_PATIENT.GetPatItemsByPatCLIDRS",
            pList,
            out ds);
    }

    /// <summary>
    /// returns the item component state base on the primary key
    /// </summary>
    /// <param name="lPatItemID"></param>
    /// <param name="lItemCompID"></param>
    /// <param name="lItemCompState"></param>
    /// <returns></returns>
    public CStatus GetItemCompStateByPKey(
        long lPatItemID,
        long lItemCompID,
        out long lItemCompState)
    {
        //initialize parameters
        lItemCompState = 0;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        pList.AddInputParameter("pi_nPatItemID", lPatItemID);
        pList.AddInputParameter("pi_nItemCompID", lItemCompID);
        pList.AddOutputParameter("po_nItemCompState", lItemCompState);

        //get the dataset
        DataSet ds = null;
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(
            base.DBConn,
            "PCK_PATIENT.GetItemCompStateByPKey",
            pList,
            out ds);

        if (!status.Status)
        {
            return status;
        }

        //get the Item Component State returned from the SP call
        lItemCompState = pList.GetParamLongValue("po_nItemCompState");

        return status;
    }
}
