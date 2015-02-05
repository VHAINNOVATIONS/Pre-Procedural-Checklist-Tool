using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Configuration;
using VAPPCT.DA;
using VAPPCT.Data.MDWSEmrSvc;

    public class CCommunicator
    {
        /// <summary>
        /// US:834 writes an event to the communicator event table
        /// </summary>
        /// <param name="strEventName"></param>
        /// <param name="strEventDetails"></param>
        public void WriteEvent(CData data,
                               string strEventName,
                               string strEventDetails)
        {
            //comm data class
            CVAPPCTCommData commData = new CVAPPCTCommData(data);

            CStatus status = new CStatus();

            //com data object for logging etc...
            status = commData.SaveCommEvent(strEventName, strEventDetails);
        }

        /// <summary>
        /// US:834 gets a connection to the vappct oracle database and MDWS
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mdwsSOAPClient"></param>
        /// <returns></returns>
        public CStatus GetConnections(out CCommDBConn conn,
                                      out CData data,
                                      out EmrSvcSoapClient mdwsSOAPClient)
        {
            //connect to the oracle db and MDWS
            data = null;
            mdwsSOAPClient = null;
            conn = new CCommDBConn();
            CStatus status = conn.Connect(out data, out mdwsSOAPClient);
            if (!status.Status)
            {
                //write the start event to the event table
                WriteEvent(data, "Connect", status.StatusComment);
                return status;
            }

            return status;
        }

        /// <summary>
        /// US:1945 US:852 US:1883 US:834 helper to process checklist items, called from multiple places
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        /// <param name="dsChecklistItems"></param>
        /// <returns></returns>
        public CStatus RefreshPatientCheckList(CDataConnection conn,
                                                CData data,
                                                DataSet dsChecklistItems)
        {
            CStatus status = new CStatus();

            //this class is used to do all transfers 
            //from MDWS to the VAPPCT database
            CMDWSTransfer xfer = new CMDWSTransfer(data);

            //2.loop over all items and process each one
            foreach (DataTable table in dsChecklistItems.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    if (dr["item_type_id"] != null)
                    {
                        switch (Convert.ToInt32(dr["item_type_id"]))
                        {
                            case (int)k_ITEM_TYPE_ID.Collection:
                                //WriteEvent(data, "Collection", "Collection");
                                break;
                            case (int)k_ITEM_TYPE_ID.Laboratory:
                                //WriteEvent(data, "Laboratory", "Laboratory");
                                status = ProcessLab(data, xfer, dr);
                                if (!status.Status)
                                {
                                    //write the start event to the event table
                                    WriteEvent(data, "ProcessLab", status.StatusComment);
                                    return status;
                                }
                                break;
                            case (int)k_ITEM_TYPE_ID.QuestionFreeText:
                                //WriteEvent(data, "QuestionFreeText", "QuestionFreeText");
                                break;
                            case (int)k_ITEM_TYPE_ID.QuestionSelection:
                                //WriteEvent(data, "QuestionSelection", "QuestionSelection");
                                break;
                            case (int)k_ITEM_TYPE_ID.NoteTitle:
                                status = ProcessNoteTitle(data, xfer, dr);
                                if (!status.Status)
                                {
                                    //write the start event to the event table
                                    WriteEvent(data, "ProcessNoteTitle", status.StatusComment);
                                    return status;
                                }
                                break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// US:1945 US:852 US:1883 US:834 helper to process checklist items, called from multiple places
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        /// <param name="dsChecklistItems"></param>
        /// <returns></returns>
        public CStatus CommRefreshPatientCheckList( DataSet dsChecklistItems)
        {
            CStatus status = new CStatus();

            //get a connection to the vappct database and MDWS
            CData data = null;
            EmrSvcSoapClient mdwsSOAPClient = null;
            CCommDBConn conn = null;
            status = GetConnections(
                out conn,
                out data,
                out mdwsSOAPClient);
            if (!status.Status)
            {
                conn.Close();
                mdwsSOAPClient.disconnect();

                WriteEvent(data, "GetConnections", status.StatusComment);
                return status;
            }
            
            //this class is used to do all transfers 
            //from MDWS to the VAPPCT database
            CMDWSTransfer xfer = new CMDWSTransfer(data);

            //if we do too many items at once the connection to MDWS will
            //timeout. This is only an issue when running from communicator
            //because there can be a large number of items. 
            //
            //current item we are processing
            int nCount = 0;
            //number of items to process before we re-connect
            int nBatch = 50; 

            //2.loop over all items and process each one
            foreach (DataTable table in dsChecklistItems.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    nCount++;

                    //if ncount is >= batch then reconnect and reset
                    if (nCount >= nBatch)
                    {
                        //reset the data
                        data = null;

                        //close the current mdws connection
                        mdwsSOAPClient.disconnect();
                        mdwsSOAPClient = null;

                        //close the connection to the database
                        conn.Close();
                        //conn = null;

                        //get a connection to the vappct database and MDWS
                        status = GetConnections(
                            out conn,
                            out data,
                            out mdwsSOAPClient);
                        if (!status.Status)
                        {
                            conn.Close();
                            mdwsSOAPClient.disconnect();

                            WriteEvent(data, "GetConnections", status.StatusComment);
                            return status;
                        }

                        //reset the transfer object to use the new connection
                        xfer = null;
                        xfer = new CMDWSTransfer(data);

                        //reset the record count
                        nCount = 1;
                    }

                    //process the item
                    if (dr["item_type_id"] != null)
                    {
                        switch (Convert.ToInt32(dr["item_type_id"]))
                        {
                            case (int)k_ITEM_TYPE_ID.Collection:
                                //WriteEvent(data, "Collection", "Collection");
                                break;
                            case (int)k_ITEM_TYPE_ID.Laboratory:
                                //WriteEvent(data, "Laboratory", "Laboratory");
                                status = ProcessLab(data, xfer, dr);
                                if (!status.Status)
                                {
                                    //write the start event to the event table
                                    WriteEvent(data, "ProcessLab", status.StatusComment);
                                    //dont return or other records will not update return status;
                                }
                                break;
                            case (int)k_ITEM_TYPE_ID.QuestionFreeText:
                                //WriteEvent(data, "QuestionFreeText", "QuestionFreeText");
                                break;
                            case (int)k_ITEM_TYPE_ID.QuestionSelection:
                                //WriteEvent(data, "QuestionSelection", "QuestionSelection");
                                break;
                            case (int)k_ITEM_TYPE_ID.NoteTitle:
                                status = ProcessNoteTitle(data, xfer, dr);
                                if (!status.Status)
                                {
                                    //write the start event to the event table
                                    WriteEvent(data, "ProcessNoteTitle", status.StatusComment);
                                    //dont return or other records will not process return status;
                                }
                                break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// US:1883 US:834 refresh a patient checklist
        /// </summary>
        /// <param name="strPatientID"></param>
        /// <param name="strPatChecklistID"></param>
        /// <returns></returns>
        public CStatus RefreshPatientCheckList(CDataConnection conn,
                                               CData data,
                                               string strPatientID,
                                               long lPatChecklistID)
        {
            //todo: check conn and data before continuing
              
            //get all the items for this patient checklist
            DataSet dsChecklistItems = null;
            CVAPPCTCommData commData = new CVAPPCTCommData(data);
            CStatus status = commData.GetPatChecklistItemDS(lPatChecklistID, out dsChecklistItems);
            if (!status.Status)
            {
                //write the event to the event table
                WriteEvent(data, "GetPatChecklistItemDS", status.StatusComment);
                return status;
            }

            //refresh the checklist items
            status = RefreshPatientCheckList(
                conn,
                data,
                dsChecklistItems);
            if (!status.Status)
            {
                //write the event to the event table
                WriteEvent(data, "RefreshPatientCheckList", status.StatusComment);
                return status;
            }

            //refresh the checklist collection items
            DataSet dsCLCollectionItems = null;
            status = commData.GetPatientCLCollectionItemDS(lPatChecklistID, out dsCLCollectionItems);
            if (!status.Status)
            {
                //write the event to the event table
                WriteEvent(data, "GetPatientCLCollectionItemDS", status.StatusComment);
                return status;
            }

            //refresh the checklist items
            status = RefreshPatientCheckList(
                conn,
                data,
                dsCLCollectionItems);
            if (!status.Status)
            {
                //write the event to the event table
                WriteEvent(data, "RefreshPatientCheckList", status.StatusComment);
                return status;
            }

            return status;
        }

        /// <summary>
        /// US:1883 US:834 refresh multipatient view 
        /// </summary>
        /// <param name="plistPatChecklistIDs"></param>
        /// <returns></returns>
        public CStatus RefreshMultiPatientChecklists(CDataConnection conn,
                                                     CData data,
                                                     DateTime dtFrom,
                                                     DateTime dtTo,
                                                     long lChecklistID,
                                                     long lServiceID,
                                                     long lChecklistStatus)
        {
            //this class is used to do all transfers 
            //from MDWS to the VAPPCT database
            CMDWSTransfer xfer = new CMDWSTransfer(data);

            //get the multi patient checklist ds, uses the same filters 
            // as the website form does
            DataSet dsMulti = null;
            CPatientData pd = new CPatientData(data);
            CStatus status = pd.GetMultiPatientSearchDS(
                dtFrom,
                dtTo,
                lChecklistID,
                lChecklistStatus,
                lServiceID,
                out dsMulti);
            if (!status.Status)
            {
                //write the event to the event table
                WriteEvent(data, "GetMultiPatientSearchDS", status.StatusComment);
                return status;
            }

            foreach (DataTable table in dsMulti.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    //checklist ID
                    long lPatChecklistID = Convert.ToInt64(dr["pat_cl_id"].ToString());

                    //get the items for this pat checklist id
                    DataSet dsChecklistItems = null;
                    CVAPPCTCommData commData = new CVAPPCTCommData(data);
                    status = commData.GetPatChecklistItemDS(lPatChecklistID,
                                                            out dsChecklistItems);
                    if (!status.Status)
                    {
                        //write the event to the event table
                        WriteEvent(data, "GetPatChecklistItemDS", status.StatusComment);
                        return status;
                    }

                    //refresh the checklist items
                    status = RefreshPatientCheckList(
                        conn,
                        data,
                        dsChecklistItems);
                    if (!status.Status)
                    {
                        //write the event to the event table
                        WriteEvent(data, "RefreshPatientCheckList", status.StatusComment);
                        return status;
                    }

                    //refresh the checklist collection items
                    DataSet dsCLCollectionItems = null;
                    status = commData.GetPatientCLCollectionItemDS(lPatChecklistID,
                                                                   out dsCLCollectionItems);
                    if (!status.Status)
                    {
                        //write the event to the event table
                        WriteEvent(data, "GetPatientCLCollectionItemDS", status.StatusComment);
                        return status;
                    }

                    //refresh the checklist items
                    status = RefreshPatientCheckList(
                        conn,
                        data,
                        dsCLCollectionItems);
                    if (!status.Status)
                    {
                        //write the event to the event table
                        WriteEvent(data, "RefreshPatientCheckList", status.StatusComment);
                        return status;
                    }
                }
            }

            return status;
        }
        
        /// <summary>
        /// US:1883 US:834 loops through the checklist and moves data from MDWS to
        /// the VAPPCT database
        /// </summary>
        /// <returns></returns>
        public CStatus ProcessOpenChecklistItems()
        {
            //get a connection to the vappct database and MDWS
            CData data = null;
            EmrSvcSoapClient mdwsSOAPClient = null;
            CCommDBConn conn = null;
            CStatus status = GetConnections(
                out conn,
                out data,
                out mdwsSOAPClient);
            if (!status.Status)
            {
                conn.Close();
                mdwsSOAPClient.disconnect();

                WriteEvent(data, "GetConnections", status.StatusComment);
                return status;
            }

            //-----------------------------------------------------
            //do the work TODO: threading will come later
            //-----------------------------------------------------

            //1. get all of the open checklists
            DataSet dsChecklistItems = null;
            CVAPPCTCommData commData = new CVAPPCTCommData(data);
            status = commData.GetOpenPatChecklistItemDS(out dsChecklistItems);
            if (!status.Status)
            {
                conn.Close();
                mdwsSOAPClient.disconnect();

                //write the event to the event table
                WriteEvent(data, "GetOpenPatChecklistItemDS", status.StatusComment);
                return status;
            }

            //refresh the checklist items
            //status = RefreshPatientCheckList(
            //    conn,
            //    data,
            //    dsChecklistItems);
            status = CommRefreshPatientCheckList(dsChecklistItems);
            if (!status.Status)
            {
                conn.Close();
                mdwsSOAPClient.disconnect();

                //write the event to the event table
                WriteEvent(data, "RefreshPatientCheckList", status.StatusComment);
                return status;
            }

            //it could have taken a really long time for the first set
            //to process, so we reset the connections here before moving
            //to the collection items.

            //reset the data
            data = null;

            //close the current mdws connection
            mdwsSOAPClient.disconnect();
            mdwsSOAPClient = null;

            //close the connection to the database
            conn.Close();
            //conn = null;

            //get a connection to the vappct database and MDWS
            status = GetConnections(
                out conn,
                out data,
                out mdwsSOAPClient);
            if (!status.Status)
            {
                conn.Close();
                mdwsSOAPClient.disconnect();

                WriteEvent(data, "GetConnections", status.StatusComment);
                return status;
            }

            CVAPPCTCommData commData2 = new CVAPPCTCommData(data);
            
            //refresh the checklist collection items
            DataSet dsCLCollectionItems = null;
            status = commData2.GetOpenPatCLCollectionItemDS(out dsCLCollectionItems);
            if (!status.Status)
            {
                conn.Close();
                mdwsSOAPClient.disconnect();

                //write the event to the event table
                WriteEvent(data, "GetOpenPatCLCollectionItemDS", status.StatusComment);
                return status;
            }
             
            //refresh the checklist items
            //status = RefreshPatientCheckList(
            //    conn,
            //    data,
            //    dsCLCollectionItems);
            status = CommRefreshPatientCheckList(dsCLCollectionItems);
            if (!status.Status)
            {
                conn.Close();
                mdwsSOAPClient.disconnect();

                //write the event to the event table
                WriteEvent(data, "RefreshPatientCheckList", status.StatusComment);
                return status;
            }

            //-----------------------------------------------------
            //end of work
            //-----------------------------------------------------
            //cleanup: close the database connection, disconnect from MDWS
            conn.Close();
            mdwsSOAPClient.disconnect();
     
            return status;
        }

        /// <summary>
        /// US:1945 US:834 helper to transfer matching note titles
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xfer"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        public CStatus ProcessNoteTitle(CData data,
                                        CMDWSTransfer xfer,
                                        DataRow dr)
        {
            //check the data row
            if (dr == null)
            {
                WriteEvent(data, "ERROR_XFER_NOTETITLE", "DataRow is null");
                return new CStatus(false, k_STATUS_CODE.Failed, "ERROR_XFER_NOTETITLE: DataRow is null");
            }

            //make sure we have data to process
            if (dr["patient_id"] == null ||
                dr["map_id"] == null ||
                dr["item_id"] == null ||
                dr["lookback_time"] == null)
            {
                return new CStatus();
            }


            //transfer Patient notes
            CStatus status = xfer.TransferPatientNotes(
                dr["patient_id"].ToString(),
                dr["map_id"].ToString(),
                Convert.ToInt64(dr["item_id"]),
                Convert.ToInt32(dr["lookback_time"]));
            if (!status.Status)
            {
                WriteEvent(
                    data,
                    "ERROR_XFER_NOTETITLE",
                    "patient:" + dr["patient_id"].ToString() + " note:" + dr["map_id"].ToString());
                return status;
            }

            return status;
        }


        /// <summary>
        /// US:852 process a lab item
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xfer"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        public CStatus ProcessLab(CData data,
                                  CMDWSTransfer xfer,
                                  DataRow dr)
        {
            //check the data row
            if (dr == null)
            {
                WriteEvent(data, "ERROR_XFER_LAB", "DataRow is null");
                return new CStatus(false, k_STATUS_CODE.Failed, "ERROR_XFER_LAB: DataRow is null");
            }

            //check that we have data
            if (dr["patient_id"] == null ||
                dr["map_id"] == null ||
                dr["item_id"] == null ||
                dr["lookback_time"] == null)
            {
                return new CStatus();
            }

            //transfer Patient labs
            CStatus status = xfer.TransferPatientLabs(
                dr["patient_id"].ToString(),
                dr["map_id"].ToString(),
                Convert.ToInt64(dr["item_id"]),
                Convert.ToInt32(dr["lookback_time"]));

            if (!status.Status)
            {
                WriteEvent(
                    data,
                    "ERROR_XFER_LAB",
                    "patient:" + dr["patient_id"].ToString() + " note:" + dr["map_id"].ToString());
                return status;
            }

            return status;
        }
    }
