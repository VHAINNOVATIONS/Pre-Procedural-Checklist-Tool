using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace VAPPCT.UI
{
    /// <summary>
    /// This class contains methods to load, set/get Panel state and properties
    /// </summary>
    public class CPanel
    {
        /// <summary>
        /// builds javascript used to re-position the scroll bar of 
        /// a panel on postback
        /// </summary>
        /// <param name="pnl"></param>
        /// <returns></returns>
        public string GetScrollJS(Panel pnl)
        {
            string strJS = String.Empty;
            if (pnl == null)
            {
                return strJS;
            }

            string strClientID = pnl.ClientID;

            string strJSID = strClientID;
            if (strJSID.Length > 6)
            {
                strJSID = strJSID.Substring(strJSID.Length - 6);
            }

            //add somthing random to the js id so we 
            //can use the same control twice from the same page or uc
            System.Threading.Thread.Sleep(1);
            DateTime dt = DateTime.Now;
            Random rnd = new Random(dt.Millisecond);
            int rRandValue = rnd.Next(100, 900);
            strJSID += Convert.ToString(rRandValue);
            
            strJS += "<script type=\"text/javascript\">";
            strJS += "var xPos" + strJSID + ", yPos" + strJSID + ";";
            strJS += "function BeginRequest" + strJSID + "(sender, args) {";
            strJS += "    xPos" + strJSID + " = $get('" + strClientID + "').scrollLeft;";
            strJS += "    yPos" + strJSID + " = $get('" + strClientID + "').scrollTop;";
            strJS += "}";
            strJS += "function EndRequest" + strJSID + "(sender, args) {";
            strJS += "    setTimeout('ResetScroll" + strJSID + "Sel()', 0);";
            strJS += "}";
            strJS += "function ResetScroll" + strJSID + "Sel() {";
            strJS += "    $get('" + strClientID + "').scrollLeft = xPos" + strJSID + ";";
            strJS += "    $get('" + strClientID + "').scrollTop = yPos" + strJSID + ";";
            strJS += "}";
            strJS += "function appl_init" + strJSID + "() {";
            strJS += "    var prm" + strJSID + " = Sys.WebForms.PageRequestManager.getInstance();";
            strJS += "    if (!prm" + strJSID + ".get_isInAsyncPostBack()) {";
            strJS += "        prm" + strJSID + ".add_beginRequest(BeginRequest" + strJSID + ");";
            strJS += "        prm" + strJSID + ".add_endRequest(EndRequest" + strJSID + ");";
            strJS += "    } ";
            strJS += "}";
            strJS += "Sys.Application.add_init(appl_init" + strJSID + ");";
            strJS += "</script> ";
    
            return strJS;
        }
    }
}
