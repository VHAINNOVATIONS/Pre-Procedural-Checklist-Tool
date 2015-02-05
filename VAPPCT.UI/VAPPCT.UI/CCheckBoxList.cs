using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace VAPPCT.UI
{
    /// <summary>
    /// This class contains methods to set/get checkboxlist state and properties
    /// </summary>
    public class CCheckBoxList
    {
        /// <summary>
        /// checks rows in a checkboxlist(cbl) based on the values in the dataset(ds)
        /// </summary>
        /// <param name="cbl"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public bool CheckSelected(CheckBoxList cbl, DataSet ds)
        {
            cbl.SelectedIndex = -1;

            if (cbl == null || ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables[0];
            if (dt == null || dt.Columns[cbl.DataValueField] == null)
            {
                return false;
            }

            foreach (ListItem li in cbl.Items)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    li.Selected = (dr[cbl.DataValueField].ToString() == li.Value) ? true : li.Selected;
                }
            }

            return true;
        }

        /// <summary>
        /// gets the index of the item just checked
        /// </summary>
        /// <param name="req"></param>
        /// <param name="chklst"></param>
        /// <returns></returns>
        public int GetIndexJustSelected(HttpRequest req,
                                        CheckBoxList chklst)
        {
            //per microsoftm the checkbox item just selected is in the
            //_eventtarget: ctl00$ContentPlaceHolder1$chklstMAJCOM$1
            //              ctl00$ContentPlaceHolder1$chklstMAJCOM$2 etc...
            string strIndex = "";

            string strET = req.Form["__EVENTTARGET"];
            string strCtlID = chklst.ID;
            int nIndex = strET.IndexOf(strCtlID + "$");
            if (nIndex > -1)
            {
                strIndex = strET.Substring(nIndex + strCtlID.Length + 1);
                if (strIndex != "")
                {
                    return Convert.ToInt32(strIndex);
                }
            }

            return -1;
        }

        /// <summary>
        /// checks rows in a checkboxlist(cbl) based on the values in the datarow(dra)
        /// </summary>
        /// <param name="cbl"></param>
        /// <param name="dra"></param>
        /// <returns></returns>
        public bool CheckSelected(CheckBoxList cbl, DataRow[] dra)
        {
            cbl.SelectedIndex = -1;

            if (cbl == null || dra == null)
            {
                return false;
            }

            foreach (ListItem li in cbl.Items)
            {
                foreach (DataRow dr in dra)
                {
                    if (dr[cbl.DataValueField] == null)
                    {
                        return false;
                    }

                    li.Selected = (dr[cbl.DataValueField].ToString() == li.Value) ? true : li.Selected;
                }
            }

            return true;
        }
    }
}
