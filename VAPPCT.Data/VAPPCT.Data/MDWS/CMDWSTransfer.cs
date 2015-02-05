using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Configuration;

//our data access library
using VAPPCT.DA;

//MDWS web service
using VAPPCT.Data.MDWSEmrSvc;

/// <summary>
/// Summary description for CMDWSTransfer
/// </summary>
public class CMDWSTransfer: CData
{
    //Constructor
    public CMDWSTransfer(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }

    EmrSvcSoapClient m_EmrSvcSoapClient = null;
    /// <summary>
    /// gets the soap client so we can talk to mdws
    /// </summary>
    /// <returns></returns>
    EmrSvcSoapClient GetMDWSSOAPClient()
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
            if (m_EmrSvcSoapClient == null)
            {
                m_EmrSvcSoapClient = new EmrSvcSoapClient("EmrSvcSoap");
            }

            return m_EmrSvcSoapClient;
        }
    }

/*
    /// <summary>
    /// delete later
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetZZTestDS( string strMDWSCall,
                                out DataSet ds)
    {
        CStatus status = new CStatus();

        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        status = base.DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vMDWSCall", strMDWSCall); 
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                     "PCK_MDWS.GetZZTestRS",
                                     pList,
                                     out ds
                                     );
        if (!status.Status)
        {
            return status;
        }

        return status;
    }

    //todo: delete later
    public void InsertZZTEst(   string pi_vPatientID,
                                string pi_vMDWSCall,
                                string pi_vTest,
                                string pi_vValue,
                                string pi_vTestID)
    {

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_vPatientID", pi_vPatientID);
        pList.AddInputParameter("pi_vMDWSCall", pi_vMDWSCall);
        pList.AddInputParameter("pi_vTest", pi_vTest);
        pList.AddInputParameter("pi_vValue", pi_vValue);
        pList.AddInputParameter("pi_vTestID", pi_vTestID);

        CStatus status = new CStatus();

        status = base.DBConn.ExecuteOracleSP("PCK_MDWS.InsertZZTest",
                                           pList);

    }

    //todo: deletelater
    public void LoopPatients(int nOption)
    {
        //add sp cll to get MDWS patients

        //UserArray ua = BaseMster.MDWSEmrSvcClient.cprsUserLookup("20001");
        
        DataSet ds = null;
        CStatus status = new CStatus();
        CDataUtils utils = new CDataUtils();
        string strDFNs = String.Empty;

        status = GetMDWSPatientDS(out ds);
        if (ds != null)
        {
            utils.GetDSDelimitedData(ds,
                                     "patient_id",
                                     ",",
                                     out strDFNs);
        }
     
        string[] splitDFNs = strDFNs.Split(',');
        foreach (string strDFN in splitDFNs)
        {
            PatientTO toSelect = BaseMster.MDWSEmrSvcClient.select(strDFN);
            //if (toSelect.fault == null)
            //{
            //    //TransferPatient(Convert.ToString(BaseMster.UserID), toSelect);
            //}

            //getChemHemReports
            if (nOption == 1)
            {
                if (toSelect.fault == null)
                {
                    TaggedChemHemRptArrays ChemHemArrays =
                    BaseMster.MDWSEmrSvcClient.getChemHemReports("19990101",
                                                                 "20120303",
                                                                  0);
                    if (ChemHemArrays.fault == null)
                    {
                        for (int i = 0; i < ChemHemArrays.count; i++)
                        {
                            TaggedChemHemRptArray ChemHemArray = ChemHemArrays.arrays[i];
                            if (ChemHemArray.rpts != null)
                            {
                                foreach (ChemHemRpt rpt in ChemHemArray.rpts)
                                {
                                    foreach (LabResultTO result in rpt.results)
                                    {
                                        LabTestTO toLab = result.test;
                                        if (toLab.fault == null)
                                        {

                                        }

                                        //InsertZZTEst( toSelect.localPid,
                                          //            "getChemHemReports",
                                            //          toLab.name,
                                              //        result.value,
                                                //      toLab.id);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //////////////////////////////////////////////////////////
            if (nOption == 2)//getChemHemReportsSimple 
            {
                return; //using full above

                if (toSelect.fault == null)
                {
                    TaggedChemHemRptArrays ChemHemArrays =
                    BaseMster.MDWSEmrSvcClient.getChemHemReportsSimple("19990101",
                                                                       "20120303",
                                                                       0);
                    if (ChemHemArrays.fault == null)
                    {
                        for (int i = 0; i < ChemHemArrays.count; i++)
                        {
                            TaggedChemHemRptArray ChemHemArray = ChemHemArrays.arrays[i];
                            if (ChemHemArray.rpts != null)
                            {
                                foreach (ChemHemRpt rpt in ChemHemArray.rpts)
                                {
                                    foreach (LabResultTO result in rpt.results)
                                    {
                                        LabTestTO toLab = result.test;
                                        if (toLab.fault == null)
                                        {

                                        }

                                        string strResult = result.value;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ///////////////////////////////////////////////////////////
             
            //////////////////////////////////////////////////////////
            //getCytologyReports
            if (nOption == 3)
            {
                if (toSelect.fault == null)
                {
                    TaggedCytologyRptArrays CytoArrays =
                    BaseMster.MDWSEmrSvcClient.getCytologyReports("19990101",
                                                                  "20120303",
                                                                   0);
                    if (CytoArrays.fault == null)
                    {
                        for (int i = 0; i < CytoArrays.count; i++)
                        {
                            TaggedCytologyRptArray CytoArray = CytoArrays.arrays[i];
                            if (CytoArray.rpts != null)
                            {
                                foreach (CytologyRpt rpt in CytoArray.rpts)
                                {
                                    if (rpt.specimen != null)
                                    {
                                       // InsertZZTEst(toSelect.localPid,
                                         //            "getCytologyReports",
                                            //          rpt.title,
                                              //        rpt.specimen.collectionDate + " " + rpt.specimen.name,
                                                //      "");
                                    }
                                    else
                                    {

                                        //InsertZZTEst(toSelect.localPid,
                                          //           "getCytologyReports",
                                            //          rpt.title,
                                              //       rpt.specimen.collectionDate + " " + rpt.specimen.name,
                                                 //   "?");

                                        int k = 0;
                                    }
                                }

                            }
                        }
                    }
                }
            }
                    
            ///////////////////////////////////////////////////////////
            if (nOption == 4)
            {
                if (toSelect.fault == null)
                {
                    TaggedMicrobiologyRptArrays microArrays = 
                    BaseMster.MDWSEmrSvcClient.getMicrobiologyReports("19990101",
                                                                      "20120303",
                                                                       0);
                    if (microArrays.fault == null)
                    {
                        for (int i = 0; i < microArrays.count; i++)
                        {
                            TaggedMicrobiologyRptArray microArray = microArrays.arrays[i];
                            if (microArray.fault == null)
                            {
                                if (microArray.rpts != null)
                                {
                                    foreach (MicrobiologyRpt rpt in microArray.rpts)
                                    {
                                       // InsertZZTEst(toSelect.localPid,
                                         //            "getMicrobiologyReports",
                                            //         rpt.title,
                                              //       rpt.sample,
                                                 //    "");

                                    }
                                }
                            }
                        }
                    }
                }
            }


            ///////////////////////////////////////////////////////////
            if (nOption == 5)
            {
                if (toSelect.fault == null)
                {
                    try
                    {

                        TaggedOrderArrays OrderArrays = BaseMster.MDWSEmrSvcClient.getAllOrders();
                        if (OrderArrays.fault == null)
                        {
                            for (int i = 0; i < OrderArrays.count; i++)
                            {
                                TaggedOrderArray OrderArray = OrderArrays.arrays[i];
                                if (OrderArray.fault == null)
                                {
                                    foreach (OrderTO toOrder in OrderArray.items)
                                    {
                                        
                                        string strProvider = "";
                                        if (toOrder.provider != null)
                                        {
                                            strProvider = toOrder.provider.name;
                                        }

                                        //InsertZZTEst(toSelect.localPid,
                                          //           "getAllOrders",
                                            //         toOrder.text,
                                              //       toOrder.status + " " + toOrder.timestamp + " " + strProvider,
                                                //     toOrder.id);
                                    }

                                }
                            }
                        }
                    }
                   
                    catch (Exception e)
                    {

                    }
                }
                
            }

            ///////////////////////////////////////////////////////////
            if (nOption == 6)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedRadiologyReportArrays radArrays = 
                        BaseMster.MDWSEmrSvcClient.getRadiologyReports("19990101",
                                                                   "20120303",
                                                                    0);
                        if (radArrays.fault == null)
                        {
                            for (int i = 0; i < radArrays.count; i++)
                            {
                                TaggedRadiologyReportArray radArray = radArrays.arrays[i];
                                if (radArray.fault == null)
                                {
                                    if (radArray.rpts != null)
                                    {
                                        foreach (RadiologyReportTO rpt in radArray.rpts)
                                        {
                                            if (rpt.cptCode != null)
                                            {
                                                int j = 0;
                                                j++;

                                            }
                                          //  InsertZZTEst(toSelect.localPid,
                                            //                "getRadiologyReports",
                                              //              rpt.title,
                                                //            rpt.status + " " +rpt.timestamp,
                                                  //          "");

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) { };
                }
            }
            ///////////////////////////////////////////////////////////

            if (nOption == 7)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedVitalSignArrays vitalArrays =
                        BaseMster.MDWSEmrSvcClient.getLatestVitalSigns();

                        if (vitalArrays.fault == null)
                        {
                            for (int i = 0; i < vitalArrays.count; i++)
                            {
                                TaggedVitalSignArray vitalArray = vitalArrays.arrays[i];
                                if (vitalArray.fault == null)
                                {
                                    if (vitalArray.vitals != null)
                                    {
                                        foreach (VitalSignTO rpt in vitalArray.vitals)
                                        {
                                          
                                            //  InsertZZTEst(toSelect.localPid,
                                            //                "getRadiologyReports",
                                            //              rpt.title,
                                            //            rpt.status + " " +rpt.timestamp,
                                            //          "");

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) { };
                }
            }

            if (nOption == 8)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedVitalSignSetArrays vitalArrays =
                        BaseMster.MDWSEmrSvcClient.getVitalSigns();

                        if (vitalArrays.fault == null)
                        {
                            for (int i = 0; i < vitalArrays.count; i++)
                            {
                                TaggedVitalSignSetArray vitalArray = vitalArrays.arrays[i];
                                if (vitalArray.fault == null)
                                {
                                    if (vitalArray.sets != null)
                                    {
                                        foreach (VitalSignSetTO rpt in vitalArray.sets)
                                        {
                                       
                                            //  InsertZZTEst(toSelect.localPid,
                                            //                "getRadiologyReports",
                                            //              rpt.title,
                                            //            rpt.status + " " +rpt.timestamp,
                                            //          "");

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) { };
                }
            }
            ///////////////////////////////////////////////////////////
            if (nOption == 9)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedIcdRptArrays icdArrays =
                        BaseMster.MDWSEmrSvcClient.getIcdProceduresReports("19890101",
                                                                            "20120303",
                                                                            0);

                        if (icdArrays.fault == null)
                        {
                            for (int i = 0; i < icdArrays.count; i++)
                            {
                                TaggedIcdRptArray icdArray = icdArrays.arrays[i];
                                if (icdArray != null)
                                {
                                    if (icdArray.fault == null)
                                    {
                                        if (icdArray.rpts != null)
                                        {
                                            foreach (IcdRpt rpt in icdArray.rpts)
                                            {
                                                int x = 12;
                                                x++;



                                                //  InsertZZTEst(toSelect.localPid,
                                                //                "getRadiologyReports",
                                                //              rpt.title,
                                                //            rpt.status + " " +rpt.timestamp,
                                                //          "");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) { };
                }
            }

            ///////////////////////////////////////////////////////////

            //getIcdSurgeryReports 
            if (nOption == 10)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedIcdRptArrays icdArrays =
                        BaseMster.MDWSEmrSvcClient.getIcdSurgeryReports("19990101",
                                                                            "20120303",
                                                                            0);

                        if (icdArrays.fault == null)
                        {
                            for (int i = 0; i < icdArrays.count; i++)
                            {
                                TaggedIcdRptArray icdArray = icdArrays.arrays[i];
                                if (icdArray != null)
                                {
                                    if (icdArray.fault == null)
                                    {
                                        if (icdArray.rpts != null)
                                        {
                                            foreach (IcdRpt rpt in icdArray.rpts)
                                            {
                                                int x = 12;
                                                x++;
                                                
                                                //  InsertZZTEst(toSelect.localPid,
                                                //                "getRadiologyReports",
                                                //              rpt.title,
                                                //            rpt.status + " " +rpt.timestamp,
                                                //          "");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) { };
                }
            }

            if (nOption == 11)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedTextArray micronArray =
                        BaseMster.MDWSEmrSvcClient.getElectronMicroscopyReports("19990101",
                                                                                "20120303",
                                                                                0);

                        if (micronArray.fault == null)
                        {
                            for (int i = 0; i < micronArray.count; i++)
                            {
                                TaggedText micronText = micronArray.results[i];
                                if (micronText != null)
                                {
                                    if (micronText.fault == null)
                                    {
                                        if (!String.IsNullOrEmpty(micronText.text))
                                        {
                                            int k = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) { };
                }
            }


            if (nOption == 12)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedMedicationArrays meds =
                        BaseMster.MDWSEmrSvcClient.getImoMeds();
                        if (meds.arrays != null)
                        {
                            foreach (TaggedMedicationArray med in meds.arrays)
                            {
                                foreach (MedicationTO toMed in med.meds)
                                {
                                    if (toMed != null)
                                    {
                                        int kf = 0;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }
    }

    //end delete
    */

    /// <summary>
    /// US:838
    /// get mdws patients
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetMDWSPatientDS(out DataSet ds)
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
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                      "PCK_MDWS.GetMDWSPatientRS",
                                      pList,
                                      out ds);
        if (!status.Status)
        {
            return status;
        }

        return status;
    }

    /// <summary>
    /// transfer regions and sites from MDWS to our tables
    /// </summary>
    /// <param name="ra"></param>
    /// <returns></returns>
    public CStatus TransferSites(RegionArray ra)
    {
        CStatus status = new CStatus();

        foreach (RegionTO regionbTo in ra.regions)
        {
            string strRegionID = regionbTo.id;
            if (String.IsNullOrEmpty(strRegionID))
            {
                strRegionID = "";
            }

            string strRegionName = regionbTo.name;
            if (String.IsNullOrEmpty(strRegionName))
            {
                strRegionName = "";
            }
            
            //save the region
            CSiteData siteData = new CSiteData(this);
            siteData.SaveRegion(1,
                                CDataUtils.ToLong(strRegionID),
                                strRegionName);
            
            for (int i = 0; i < regionbTo.sites.count; i++)
            {
                string strSiteCode = regionbTo.sites.sites[i].sitecode;
                if (String.IsNullOrEmpty(strSiteCode))
                {
                    strSiteCode = "";
                }

                string strSiteName = regionbTo.sites.sites[i].displayName;
                if (String.IsNullOrEmpty(strSiteName))
                {
                    strSiteName = "";
                }

                //save the site
                siteData.SaveSite(1,
                                  CDataUtils.ToLong(strRegionID),
                                  CDataUtils.ToLong(strSiteCode),
                                  strSiteName);

            }
        }


        return status;

    }


    /// <summary>
    /// US:1945 transfers patient notes from MDWS to our tables
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="strNoteTitle"></param>
    /// <param name="nLookBackDays"></param>
    /// <returns></returns>
    public CStatus TransferPatientNotes(string strPatientID,
                                        string strNoteTitle,
                                        long lItemID,
                                        int nLookBackDays)
    {
        CStatus status = new CStatus();

        //select the patient
        PatientTO toSelect = GetMDWSSOAPClient().select(strPatientID);
        
        //nothing to do
        if (toSelect == null)
        {
            return status;
        }

        //end if we have a fault
        if (toSelect.fault != null)
        {
            return new CMDWSStatus(toSelect.fault);
        }
        
        try
        {
            //todate is today
            DateTime dtToDate = DateTime.Now;
            string strToDate = CDataUtils.GetMilitaryDateAsString(dtToDate);

            //fromdate is today - lookback
            DateTime dtFromDate = dtToDate.AddDays(-1 * nLookBackDays);
            string strFromDate = CDataUtils.GetMilitaryDateAsString(dtFromDate);

            //todo: testing, our new notes are not signed so it appears that they cant be 
            //retrieved from getNotesWithText but they do retrieve by using getUnsignedNotes
            //TaggedNoteArrays tnaUnsigned = GetMDWSSOAPClient().getUnsignedNotes(strFromDate,
            //                                                                    strToDate,
            //                                                                    0);

            //get the notes
            TaggedNoteArrays tnas = GetMDWSSOAPClient().getNotesWithText(strFromDate,
                                                                         strToDate,
                                                                         0);
            if (tnas == null)
            {
                return status;
            }
            
            if (tnas.fault != null)
            {
                return new CMDWSStatus(toSelect.fault);
            }

            foreach (TaggedNoteArray tna in tnas.arrays)
            {
                if (tna.notes != null)
                {
                    foreach (NoteTO note in tna.notes)
                    {
                        string strLocalT = note.localTitle.ToLower().Trim();
                        strLocalT = strLocalT.Replace("<", "[");
                        strLocalT = strLocalT.Replace(">", "]");

                        if (strLocalT ==
                            strNoteTitle.ToLower().Trim())
                        {
                            //create a new patient item
                            CPatientItemDataItem di = new CPatientItemDataItem();
                            di.PatientID = strPatientID;
                            di.ItemID = lItemID;
                            di.SourceTypeID = (long)k_SOURCE_TYPE_ID.VistA;
                            di.EntryDate = CDataUtils.GetMDWSDate(note.timestamp);
                            
                            //build a pat item component to hold the note text
                            CPatientItemComponentDataItem cdi = new CPatientItemComponentDataItem();
                            cdi.PatientID = strPatientID;
                            cdi.ItemID = lItemID;

                            //need to get the component given the item
                            DataSet dsItemComps = null;
                            CItemComponentData cid = new CItemComponentData(this);
                            cid.GetItemComponentDS(lItemID, k_ACTIVE_ID.All, out dsItemComps);
                            if (dsItemComps != null)
                            {
                                //note titles only have 1 component.
                                cdi.ComponentID = CDataUtils.GetDSLongValue(dsItemComps, "ITEM_COMPONENT_ID");
                            }


                            string strNoteText = note.text;

                            //todo: remove this temp fix when we switch to clob
                            if (strNoteText.Length > 4000)
                            {
                                strNoteText = strNoteText.Substring(0, 3995);
                                strNoteText += "...";
                            }

                            cdi.ComponentValue = strNoteText;
                            cdi.EntryDate = di.EntryDate;
                                                        
                            //create a new item component list with 1 component 
                            //for the note text
                            CPatientItemCompList comps = new CPatientItemCompList();
                            comps.Add(cdi);

                            //check to see if the note already exists
                            //in the vappct database, if so we want to update 
                            //it, else insert it
                            bool bInsert = true;
                            DataSet dsExists = null;
                            CPatientItemData itemData = new CPatientItemData(this);
                            status = itemData.GetPatItemCompDS( strPatientID,
                                                                di.ItemID,
                                                                di.EntryDate,
                                                                out dsExists);
                            if (!status.Status)
                            {
                                //todo:
                                return status;
                            }

                            if (!CDataUtils.IsEmpty(dsExists))
                            {
                                bInsert = false;
                            }

                            if (!bInsert)
                            {
                                di.PatItemID = CDataUtils.GetDSLongValue(dsExists, "pat_item_id");
                                cdi.PatItemID = di.PatItemID;
                                
                                //todo: just testing
                                //cdi.ComponentValue = "UPDATED: " + cdi.ComponentValue;

                                //update the record 
                                status = itemData.UpdatePatientItem(di,
                                                                    comps);
                                if (!status.Status)
                                {
                                    //todo:
                                }

                    //            CPatientChecklistLogic pcll = new CPatientChecklistLogic(this);
                      //          pcll.RunLogic(di.PatientID, di.ItemID);
                            }
                            else
                            {
                                //add the record (insert only todo need to
                                //handle dupes in the sp maybe have it update)
                                long lPatItemID = 0;
                                status = itemData.InsertPatientItem(di,
                                                                    comps,
                                                                    out lPatItemID);
                                if (!status.Status)
                                {
                                    //todo:
                                }

                        //        CPatientChecklistLogic pcll = new CPatientChecklistLogic(this);
                          //      pcll.RunLogic(di.PatientID, di.ItemID);
                            }

                            //cleanup
                            di = null;
                            cdi = null;
                            comps = null;
                            itemData = null;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = e.Message;
            return status;
        }


        //now that we inserted all the items run the logic
        CPatientChecklistLogic pcll = new CPatientChecklistLogic(this);
        pcll.RunLogic(strPatientID, lItemID);

        return status;
    }

    /// <summary>
    /// US:852 transfer patient labs from MDWS
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="strNoteTitle"></param>
    /// <param name="lItemID"></param>
    /// <param name="nLookBackDays"></param>
    /// <returns></returns>
    public CStatus TransferPatientLabs( string strPatientID,
                                        string strMapID,
                                        long lItemID,
                                        int nLookBackDays)
    {
        CStatus status = new CStatus();

        //select the patient
        PatientTO toSelect = GetMDWSSOAPClient().select(strPatientID);

        //nothing to do
        if (toSelect == null)
        {
            return status;
        }

        //end if we have a fault
        if (toSelect.fault != null)
        {
            return new CMDWSStatus(toSelect.fault);
        }

        try
        {
            //todate is today
            DateTime dtToDate = DateTime.Now;
            string strToDate = CDataUtils.GetMilitaryDateAsString(dtToDate);

            //fromdate is today - lookback
            DateTime dtFromDate = dtToDate.AddDays(-1 * nLookBackDays);
            string strFromDate = CDataUtils.GetMilitaryDateAsString(dtFromDate);

            //map id may have a carrot dilimiter so need to strip it
            string strMap = strMapID;
            string[] strMapSplit = strMap.Split('^');
            if(strMapSplit.Length > 1)
            {
                strMap = strMapSplit[0].ToString();
            }
            
            //get the lab results for this patient
            TaggedChemHemRptArrays ChemHemArrays =
                GetMDWSSOAPClient().getChemHemReports( strFromDate,
                                                       strToDate,
                                                       0);

            if (ChemHemArrays.fault == null)
            {
                for (int i = 0; i < ChemHemArrays.count; i++)
                {
                    TaggedChemHemRptArray ChemHemArray = ChemHemArrays.arrays[i];
                    if (ChemHemArray.rpts != null)
                    {
                        foreach (ChemHemRpt rpt in ChemHemArray.rpts)
                        {
                            foreach (LabResultTO result in rpt.results)
                            {
                                LabTestTO toLab = result.test;
                                if (toLab.fault == null)
                                {
                                    //if id of lab = map then process it
                                    if (toLab.id == strMap)
                                    {
                                        //create a new patient item
                                        CPatientItemDataItem di = new CPatientItemDataItem();
                                        di.PatientID = strPatientID;
                                        di.ItemID = lItemID;
                                        di.SourceTypeID = (long)k_SOURCE_TYPE_ID.VistA;
                                        
                                        //use the report time stamp
                                        di.EntryDate = CDataUtils.GetMDWSDate(rpt.timestamp);

                                        //build a pat item component to hold the note text
                                        CPatientItemComponentDataItem cdi = new CPatientItemComponentDataItem();
                                        cdi.PatientID = strPatientID;
                                        cdi.ItemID = lItemID;

                                        //need to get the component given the item
                                        DataSet dsItemComps = null;
                                        CItemComponentData cid = new CItemComponentData(this);
                                        cid.GetItemComponentDS(lItemID, k_ACTIVE_ID.All, out dsItemComps);
                                        if (dsItemComps != null)
                                        {
                                            //note titles only have 1 component.
                                            cdi.ComponentID = CDataUtils.GetDSLongValue(dsItemComps, "ITEM_COMPONENT_ID");
                                        }

                                        //get the result
                                        string strResult = result.value;
                                        //todo: remove this temp fix when we switch to clob
                                        if (strResult.Length > 4000)
                                        {
                                            strResult = strResult.Substring(0, 3995);
                                            strResult += "...";
                                        }
                                        
                                        //component value = the result
                                        cdi.ComponentValue = strResult;

                                        //component date is same as item date
                                        cdi.EntryDate = di.EntryDate;
                                        
                                        //create a new item component list with 1 component 
                                        //for the note text
                                        CPatientItemCompList comps = new CPatientItemCompList();
                                        comps.Add(cdi);

                                        //check to see if the note already exists
                                        //in the vappct database, if so we want to update 
                                        //it, else insert it
                                        bool bInsert = true;
                                        DataSet dsExists = null;
                                        CPatientItemData itemData = new CPatientItemData(this);
                                        status = itemData.GetPatItemCompDS(strPatientID,
                                                                            di.ItemID,
                                                                            di.EntryDate,
                                                                            out dsExists);
                                        if (!status.Status)
                                        {
                                            //todo:
                                            return status;
                                        }

                                        if (!CDataUtils.IsEmpty(dsExists))
                                        {
                                            bInsert = false;
                                        }

                                        if (!bInsert)
                                        {
                                            di.PatItemID = CDataUtils.GetDSLongValue(dsExists, "pat_item_id");
                                            cdi.PatItemID = di.PatItemID;

                                            //update the record 
                                            status = itemData.UpdatePatientItem(di,
                                                                                comps);
                                            if (!status.Status)
                                            {
                                                //todo:
                                            }

                                  //          CPatientChecklistLogic pcll = new CPatientChecklistLogic(this);
                                    //        pcll.RunLogic(di.PatientID, di.ItemID);
                                        }
                                        else
                                        {
                                            //add the record (insert only todo need to
                                            //handle dupes in the sp maybe have it update)
                                            long lPatItemID = 0;
                                            status = itemData.InsertPatientItem(di,
                                                                                comps,
                                                                                out lPatItemID);
                                            if (!status.Status)
                                            {
                                                //todo:
                                            }

                                  //          CPatientChecklistLogic pcll = new CPatientChecklistLogic(this);
                                    //        pcll.RunLogic(di.PatientID, di.ItemID);
                                        }

                                        //cleanup
                                        di = null;
                                        cdi = null;
                                        comps = null;
                                        itemData = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = e.Message;
            return status;
        }


        //now that we inserted all the items run the logic
        CPatientChecklistLogic pcll = new CPatientChecklistLogic(this);
        pcll.RunLogic(strPatientID, lItemID);

        return status;
    }      


    //todo: deletelater
    public void LoopPatientNotes()
    {
        DataSet ds = null;
        string strDFNs = string.Empty;
        CStatus status = GetMDWSPatientDS(out ds);
        if (ds != null)
        {
            CDataUtils.GetDSDelimitedData(
                ds,
                "patient_id",
                ",",
                out strDFNs);
        }

        string strTitles = string.Empty;
        DataSet dsND = null;
        CNoteTitleData nd = new CNoteTitleData(this);
        nd.GetNoteTitleDS(out dsND);
        if (dsND != null)
        {
            CDataUtils.GetDSDelimitedData(
                dsND,
                "note_title_label",
                ",",
                out strTitles);
            
            strTitles = strTitles.ToLower().Trim();

        }


        string strNoteTitles = string.Empty;
        string strTags = string.Empty;
        
        string[] splitDFNs = strDFNs.Split(',');
        foreach (string strDFN in splitDFNs)
        {
            //notes for the patient
            PatientTO toSelect = GetMDWSSOAPClient().select(strDFN);
            if (toSelect != null)
            {
                if (toSelect.fault == null)
                {
                    try
                    {

                        TaggedNoteArrays tnas = GetMDWSSOAPClient().getNotesWithText(
                            "19990101",
                            "20120227",
                            0);

                        bool bAddTrailer = false;

                        if (tnas != null)
                        {
                            if (tnas.fault == null)
                            {
                                foreach (TaggedNoteArray tna in tnas.arrays)
                                {
                                    if (tna.notes != null)
                                    {
                                        foreach (NoteTO note in tna.notes)
                                        {
                                            string strLoc = note.localTitle;
                                            strLoc = strLoc.Replace("<", "[");
                                            strLoc = strLoc.Replace(">", "]");
                                            strLoc = strLoc.ToLower().Trim();

                                            if (strTitles.IndexOf(strLoc) != -1)
                                            {
                                                strNoteTitles += "Patient: " + strDFN + " : " + toSelect.name + "\r\n";
                                                strNoteTitles += "Standard Title: " + note.standardTitle + "\r\n";
                                                strNoteTitles += "Local Title: " + note.localTitle + "\r\n\r\n";
                                                //strNoteTitles += note.text + "\r\n***************************\r\n";
                                                strNoteTitles += "";
                                                bAddTrailer = true;
                                            }
                                        }
                                    }
                                }

                                if (bAddTrailer)
                                {
                                    strNoteTitles += "\r\n***************************\r\n";
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
    
                    }
                }
            }
        }
    }

    //todo: deletelater
    public void LoopPatientRads()
    {
        DataSet ds = null;
        CStatus status = new CStatus();
        string strDFNs = String.Empty;
        status = GetMDWSPatientDS(out ds);
        if (ds != null)
        {
            CDataUtils.GetDSDelimitedData(
                ds,
                "patient_id",
                ",",
                out strDFNs);
        }

        string[] splitDFNs = strDFNs.Split(',');
        foreach (string strDFN in splitDFNs)
        {
            PatientTO toSelect = GetMDWSSOAPClient().select(strDFN);
            if (toSelect != null)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedRadiologyReportArrays radArrays =
                        GetMDWSSOAPClient().getRadiologyReports(
                            "19990101",
                            "20120303",
                            0);
                        if (radArrays.fault == null)
                        {
                            for (int i = 0; i < radArrays.count; i++)
                            {
                                TaggedRadiologyReportArray radArray = radArrays.arrays[i];
                                if (radArray.fault == null)
                                {
                                    if (radArray.rpts != null)
                                    {
                                        foreach (RadiologyReportTO rpt in radArray.rpts)
                                        {
                                            if (rpt.cptCode != null)
                                            {
                                                int j = 0;
                                                j++;

                                            }
                                            //  InsertZZTEst(toSelect.localPid,
                                            //                "getRadiologyReports",
                                            //              rpt.title,
                                            //            rpt.status + " " +rpt.timestamp,
                                            //          "");

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception) { };
                }
            }
        }
    }

    //todo: delete me later
    public void LoopPatientMeds()
    {
        DataSet ds = null;
        CStatus status = new CStatus();
        string strDFNs = String.Empty;
        status = GetMDWSPatientDS(out ds);
        if (ds != null)
        {
            CDataUtils.GetDSDelimitedData(
                ds,
                "patient_id",
                ",",
                out strDFNs);
        }

        string[] splitDFNs = strDFNs.Split(',');
        foreach (string strDFN in splitDFNs)
        {
            PatientTO toSelect = GetMDWSSOAPClient().select(strDFN);
            if (toSelect != null)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        //
                        TaggedMedicationArrays ar1 = GetMDWSSOAPClient().getImoMeds();
                        if(ar1.fault == null)
                        {
                            for(int i=0; i<ar1.count; i++)
                            {
                                TaggedMedicationArray ta1 = ar1.arrays[i];
                                if (ta1.fault == null)
                                {
                                    if(ta1.meds != null)
                                    {
                                        foreach (MedicationTO rpt in ta1.meds)
                                        {
                                            int j = 0;
                                            j++;
                                        }
                                    }
                                }
                            }
                        }

                        //
                        TaggedMedicationArrays ar2 = GetMDWSSOAPClient().getAllMeds();
                        if (ar2.fault == null)
                        {
                            for (int i = 0; i < ar2.count; i++)
                            {
                                TaggedMedicationArray ta2 = ar2.arrays[i];
                                if (ta2.fault == null)
                                {
                                    if (ta2.meds != null)
                                    {
                                        foreach (MedicationTO rpt in ta2.meds)
                                        {
                                            int j = 0;
                                            j++;
                                        }
                                    }
                                }
                            }
                        }

                        //
                        TaggedMedicationArrays ar3 = GetMDWSSOAPClient().getIvMeds();
                        if (ar3.fault == null)
                        {
                            for (int i = 0; i < ar3.count; i++)
                            {
                                TaggedMedicationArray ta3 = ar3.arrays[i];
                                if (ta3.fault == null)
                                {
                                    if (ta3.meds != null)
                                    {
                                        foreach (MedicationTO rpt in ta3.meds)
                                        {
                                            int j = 0;
                                            j++;
                                        }
                                    }
                                }
                            }
                        }

                        //
                        TaggedMedicationArrays ar4 = GetMDWSSOAPClient().getOtherMeds();
                        if (ar4.fault == null)
                        {
                            for (int i = 0; i < ar4.count; i++)
                            {
                                TaggedMedicationArray ta4 = ar4.arrays[i];
                                if (ta4.fault == null)
                                {
                                    if (ta4.meds != null)
                                    {
                                        foreach (MedicationTO rpt in ta4.meds)
                                        {
                                            int j = 0;
                                            j++;
                                        }
                                    }
                                }
                            }
                        }

                        //
                        //
                        TaggedMedicationArrays ar5 = GetMDWSSOAPClient().getOutpatientMeds();
                        if (ar5.fault == null)
                        {
                            for (int i = 0; i < ar5.count; i++)
                            {
                                TaggedMedicationArray ta5 = ar5.arrays[i];
                                if (ta5.fault == null)
                                {
                                    if (ta5.meds != null)
                                    {
                                        foreach (MedicationTO rpt in ta5.meds)
                                        {
                                            int j = 0;
                                            j++;
                                        }
                                    }
                                }
                            }
                        }



                    }
                    catch (Exception) { };
                }
            }
        }
    }

    //todo: delete me later
    public void LoopPatientICD()
    {
        DataSet ds = null;
        CStatus status = new CStatus();
        string strDFNs = String.Empty;
        status = GetMDWSPatientDS(out ds);
        if (ds != null)
        {
            CDataUtils.GetDSDelimitedData(
                ds,
                "patient_id",
                ",",
                out strDFNs);
        }

        string[] splitDFNs = strDFNs.Split(',');
        foreach (string strDFN in splitDFNs)
        {
            PatientTO toSelect = GetMDWSSOAPClient().select(strDFN);
            if (toSelect != null)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        //
                        TaggedIcdRptArrays ar1 = GetMDWSSOAPClient().getIcdProceduresReports("19990101",
                                                                                                 "20120303",
                                                                                                 0);

                         if (ar1.fault == null)
                        {
                            for (int i = 0; i < ar1.count; i++)
                            {
                                TaggedIcdRptArray ta1 = ar1.arrays[i];
                                if (ta1.fault == null)
                                {
                                    if (ta1.rpts != null)
                                    {
                                        foreach (IcdRpt rpt in ta1.rpts)
                                        {
                                            int j = 0;
                                            j++;
                                        }
                                    }
                                }
                            }
                        }


                         //
                         TaggedIcdRptArrays ar2 = GetMDWSSOAPClient().getIcdSurgeryReports("19990101",
                                                                                                  "20120303",
                                                                                                  0);

                         if (ar2.fault == null)
                         {
                             for (int i = 0; i < ar1.count; i++)
                             {
                                 TaggedIcdRptArray ta2 = ar2.arrays[i];
                                 if (ta2.fault == null)
                                 {
                                     if (ta2.rpts != null)
                                     {
                                         foreach (IcdRpt rpt in ta2.rpts)
                                         {
                                             int j = 0;
                                             j++;
                                         }
                                     }
                                 }
                             }
                         }
                        
                        

                    }
                    catch (Exception) { };
                }
            }
        }
    }

    //todo: delete me later
    public void LoopOrders()
    {
        TaggedTextArray ta = GetMDWSSOAPClient().getOrderableItemsByName("CHOL");
        if(ta != null)
        {
            if(ta.results != null)
            {
                //loop and process this block of note titles
                foreach (TaggedText ttr in ta.results)
                {
                    int i = 0;
                    i++;
                }
            }
        }

        return;

    }



    //todo:delete later
    public void LoopPatientLabs()
    {
        //DataSet ds = null;
        CStatus status = new CStatus();
        string strDFNs = String.Empty;
        //status = GetMDWSPatientDS(out ds);
        //if (ds != null)
        //{
        //    CDataUtils.GetDSDelimitedData(
        //        ds,
        //        "patient_id",
        //        ",",
        //        out strDFNs);
        // }

        strDFNs = "747,754,69,724,750,751,755,780,100000,224,775,776,777,100002,778,779,100001,433,149,3,235,362,100013,238,244,241,237,232,192,231,748,749,752,753,773,772,767,766,418,769,428,771,770,768,379,774,253,205,229,217,25,723,722,715,706,420,728,407,366,350,419,271,764,763,762,765,218,347,761,759,384,391,267,758,757,756,746,745,744,692,442,760,301,520,569,600,620,140,151,236,228,260,168,146,8,240,204,427,251,250,247,246,245,111,631,398,393,443,696,239,227,233,234,243,144,296,42,711,65,309,100005,100012,100006,100008,23,709,421,100011,100009,100007,100010,100004,100003,100604,100014,99,129,154,360,441,431,457,655,736,100839,100840";
        string strData = "";
        string[] splitDFNs = strDFNs.Split(',');
        foreach (string strDFN in splitDFNs)
        {
            PatientTO toSelect = GetMDWSSOAPClient().select(strDFN);
            if (toSelect != null)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedChemHemRptArrays ChemHemArrays =
                        GetMDWSSOAPClient().getChemHemReports("19990101",
                                                              "20120303",
                                                               0);
                        if (ChemHemArrays.fault == null)
                        {
                            for (int i = 0; i < ChemHemArrays.count; i++)
                            {
                                if (i == 0)
                                {
                                    strData += "Patient: " + toSelect.localPid + ": " + toSelect.name + "\r\n";
                                }

                                TaggedChemHemRptArray ChemHemArray = ChemHemArrays.arrays[i];
                                if (ChemHemArray.rpts != null)
                                {
                                    foreach (ChemHemRpt rpt in ChemHemArray.rpts)
                                    {
                                        foreach (LabResultTO result in rpt.results)
                                        {
                                            LabTestTO toLab = result.test;
                                            if (toLab.fault == null)
                                            {

                                            }

                                            strData += "id: " + toLab.id + " Name: " + toLab.name + " Value: " + result.value + "\r\n";


                                            //InsertZZTEst( toSelect.localPid,
                                            //            "getChemHemReports",
                                            //          toLab.name,
                                            //        result.value,
                                            //      toLab.id);
                                        }
                                    }
                                }
                            }
                            strData += "\r\n*********************************\r\n";
                        }
                    }
                    catch (Exception) { };
                }
            }
        }

        strData += "";
    }
    //todo: deletelater
    public void LoopPatientAppointments()
    {
        DataSet ds = null;
        CStatus status = new CStatus();
        string strDFNs = String.Empty;
        status = GetMDWSPatientDS(out ds);
        if (ds != null)
        {
            CDataUtils.GetDSDelimitedData(
                ds,
                "patient_id",
                ",",
                out strDFNs);
        }

        string strData = "";
    
        string[] splitDFNs = strDFNs.Split(',');
        int jjjjj = 0;
        foreach (string strDFN in splitDFNs)
        {
            if (jjjjj > 20)
            {
                break;
            }

            //notes for the patient
            PatientTO toSelect = GetMDWSSOAPClient().select(strDFN);
            if (toSelect != null)
            {
                if (toSelect.fault == null)
                {
                    try
                    {
                        TaggedAppointmentArrays tnas
                            = GetMDWSSOAPClient().getAppointments();
                        
                        if (tnas != null)
                        {
                            if (tnas.fault == null)
                            {
                                foreach (TaggedAppointmentArray tna in tnas.arrays)
                                {
                                    if (tna.appts != null)
                                    {
                                        foreach (AppointmentTO to in tna.appts)
                                        {
                                           
                                            string strData2 = "";

                                            strData2 += "Patient: " + toSelect.localPid + " - " + toSelect.name + "\r\n";

                                            strData2 += "Appointment ID: " + to.id + "\r\n";

                                            if (to.clinic != null)
                                            {
                                                strData2 += "Clinic ID: " + to.clinic.id;
                                                strData2 += "\r\n";

                                                strData2 += "Clinic Name: " + to.clinic.name;
                                                strData2 += "\r\n";

                                            }

                                            strData2 += "Status: " + to.status;
                                            strData2 += "\r\n";

                                            strData2 += "Pursose: " + to.purpose;
                                            strData2 += "\r\n";

                                            strData2 += "Type: " + to.type;
                                            strData2 += "\r\n";

                                            strData2 += "ekgDateTime: " + to.ekgDateTime;
                                            strData2 += "\r\n";

                                            strData2 += "labDateTime: " + to.labDateTime;
                                            strData2 += "\r\n";

                                            strData2 += "xrayDateTime: " + to.xrayDateTime;
                                            strData2 += "\r\n";

                                            strData2 += "Text: " + to.text;
                                            strData2 += "\r\n";

                                            TextTO toNote = GetMDWSSOAPClient().getAppointmentText("901", to.id);
                                            
                                            if (toNote != null)
                                            {
                                                strData2 += "Note: " + toNote.text;
                                                strData2 += "\r\n";
                                            }

                                            strData2 += "***********************************\r\n";

                                            if (toNote.text.ToLower().IndexOf("no progress notes for") == -1)
                                            {
                                                jjjjj++;
                                                strData += strData2;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }


    /// <summary>
    /// transfer lab tests from MDWS to the VAPPCT database
    /// </summary>
    /// <param name="la"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferLabTestArray(TaggedLabTestArrays la, 
                                        out long lCount,
                                        out string strNextSearch)
    {
        //create a status object and check for valid dbconnection
        lCount = 0;
        strNextSearch = String.Empty;

        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check for valid teams
        if (la == null)
        {
            //todo:
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "lab array is null";

            return status;
        }

        //make sure patient is valid (no fault)
        if (la.fault != null)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = la.fault.message;
            return status;
        }

        //loop over the teams and save each one to the database
        string strNames = String.Empty;
        foreach (TaggedLabTestArray ta in la.arrays)
        {
            if (ta != null)
            {
                if (ta.labTests != null)
                {
                    if (ta.labTests.tests != null)
                    {
                        foreach (LabTestTO toLab in ta.labTests.tests)
                        {
                            if (toLab != null)
                            {
                                lCount++;

                                string strLOINC = String.Empty;
                                if (toLab.loinc != null)
                                {
                                    strLOINC = toLab.loinc;
                                }

                                string strLabTestID = String.Empty; 
                                if (toLab.id != null)
                                {
                                    strLabTestID = toLab.id;
                                }

                                string strName = String.Empty;
                                if (toLab.name != null)
                                {
                                    strName = toLab.name;
                                }

                                strNextSearch = strName;

                                string strHIREF = String.Empty;
                                if (toLab.hiRef != null)
                                {
                                    strHIREF = toLab.hiRef;
                                }

                                string strLOREF = String.Empty;
                                if (toLab.lowRef != null)
                                {
                                    strLOREF = toLab.lowRef;
                                }

                                string strREFRANGE = String.Empty;
                                if (toLab.refRange != null)
                                {
                                    strREFRANGE = toLab.refRange;
                                }

                                string strUnits = String.Empty;
                                if (toLab.units != null)
                                {
                                    strUnits = toLab.units;
                                }
                                
                                string strDescription = String.Empty;

                                //get the lab test description using a seperate call
                                TaggedTextArray tta = GetMDWSSOAPClient().getLabTestDescription(toLab.id);
                                if (tta != null)
                                {
                                    if (tta.fault == null)
                                    {
                                        if (tta.results[0] != null)
                                        {
                                            strDescription = tta.results[0].text;
                                        }
                                    }
                                }
                        
                                //save the lab test to th vappct database
                                CLabData ld = new CLabData(this);
                                status = ld.SaveLabTest( 1, //todo: magic number 
                                                         strLabTestID,
                                                         strName,
                                                         strHIREF,
                                                         strLOREF,
                                                         strREFRANGE,
                                                         strUnits,
                                                         strDescription,
                                                         strLOINC);
                                if (!status.Status)
                                {
                                    //todo: dont end because 1 does not save...
                                }
                            }
                        }
                    }
                }
            }
        }
        return status;
    }
    
    /// <summary>
    /// US:838
    /// Transfer MDWS teams to our db
    /// </summary>
    /// <param name="txtTeams"></param>
    /// <returns></returns>
    public CStatus TransferTeams(TaggedText txtTeams, out long lCount)
    {
        //todo: delete me later
        //LoopPatientNotes();

        //create a status object and check for valid dbconnection
        lCount = 0;
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check for valid teams
        if (txtTeams == null)
        {
            //todo:
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "teams is null";

            return status;
        }

        //make sure patient is valid (no fault)
        if (txtTeams.fault != null)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = txtTeams.fault.message;
            return status;
        }

        //loop over the teams and save each one to the database
        foreach (TaggedText txtTeam in txtTeams.taggedResults)
        {
            if (txtTeam != null)
            {
                lCount++;

                long lTeamID = CDataUtils.ToLong(txtTeam.tag);
                CTeamData td = new CTeamData(this);
                status = td.SaveTeam(1, lTeamID, txtTeam.text);

                if (!status.Status)
                {
                    //todo:
                }
            }
        }

        return status;
    }
    
    /// <summary>
    /// US:1945 US:838
    /// Transfer MDWS note tiles to our db
    /// </summary>
    /// <param name="txtTeams"></param>
    /// <returns></returns>
    public CStatus TransferNoteTitles(TaggedTextArray tta, out long lCount)
    {
        //create a status object and check for valid dbconnection
        lCount = 0;
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }
        //loop over alll taggedreulsts in the array
        foreach (TaggedText tt in tta.results)
        {
            //loop and process this block of note titles
            foreach (TaggedText ttr in tt.taggedResults)
            {
                //get the note title
                long lNoteTitleTag = CDataUtils.ToLong(ttr.tag);

                //get the note details (todo: need to research)
                string strNoteDetails = String.Empty;

                foreach (string strNoteLabel in ttr.textArray)
                {
                    //< > cause page validation problems.
                    string strNoteLbl = strNoteLabel;
                    strNoteLbl = strNoteLbl.Replace("<", "[");
                    strNoteLbl = strNoteLbl.Replace(">", "]");

                    //save the note title in our db
                    CNoteTitleData ntd = new CNoteTitleData(this);
                    status = ntd.SaveNoteTitle(1,
                                               lNoteTitleTag,
                                               strNoteLbl,
                                               strNoteDetails);
                }
            }
        }

        //LoopPatientNotes();
        //LoopPatientAppointments();
        
        return status;
    }

    /// <summary>
    /// US:838
    /// transfer MDWS specialties
    /// </summary>
    /// <param name="txtSpecialties"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferSpecialties(TaggedText txtSpecialties,
                                       out long lCount)
    {
        //create a status object and check for valid dbconnection
        lCount = 0;
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check for valid teams
        if (txtSpecialties == null)
        {
            //todo:
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "specialties is null";

            return status;
        }

        //make sure patient is valid (no fault)
        if (txtSpecialties.fault != null)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = txtSpecialties.fault.message;
            return status;
        }

        //loop over the teams and save each one to the database
        foreach (TaggedText txtSpecialty in txtSpecialties.taggedResults)
        {
            if (txtSpecialty != null)
            {
                lCount++;

                long lSpecialtyID = CDataUtils.ToLong(txtSpecialty.tag);
                CSpecialtyData sd = new CSpecialtyData(this);
                status = sd.SaveSpecialty(1, lSpecialtyID, txtSpecialty.text);

                if (!status.Status)
                {
                    //todo:
                }
            }
        }

        return status;
    }


    /// <summary>
    /// US:838
    /// transfer MDWS wards to the checklist db
    /// </summary>
    /// <param name="txtSpecialties"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferWards(TaggedHospitalLocationArray HospitalLocArray,
                                 out long lCount)
    {
        //create a status object and check for valid dbconnection
        lCount = 0;
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check for valid data
        if (HospitalLocArray == null)
        {
            //todo:
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "wards are null";

            return status;
        }

        //make sure patient is valid (no fault)
        if (HospitalLocArray.fault != null)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = HospitalLocArray.fault.message;
            return status;
        }

        //loop over the teams and save each one to the database
        foreach (HospitalLocationTO hlto in HospitalLocArray.locations)
        {
            if (hlto != null)
            {
                lCount++;

                long lWardID = CDataUtils.ToLong(hlto.id);
                CWardData wd = new CWardData(this);
                status = wd.SaveWard(1, lWardID, hlto.name);

                if (!status.Status)
                {
                    //todo:
                }
            }
        }

        return status;
    }

    /// <summary>
    /// transfer user security keys
    /// </summary>
    /// <param name="usk"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferSecurityKeys( long lUserID,
                                         UserSecurityKeyArray usk,
                                         out long lCount)
    {
        //create a status object and check for valid dbconnection
        lCount = 0;
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check for valid data
        if (usk == null)
        {
            //todo:
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "security keys are null";

            return status;
        }

        //make sure usk is valid (no fault)
        if (usk.fault != null)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = usk.fault.message;
            return status;
        }

        //loop over the teams and save each one to the database
        foreach (UserSecurityKeyTO keyTO in usk.keys)
        {
            if (keyTO != null)
            {
                lCount++;

                long lKeyID = CDataUtils.ToLong(keyTO.id);
                CSecurityKeyData keyData = new CSecurityKeyData(this);
                status = keyData.SaveSecurityKey(1, 
                                                 lUserID, 
                                                 lKeyID,
                                                 keyTO.name);

                if (!status.Status)
                {
                    //todo:
                }
            }
        }

        return status;
    }



    /// <summary>
    /// US:838
    /// transfer MDWS clinics to the checklist db
    /// </summary>
    /// <param name="txtSpecialties"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferClinics(TaggedHospitalLocationArray HospitalLocArray,
                                   out long lCount)
    {
        //create a status object and check for valid dbconnection
        lCount = 0;
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check for valid data
        if (HospitalLocArray == null)
        {
            //todo:
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "wards are null";

            return status;
        }

        //make sure patient is valid (no fault)
        if (HospitalLocArray.fault != null)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = HospitalLocArray.fault.message;
            return status;
        }

        //loop over the teams and save each one to the database
        foreach (HospitalLocationTO hlto in HospitalLocArray.locations)
        {
            if (hlto != null)
            {
                lCount++;

                long lClinicID = CDataUtils.ToLong(hlto.id);
                CClinicData cd = new CClinicData(this);
                status = cd.SaveClinic(1, lClinicID, hlto.name);

                if (!status.Status)
                {
                    //todo:
                }
            }
        }

        return status;
    }

    /// <summary>
    /// US:838
    /// Transfers 1 patient and adds the patient to a team, ward etc... as needed
    /// </summary>
    /// <param name="tpa"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferPatient(string strDUZ,
                                   long lTeamID,
                                   long lSpecialtyID,
                                   long lWardID,
                                   long lClinicID,
                                   PatientTO toPatient)
    {
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check for valid patient
        if (toPatient == null)
        {
            //todo:
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "Patient is null";

            return status;
        }

        //make sure patient is valid (no fault)
        if (toPatient.fault != null)
        {
            //todo: need better error message
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "Patient has a fault";

            return status;
        }

        //TODO: may not want to do this! is some cases the patients are coming back with missing data
        //needed for the transfer. If you select the patient the data fills in
        //and transfer will work
        if (String.IsNullOrEmpty(toPatient.ssn) ||
            toPatient.ssn.IndexOf("*") != -1)
        {
            PatientTO toPatSel = GetMDWSSOAPClient().select(toPatient.localPid);
            toPatient = toPatSel;
        }

        //our patient item
        CPatientDataItem pdi = new CPatientDataItem();
        //string strDOB = pat.dob;

        //name
        pdi.FullName = toPatient.patientName;
        string[] splitFullName = toPatient.patientName.Split(',');
        if (splitFullName[0] != null)
        {
            pdi.LastName = splitFullName[0];
        }

        if (splitFullName.Length > 1)
        {
            if (splitFullName[1] != null)
            {
                pdi.FirstName = splitFullName[1];
            }
        }

        pdi.MI = String.Empty;

        //gender
        //default gender to unknown
        pdi.Sex = k_SEX.UNKNOWN;
        if (toPatient.gender == null)
        {
            pdi.Sex = k_SEX.UNKNOWN;
        }
        else
        {
            if (toPatient.gender.ToLower().IndexOf("unk") != -1)
            {
                pdi.Sex = k_SEX.UNKNOWN;
            }
        }

        pdi.SSN = toPatient.ssn;
        pdi.PatientID = toPatient.localPid;

        //save the patient to our db
        CPatientData dta = new CPatientData(this);
        //todo: make a constant for "1"
        status = dta.SavePatient(CDataUtils.ToLong(strDUZ), 
                                 lTeamID, 
                                 lSpecialtyID, 
                                 lWardID, 
                                 lClinicID,
                                 1, 
                                 pdi);
        
        return status;
    }
    

    /// <summary>
    /// US:838
    /// Transfers a patient array to our db
    /// </summary>
    /// <param name="tpa"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferPatientArray(string strDUZ,
                                        long lTeamID,
                                        long lSpecialtyID,
                                        long lWardID,
                                        long lClinicID,
                                        TaggedPatientArray tpa,
                                        out long lCount)
    {
        //initialize parameters
        lCount = 0;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check to see if there is anything to transfer
        if (tpa.count < 1)
        {
            return status;
        }
        
        foreach (PatientTO pat in tpa.patients)
        {
            //additional demographics
            DemographicSetTO[] demog = pat.demographics;
            
            CStatus statXfer = new CStatus();
            statXfer = TransferPatient(strDUZ, 
                                       lTeamID, 
                                       lSpecialtyID, 
                                       lWardID,
                                       lClinicID,
                                       pat);
            lCount++;
            if (!statXfer.Status)
            {
                //todo: better error message
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = "One or more of your patient(s) could not be transfered!";
                //note: dont return yet, try the rest of the patients
            }
        }
        
        return status;
    }

    /// <summary>
    /// transfer a MDWS user array to our db
    /// </summary>
    /// <param name="ua"></param>
    /// <param name="lCount"></param>
    /// <returns></returns>
    public CStatus TransferUserArray( UserArray ua,
                                      out long lCount)
    {
        //initialize parameters
        lCount = 0;
        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //check to see if there is anything to transfer
        if (ua.count < 1)
        {
            return status;
        }

        foreach (UserTO toUser in ua.users)
        {
           
            long lUserID = 0;

            CStatus statXfer = new CStatus();
            statXfer = TransferUser(toUser,
                                    out lUserID);
            lCount++;
            if (!statXfer.Status)
            {
                //todo: better error message
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = "One or more of your users(s) could not be transfered!";
                //note: dont return yet, try the rest of the patients
            }
        }

        return status;
    }

    /// <summary>
    /// US:840
    /// transfer MDWS user data to the checklist database
    /// </summary>
    /// <param name="toUser"></param>
    /// <param name="lUserID"></param>
    /// <returns></returns>
    public CStatus TransferUser(UserTO toUser, out long lUserID)
    {
        //initialize parameters
        lUserID = 0;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);

        //todo: thinking we can use user class from the
        //sp to translate into our user_role id
        string strUserClass = String.Empty;
        if (!String.IsNullOrEmpty(toUser.userClass))
        {
            strUserClass = toUser.userClass;
        }

        string strGreeting = "";
        if (!String.IsNullOrEmpty(toUser.greeting))
        {
            strGreeting = toUser.greeting;
        }

        string strUserName = "";
        if (!String.IsNullOrEmpty(toUser.name))
        {
            strUserName = toUser.name;
        }

        //add the rest of the parameters
        //todo: move thsi sp call to CUSERData class
        long lUID = CDataUtils.ToLong(toUser.DUZ);
        long lSiteID = CDataUtils.ToLong(toUser.siteId);
        pList.AddInputParameter("pi_nXferSystemID", 1);//todo: need constant
        pList.AddInputParameter("pi_nSysUserID", lUID);
        pList.AddInputParameter("pi_vName", strUserName);
        pList.AddInputParameter("pi_vGreeting", strGreeting);
        pList.AddInputParameter("pi_nSiteID", lSiteID);
        pList.AddInputParameter("pi_vUserClass", strUserClass);
        pList.AddOutputParameter("po_nUserID", lUserID);

        //execute the SP
        status = DBConn.ExecuteOracleSP("PCK_USR.SaveUser", pList);
        if (status.Status)
        {
            //get the returned ID from the SP call
            lUserID = pList.GetParamLongValue("po_nUserID");
        }

        return status;
    }
}
