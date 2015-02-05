using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using VAPPCT.DA;
using VAPPCT.Data.MDWSEmrSvc;
using VAPPCT.Data;

/// <summary>
/// Summary description for CMDWSOps
/// </summary>
public class CMDWSOps : CData
{
    //Constructor
    public CMDWSOps(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }

    protected EmrSvcSoapClient m_EmrSvcSoapClient = null;
    /// <summary>
    /// gets the soap client so we can talk to mdws
    /// </summary>
    /// <returns></returns>
    protected EmrSvcSoapClient GetMDWSSOAPClient()
    {
        //this is used from communicator also and we 
        //have no "Session" in communicator
        if (WebSession != null)
        {
            if (WebSession["EmrSvcSoapClient"] == null)
            {
                m_EmrSvcSoapClient = new EmrSvcSoapClient("EmrSvcSoap");
                WebSession["EmrSvcSoapClient"] = m_EmrSvcSoapClient;
            }

            return (EmrSvcSoapClient)WebSession["EmrSvcSoapClient"];
        }
        else
        {
            if(m_EmrSvcSoapClient == null)
            {
                m_EmrSvcSoapClient = new EmrSvcSoapClient("EmrSvcSoap");
            }

            return m_EmrSvcSoapClient;
        }        
    }

    /// <summary>
    /// US:840
    /// helper to determine if MDWS is valid
    /// </summary>
    /// <returns></returns>
    public CStatus IsMDWSValid()
    {
        //check the connection to MDWS, will attempt to reconnect
        //if necessary
        CAppUser appUser = new CAppUser(this);

        //get the users security keys, this is how we test the connection
        UserSecurityKeyArray usk = GetMDWSSOAPClient().getUserSecurityKeys(appUser.UserID.ToString());
        if (usk != null && usk.fault != null)
        {
            long lUserID = 0;
            EmrSvcSoapClient mdwsSOAPClient = null;
            CStatus status = MDWSLogin(
                appUser.MDWSUID.ToString(),
                appUser.MDWSPWD.ToString(),
                appUser.SiteID,
                out lUserID,
                out mdwsSOAPClient);
            if (!status.Status)
            {
                return status;
            }
        }

        /*todo: debug
        else
        {
            string strkeys = String.Empty;
            foreach (UserSecurityKeyTO to in usk.keys)
            {
                strkeys += to.name + "\r\n";
            }

            strkeys += "";
        }*/

        return new CStatus();
    }

    /// <summary>
    /// get sites
    /// </summary>
    /// <param name="ra"></param>
    /// <returns></returns>
    public CStatus GetMDWSSites()
    {
    //    CStatus status = IsMDWSValid();
    //    if (!status.Status)
    //    {
    //        return status;
    //    }

        CStatus status = new CStatus();

        //get the sites from MDWS
        RegionArray ra = GetMDWSSOAPClient().getVHA();
        if (ra == null || ra.fault != null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "getVHA failed!");
        }

        //transfer the patients to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferSites(ra);
        if (!status.Status)
        {
            return status;
        }
        
