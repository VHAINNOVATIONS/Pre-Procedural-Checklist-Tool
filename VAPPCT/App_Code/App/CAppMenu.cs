using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;

//access resource.resx

using System.Resources;

//our data access class library
using VAPPCT.DA;

//out UI library
using VAPPCT.UI;

/// <summary>
/// Summary description for CAppMenu
/// </summary>
public class CAppMenu
{
    //Constructor
    public CAppMenu()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary>
    /// US:837 loads the apps top main menu
    /// </summary>
    /// <param name="mnuMain"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    /// <returns></returns>
    public CStatus LoadMainMenu(CBaseMaster BaseMstr, Menu mnuMain)
    {
        CStatus status = new CStatus();
        if (mnuMain == null)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = Resources.ErrorMessages.ERROR_LOAD_MENU;
            return status;
        }

        DataSet dsMenu = null;
        CMenuData mnuData = new CMenuData(BaseMstr);
        status = mnuData.GetUserMenuOptionDS(out dsMenu);
        if (!status.Status)
        {
            return status;
        }

        //use our UI class to render the menu
        CMenu menu = new CMenu();
        status = menu.RenderDataSet(
            dsMenu,
            mnuMain,
            "menu_option_label",
            "menu_option_url",
            "menu_option_label",
            BaseMstr.GetPageName());
        if (!status.Status)
        {
            return status;
        }


        return status;
    }
}
