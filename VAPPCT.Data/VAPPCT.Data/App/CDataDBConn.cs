using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Summary description for CAppDBConn
/// </summary>
public class CDataDBConn : CDataConnection
{
    //default status
    const string k_DEFAULT_STATUS = "";


    //constructor
    public CDataDBConn()
    {

    }

    /// <summary>
    /// Connect to the database using info from the web.config file
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus Connect()
    {
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
        return Connect(strConnString, bAudit);
    }

    /// <summary>
    /// get connection info form the web.config
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
            strConnString = ConfigurationManager.ConnectionStrings["VAPPCConn"].ConnectionString;
       
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
        if (ConfigurationManager.AppSettings["AUDIT"] != null)
        {
            strAudit = ConfigurationManager.AppSettings["AUDIT"].ToString();
            if (strAudit == "1")
            {
                bAudit = true;
            }
        }

        return status;
    }
}
