using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;

//access app.config
using System.Configuration;

//MDWS web service
using VAPPCT.Data.MDWSEmrSvc;

//our data access library
using VAPPCT.DA;

    public class CCommDBConn : CDataConnection
    {
        //constructor
        public CCommDBConn()
        {


        }

        /// <summary>
        /// US:834 Connect to the database using info from the 
        /// app.config file will also load a CData object for use
        /// by other data classes and setup the connectsion to MDWS and
        /// return a mdwsSOAPClient for accessing MDWD methods
        /// </summary>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public CStatus Connect(out CData data,
                               out EmrSvcSoapClient mdwsSOAPClient)
        {
            data = null;
            mdwsSOAPClient = null;

            //initialize parameters
            string strConnString = String.Empty;
            bool bAudit = false;

            //get the connection info from the web.config
            CStatus status = new CStatus();
            status = GetConnectionInfo(out strConnString, out bAudit);
            if (!status.Status)
            {
                return status;
            }

            //Connect to the db, if successful caller can use the 
            //CDataConnection::Conn property for access to the DB connection
            status = Connect(strConnString, bAudit);
            if (!status.Status)
            {
                //todo handle error 
                return status;
            }

            //create a new base data object
            //todo: more later
            //
            //get the ipaddress
            string strIPAddress = String.Empty;
            string strHost = System.Net.Dns.GetHostName();
            IPHostEntry host;
            host = Dns.GetHostEntry(strHost);
            foreach (IPAddress ip in host.AddressList)
            {
                strIPAddress = ip.ToString();
            }

            //build the base data item used by data classes
            string strNow = CDataUtils.GetDateTimeAsString(DateTime.Now);
            data = new CData(this,
                             strIPAddress,
                             0,
                             "VAPPCTCOMM_" + strNow,
                             null,
                             true);

            //comm data class
            CVAPPCTCommData commData = new CVAPPCTCommData(data);

            //login to MDWS
            long lUserID = 0;
            CMDWSOps ops = new CMDWSOps(data);
            //uid and pwd need come from config file: 
            //TODO: they need to be encrypted
            status = ops.MDWSLogin(ConfigurationSettings.AppSettings["MDWSEmrSvcUID"],
                                   ConfigurationSettings.AppSettings["MDWSEmrSvcPWD"],
                                   CDataUtils.ToLong(ConfigurationSettings.AppSettings["MDWSEmrSvcSiteList"]),
                                   out lUserID,
                                   out mdwsSOAPClient);
            if (!status.Status)
            {
                commData.SaveCommEvent("MDWSLogin_FAILED",
                                        status.StatusComment);
                return status;
            }

            //set the user id on the CData object
            data.UserID = lUserID;

            //create the session so that we can call stored proc
            CUserData ud = new CUserData(data);
            string strFXSessionID = String.Empty;
            status = ud.CreateFXSession(out strFXSessionID);
            if (!status.Status)
            { 
                commData.SaveCommEvent("MDWSSessionCreate_FAILED",
                                        status.StatusComment);
                return status;
            }

            return status;
        }

        /// <summary>
        /// US:834 get connection info form the web.config
        /// </summary>
        /// <param name="strConnString"></param>
        /// <param name="bAudit"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        private CStatus GetConnectionInfo(out string strConnString, out bool bAudit)
        {
            //initialize out params
            CStatus status = new CStatus();
            strConnString = string.Empty;
            bAudit = false;

            //get the connection string from the web.config file
            try
            {
                //try to get the connection string from the encrypted 
                //connectionstrings section
                //strConnString = ConfigurationManager.ConnectionStrings["VAPPCConn"].ConnectionString;

                //todo; using appsettings
                strConnString = ConfigurationSettings.AppSettings["VAPPCTCONN"];

            }
            catch (Exception ex)
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = ex.Message;
                return status;
            }

            //get the audit flag from the web.config
            string strAudit = string.Empty;
            if (ConfigurationSettings.AppSettings["AUDIT"] != null)
            {
                strAudit = ConfigurationSettings.AppSettings["AUDIT"].ToString();
                if (strAudit == "1")
                {
                    bAudit = true;
                }
            }

            return status;
        }
    }

