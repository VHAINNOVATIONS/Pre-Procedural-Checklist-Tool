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

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Methods that pertain to the menu
/// </summary>
public class CMenuData : CBaseData
{
    //Constructor
    public CMenuData(CBaseMaster BaseMaster)
        : base(BaseMaster)
    {
        //constructors are not inherited in c#!
    }

    /// <summary>
    /// Gets the users menu options.
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus GetUserMenuOptionDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = new CStatus();
        status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID,
                                                  ClientIP,
                                                  UserID);
        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                     "PCK_USR.GetUserMenuOptionsRS",
                                     pList,
                                     out ds);
    }
}