        return status;
    }


    /// <summary>
    /// get a list of rpcs
    /// </summary>
    /// <param name="strRPCs"></param>
    /// <returns></returns>
    public CStatus GetMDWSRPCs(out string strRPCs)
    {
        strRPCs = String.Empty;

        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the rpcs from MDWS
        TextArray ta = GetMDWSSOAPClient().getRpcs();
        if (ta == null || ta.fault != null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "getRPCs failed!");
        }

        foreach(string str in ta.text)
        {
            strRPCs += str + "\r\n";
        }

        return status;
    }

    /// <summary>
    /// US:1945 US:1880 US:885
    /// gets the next note title target
    /// </summary>
    /// <param name="tta"></param>
    /// <param name="strTarget"></param>
    /// <returns></returns>
    protected CStatus GetNextNoteTitleTarget(TaggedTextArray tta, out string strTarget)
    {
        strTarget = String.Empty;

        //get the index of the last item in the array
        int nCountArray = tta.results.Count();
        int nIndexArray = 0;
        if (nCountArray > 0)
        {
            nIndexArray = nCountArray - 1;
        }

        //get the index of the last item in the taggedResults of the array
        int nCountTagged = tta.results[nIndexArray].taggedResults.Count();
        int nIndexTagged = 0;
        if (nCountTagged > 0)
        {
            nIndexTagged = nCountTagged - 1;
        }

        //get the next target
        strTarget = tta.results[nIndexArray].taggedResults[nIndexTagged].textArray[0];

        return new CStatus();
    }

    /// <summary>
    /// US:1945 US:1880 US:885
    /// gets note titles from MDWS and transfers them to the db
    /// </summary>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSNoteTitles(out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //per Joel: You pass text to those calls corresponding to the 
        //name of the clinic/not title where you want to begin searching. 
        //So, say I wanted to get note titles for comp and pen notes, 
        //I would enter “comp” or something like that. 
        //The list appears longer because CPRS is automagically 
        //executing the same call over and over transparently to you 
        //as you scroll through the names. By default, those RPCs 
        //usually return 44 records per search starting with the name 
        //of the argument you supplied.
        //
        //get the first batch of notes
        TaggedTextArray tta = GetMDWSSOAPClient().getNoteTitles(string.Empty, "1");
        //return if tta is null
        if (tta == null || tta.fault != null)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "Failed to retrieve note titles!");
        }

        //loop and continue to get data until we have them all
        string strCurrentTarget = String.Empty;
        string strPreviousTarget = String.Empty;
        while (tta != null && tta.fault == null)
        {
            //get the next target for the search
            //if the current target = the previous target we are done
            //processing the array
            GetNextNoteTitleTarget(tta, out strCurrentTarget);
            if (strCurrentTarget == strPreviousTarget)
            {
                break;
            }

            //transfer this block of note titles
            long lCnt = 0;
            CMDWSTransfer transfer = new CMDWSTransfer(this);
            status = transfer.TransferNoteTitles(tta, out lCnt);
            if (!status.Status)
            {
                return status;
            }

            lCount += lCnt;

            //make the call again with the target and direction = "1" = forward
            tta = GetMDWSSOAPClient().getNoteTitles(strCurrentTarget, "1");

            //set the previous target = the current target
            strPreviousTarget = strCurrentTarget;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// gets teams from MDWS and moves them to the checklist db
    /// </summary>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSTeams(out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the teams from MDWS
        TaggedText txtTeams = GetMDWSSOAPClient().getTeams();
        if (txtTeams == null || txtTeams.fault != null)
        {
            //return new CMDWSStatus(txtTeams.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the patients to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferTeams(txtTeams, out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:866 gets MDWS security keys and transfers them to the database
    /// </summary>
    /// <param name="lUserID"></param>
    /// <param name="bTransfer"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSSecurityKeys(
        long lUserID,
        bool bTransfer,
        out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the teams from MDWS
        //get the users security keys
        UserSecurityKeyArray usk = GetMDWSSOAPClient().getUserSecurityKeys(lUserID.ToString());
        if (usk == null || usk.fault != null)
        {
            //return new CMDWSStatus(usk.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //in some cases we do not want to transfer the keys...
        if (bTransfer)
        {
            //transfer the patients to the checklist db
            CMDWSTransfer transfer = new CMDWSTransfer(this);
            status = transfer.TransferSecurityKeys(
                lUserID,
                usk,
                out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// gets specialties from MDWS and moves them to the checklist db
    /// </summary>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSSpecialties(out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the specialties from MDWS
        TaggedText txtSpecialties = GetMDWSSOAPClient().getSpecialties();
        if (txtSpecialties == null || txtSpecialties.fault != null)
        {
            //return new CMDWSStatus(txtSpecialties.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the data to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferSpecialties(txtSpecialties, out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// gets wards from MDWS and moves them to the checklist db
    /// </summary>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSWards(out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //todo: testing ////////////////////////////////////////////////////////////////////////
        //does not work crashes: TaggedMedicationArrays meds = GetMDWSSOAPClient().getAllMeds();
        //does not work crashes: TaggedMedicationArrays meds = GetMDWSSOAPClient().getImoMeds();
        //does not work crashes: TaggedMedicationArrays meds = GetMDWSSOAPClient().getIvMeds();
        //does not work crashes: TaggedMedicationArrays meds = GetMDWSSOAPClient().getOtherMeds();
        //does not work crashes: TaggedMedicationArrays meds = GetMDWSSOAPClient().getOutpatientMeds();
        //does not work crashes: TaggedMedicationArrays meds = GetMDWSSOAPClient().getUnitDoseMeds();
        //does not work crashes: TaggedAllergyArrays allergies = GetMDWSSOAPClient().getAllergies();
        //TaggedPatientArray t = GetMDWSSOAPClient().getPatientsByClinic("64");
        //tried for all clinics and get no patients back t = GetMDWSSOAPClient().getPatientsByClinic("133");

        //triedTaggedTextArray ar = GetMDWSSOAPClient().getPastClinicVisitsReports("19990101",
          //                                                                       "20120501",
            //                                                                     0);

        TaggedHospitalLocationArray thla = GetMDWSSOAPClient().getWards();
        if (thla == null || thla.fault != null)
        {
            //return new CMDWSStatus(thla.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the data to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferWards(thla, out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// gets clinics from MDWS and moves them to the checklist db
    /// </summary>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSClinics(out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //TODO: wire up target when I hear back from VA
        string strTarget = String.Empty;
        TaggedHospitalLocationArray thla = GetMDWSSOAPClient().getClinics(strTarget);
        if (thla == null || thla.fault != null)
        {
            //return new CMDWSStatus(thla.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the data to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferClinics(thla, out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// transfers patients by teams to the checklist db
    /// </summary>
    /// <param name="lTeamID"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSTeamPatients(long lTeamID, out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the teams from MDWS
        TaggedPatientArray tpa = GetMDWSSOAPClient().getPatientsByTeam(lTeamID.ToString());
        if (tpa == null || tpa.fault != null)
        {
            //return new CMDWSStatus(tpa.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the patients to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferPatientArray(
            string.Empty, 
            lTeamID, 
            0, 
            0, 
            0, 
            tpa, 
            out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// tranfer MDWS patients that match criteria
    /// </summary>
    /// <param name="strMatchID"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSMatchPatients(string strMatch, out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the teams from MDWS
        TaggedPatientArrays tpas = GetMDWSSOAPClient().match(strMatch);
        if (tpas == null || tpas.fault != null)
        {
            //return new CMDWSStatus(tpas.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        foreach (TaggedPatientArray tpa in tpas.arrays)
        {
            //transfer the patients to the checklist db
            CMDWSTransfer transfer = new CMDWSTransfer(this);
            status = transfer.TransferPatientArray(
                string.Empty,
                0,
                0,
                0,
                0,
                tpa,
                out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// transfers patients by specialty to the checklist db
    /// </summary>
    /// <param name="lSpecialtyID"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSSpecialtyPatients(long lSpecialtyID, out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the teams from MDWS
        TaggedPatientArray tpa = GetMDWSSOAPClient().getPatientsBySpecialty(lSpecialtyID.ToString());
        if (tpa == null || tpa.fault != null)
        {
            //return new CMDWSStatus(tpa.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the patients to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferPatientArray(
            string.Empty,
            0,
            lSpecialtyID,
            0,
            0,
            tpa,
            out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// transfers patients by ward to the checklist db
    /// </summary>
    /// <param name="lWardID"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSWardPatients(long lWardID, out long lCount)
    {
        //status
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the wards from MDWS
        TaggedPatientArray tpa = GetMDWSSOAPClient().getPatientsByWard(lWardID.ToString());
        if (tpa == null || tpa.fault != null)
        {
            //return new CMDWSStatus(tpa.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the patients to the checklist DB
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferPatientArray(
            string.Empty,
            0,
            0,
            lWardID,
            0,
            tpa,
            out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// US:838
    /// transfers patients by clinic to the checklist db
    /// </summary>
    /// <param name="lClinicID"></param>
    /// <param name="dtApptFrom"></param>
    /// <param name="dtApptTo"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSClinicPatients(
        long lClinicID,
        DateTime dtApptFrom,
        DateTime dtApptTo,
        out long lCount)
    {
        //status
        lCount = 0;
       
        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the wards from MDWS
        TaggedPatientArray tpa = GetMDWSSOAPClient().getPatientsByClinic(Convert.ToString(lClinicID));
        if (tpa == null || tpa.fault != null)
        {
            //return new CMDWSStatus(tpa.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the patients to the checklist DB
        CMDWSTransfer transfer = new CMDWSTransfer(this);

        status = transfer.TransferPatientArray(
            string.Empty,
            0,
            0,
            0,
            lClinicID,
            tpa,
            out lCount);
        if (!status.Status)
        {
            return status;
        }
       
        return new CStatus();
    }

    /// <summary>
    /// transfers users containing the specified search string
    /// </summary>
    /// <param name="strSearch"></param>
    /// <returns></returns>
    public CStatus GetMDWSUsers(string strSearch)
    {
        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        UserArray ua = GetMDWSSOAPClient().cprsUserLookup(strSearch);
        if (ua == null || ua.fault != null)
        {
            //return CMDWSStatus(ua.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the patients to the checklist db
        long lCount = 0;
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferUserArray(ua, out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();

    }

    /// <summary>
    /// US:838
    /// retrieves a list of the users patients and transfers 
    /// them to the checklist db
    /// </summary>
    /// <param name="lUserID"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus GetMDWSUserPatients(long lUserID, out long lCount)
    {
        lCount = 0;

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        //get the patients from MDWS
        string strDUZ = Convert.ToString(lUserID);
        TaggedPatientArray tpa = GetMDWSSOAPClient().getPatientsByProvider(strDUZ);
        if (tpa == null || tpa.fault != null)
        {
            //return new CMDWSStatus(tpa.fault);
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //transfer the patients to the checklist db
        CMDWSTransfer transfer = new CMDWSTransfer(this);
        status = transfer.TransferPatientArray(
            strDUZ,
            0,
            0,
            0,
            0,
            tpa,
            out lCount);
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// another version of login that takes additional params 
    /// used from the communicator and and clients that store sitelist and 
    /// context in places other than a config file
    /// </summary>
    /// <param name="strUN"></param>
    /// <param name="strPA"></param>
    /// <param name="strSiteList"></param>
    /// <param name="strContext"></param>
    /// <param name="toUser"></param>
    /// <returns></returns>
    protected CStatus Login(
        string strUN,
        string strPA,
        string strSiteList,
        string strContext,
        out UserTO toUser)
    {
        toUser = null;
       
        try
        {
            //connect to MDWS
            DataSourceArray dsa = GetMDWSSOAPClient().connect(strSiteList);
            if (dsa == null || dsa.fault != null && dsa.fault.message != "You are already connected to that site")
            {
                //return new CMDWSStatus(dsa.fault);
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            //login to mdws
            toUser = GetMDWSSOAPClient().login(
                strUN,
                strPA,
                strContext);
            if (toUser == null || toUser.fault != null)
            {
                //return new CMDWSStatus(toUser.fault);
                return new CStatus(false, k_STATUS_CODE.Failed, ErrorMessages.ERROR_LOGIN);
            }
        }
        catch (Exception e)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, e.Message);
        }

        return new CStatus();
    }

    /// <summary>
    /// US:840
    /// helper to connect and login to MDWS
    /// </summary>
    /// <param name="strUN"></param>
    /// <param name="strPA"></param>
    /// <param name="toUser"></param>
    /// <param name="mdws"></param>
    /// <returns></returns>
    protected CStatus Login(
        string strUN,
        string strPA,
        long lSiteID,
        out UserTO toUser,
        out EmrSvcSoapClient mdws)
    {
        toUser = null;
        mdws = null;

        //mdws
        try
        {
            mdws = new EmrSvcSoapClient("EmrSvcSoap");
        }
        catch (Exception e)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, e.Message);
        }

        //initialize sitelist and context
        //todo: using appsettings unencrypted
        string strSiteList = Convert.ToString(lSiteID);
        //site id is now passed in because user chooses it from 
        //the login dialog.
        //ConfigurationSettings.AppSettings["MDWSEmrSvcSiteList"];
        
        string strContext = ConfigurationSettings.AppSettings["MDWSEmrSvcContext"];

        try
        {
            //connect to MDWS
            DataSourceArray dsa = mdws.connect(strSiteList);
            if (dsa == null || dsa.fault != null && dsa.fault.message != "You are already connected to that site")
            {
                if (dsa.fault != null)
                {
                    return new CMDWSStatus(dsa.fault);
                }
                else
                {
                    return new CStatus(false, k_STATUS_CODE.Failed, "An undefined error occurred while connecting to MDWS!");
                }
            }

            //disconnect
            mdws.disconnect();

            //try to re-connect
            dsa = mdws.connect(strSiteList);
            if (dsa == null || dsa.fault != null)
            {
                //return new CMDWSStatus(dsa.fault);
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            //login to mdws
            toUser = mdws.login(
                strUN,
                strPA,
                strContext);

            //todo: just a reminder....
            strUN = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            strPA = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            if (toUser == null || toUser.fault != null)
            {
                //return new CMDWSStatus(toUser.fault);
                return new CStatus(false, k_STATUS_CODE.Failed, ErrorMessages.ERROR_LOGIN);
            }
        }
        catch (Exception e)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, e.Message);
        }

        return new CStatus();
    }

    /// <summary>
    /// US:840
    /// login to MDWS and transfer user to the checklist db
    /// </summary>
    /// <param name="strUID"></param>
    /// <param name="strPWD"></param>
    /// <param name="lUserID"></param>
    /// <param name="mdwsSOAPClient"></param>
    /// <returns></returns>
    public CStatus MDWSLogin(
        string strUID,
        string strPWD,
        long lSiteID,
        out long lUserID,
        out EmrSvcSoapClient mdwsSOAPClient)
    {
        //status
        lUserID = 0;
        mdwsSOAPClient = null;

        //login to mdws
        UserTO toUser = null;
        CStatus status = Login(
            strUID,
            strPWD,
            lSiteID,
            out toUser,
            out mdwsSOAPClient);
        if (!status.Status)
        {
            return status;
        }

        //transfer the user data to the checklist db
        if (status.StatusCode != k_STATUS_CODE.NothingToDo)
        {
            CMDWSTransfer transfer = new CMDWSTransfer(this);
            status = transfer.TransferUser(toUser, out lUserID);
            if (!status.Status)
            {
                mdwsSOAPClient.disconnect();
                mdwsSOAPClient = null;
                return status;
            }
        }

        //note: BaseMster.MDWSEmrSvcClient gets cached in session state
        //if the login was successful
        //if we get here we are logged in

        return new CStatus();
    }

    /// <summary>
    /// US:1952 US:1945 US:885
    /// write a note to MDWS
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="strAuthorID"></param>
    /// <param name="strVisitLocationID"></param>
    /// <param name="strNoteTitleIEN"></param>
    /// <param name="strNoteText"></param>
    /// <returns></returns>
    public CStatus WriteNote(
        string strPatientID,
        string strAuthorID,
        string strESignature,
        string strVisitLocationID,
        string strNoteTitleIEN,
        string strNoteText)
    {
        //select the patient
        PatientTO toSelect = GetMDWSSOAPClient().select(strPatientID);

        // easy way to make sure we're using new value each test run -- yes 
        //this will create a lot of notes...
        DateTime dtnow = DateTime.Now;
        string strVisitTimeStamp = dtnow.ToString("yyyyMMdd.HHmmss");

        //encounter
        // create our encounter string
        // use "A" not a historical note, "H" if it was...
        string strEncounter = strVisitLocationID + ";" + strVisitTimeStamp + ";H";

        string strCosigner = String.Empty;
        string strConsultIEN = String.Empty;
        string strPrfIEN = String.Empty;

        //before we start any note writing we need to verify the 
        //esignature so that we dont have dangling notes if they 
        //enter a bad esig one time
        TextTO toTxt = GetMDWSSOAPClient().isValidEsig(strESignature);
        if (toTxt.text == null || toTxt.text.ToUpper() != "TRUE")
        {
            return new CStatus(false,
                                k_STATUS_CODE.Failed,
                                "Invalid Signature Code!");
        }

        //write the note
        //
        //For preserving line breaks within a text block that is 
        //being written to a note, replace the \r\n or \n characters 
        //with a pipe (|).
        //
        NoteResultTO noteResult = GetMDWSSOAPClient().writeNote(
            strNoteTitleIEN,
            strEncounter,
            strNoteText.Replace("\r\n", "|"),
            strAuthorID,
            strCosigner,
            strConsultIEN,
            strPrfIEN);

        //check for error while writing note...
        if (noteResult == null || noteResult.fault != null)
        {
            return new CMDWSStatus(noteResult.fault);
        }

        //sign the note
        TextTO toSign = GetMDWSSOAPClient().signNote(noteResult.id,
                                                     strAuthorID,
                                                     strESignature);
        
        //check for sign error and return
        if (toSign.fault != null)
        {
            return new CMDWSStatus(toSign.fault);
        }
       
        //close the note
        TextTO closeNoteResult = GetMDWSSOAPClient().closeNote(noteResult.id, string.Empty);
        
        //check for close error and return
        if (closeNoteResult == null || closeNoteResult.fault != null)
        {
            return new CMDWSStatus(closeNoteResult.fault);
        }

/////////////////////////////////////////////////////////////        
//todo: remove this code later - test pulling back notes
////////////////////////////////////////////////////////////

        //select the patient
/*       PatientTO toP = GetMDWSSOAPClient().select(strPatientID);

        //todate is today
        DateTime dtToDate = DateTime.Now;
        string strToDate = CDataUtils.GetMilitaryDateAsString(dtToDate);

        //fromdate is today - lookback
        int nLookBackDays = 9999;
        DateTime dtFromDate = dtToDate.AddDays(-1 * nLookBackDays);
        string strFromDate = CDataUtils.GetMilitaryDateAsString(dtFromDate);

        //get the notes
        TaggedNoteArrays tnas = GetMDWSSOAPClient().getNotesWithText( strFromDate,
                                                                      strToDate,
                                                                      0);

        string strNotes = String.Empty;

        if(tnas != null)
        {
            foreach (TaggedNoteArray tna in tnas.arrays)
            {
                if (tna.notes != null)
                {
                    foreach (NoteTO note in tna.notes)
                    {
                        strNotes += String.Empty;
                        strNotes += note.timestamp;
                        strNotes += "\r\n";
                        strNotes += note.text;
                        strNotes += "\r\n\r\n";
                    }
                }
            }
        }
*/
//////////////////////////////////////////////////////////////        
//end pulling back notes
/////////////////////////////////////////////////////////////

        return new CStatus();
    }

    /// <summary>
    /// US:852
    /// transfers static lab test information from mdws to the vappct database
    /// </summary>
    /// <param name="strSearch"></param>
    /// <returns></returns>
    public CStatus GetMDWSLabTests(string strSearch)
    {
        if (String.IsNullOrEmpty(strSearch))
        {
            return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
        }

        //check to make sure the MDWS connection is valid
        CStatus status = IsMDWSValid();
        if (!status.Status)
        {
            return status;
        }

        string strNextSearch = strSearch.Substring(0, 1).ToUpper();
        string strSrch = strNextSearch;
        while(true)
        {
            TaggedLabTestArrays la = GetMDWSSOAPClient().getLabTests(strSrch);
            if (la == null || la.fault != null)
            {
                //return new CMDWSStatus(la.fault);
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
            }

            //transfer the patients to the checklist db
            long lCount = 0;
            CMDWSTransfer transfer = new CMDWSTransfer(this);
            status = transfer.TransferLabTestArray(
                la,
                out lCount,
                out strNextSearch);
            if (!status.Status)
            {
                return status;
            }
            
            /*
             * this was not working?
             * if (String.IsNullOrEmpty(strNextSearch)
                || strNextSearch.Substring(0, 1).ToUpper() == strSearch.Substring(0, 1).ToUpper())
            {
                break;
            }*/

            if (String.IsNullOrEmpty(strNextSearch))
            {
                break;
            }

            if (strNextSearch.Substring(0, 1).ToUpper() != strSearch.Substring(0, 1).ToUpper())
            {
                break;
            }

            strSrch = strNextSearch;
        }

        return status;
    }
}
