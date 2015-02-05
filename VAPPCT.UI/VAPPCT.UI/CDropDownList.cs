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

using VAPPCT.DA;

namespace VAPPCT.UI
{
    /// <summary>
    /// This class contains methods to load, set/get dropdownlist state 
    /// and properties
    /// </summary>
    public static class CDropDownList
    {
        /// <summary>
        /// select an item in the dropdownlist(cbo) by value(strvalue)
        /// </summary>
        /// <param name="cbo"></param>
        /// <param name="strValue"></param>
        public static void SelectItemByValue(
            DropDownList cbo,
            string strValue)
        {
            if (cbo == null)
            {
                //nothing to do
                return;
            }

            //loop, find and select
            foreach (ListItem itm in cbo.Items)
            {
                if (itm.Value.ToLower() == strValue.ToLower())
                {
                    cbo.SelectedValue = itm.Value;
                }
            }
        }

        /// <summary>
        /// select and item by text
        /// </summary>
        /// <param name="cbo"></param>
        /// <param name="strText"></param>
        public static void SelectItemByText(
            DropDownList cbo,
            string strText)
        {
            if (cbo == null)
            {
                //nothing to do
                return;
            }

            //loop, find and select
            foreach (ListItem itm in cbo.Items)
            {
                if (itm.Text.ToLower() == strText.ToLower())
                {
                    cbo.SelectedValue = itm.Value;
                }
            }
        }

        /// <summary>
        /// select an item in the dropdownlist(cbo) by value(lValue)
        /// </summary>
        /// <param name="cbo"></param>
        /// <param name="strValue"></param>
        public static void SelectItemByValue(
            DropDownList cbo,
            long lValue)
        {
            if (cbo == null)
            {
                //nothing to do
                return;
            }

            string strValue = Convert.ToString(lValue);
            SelectItemByValue(cbo, strValue);
        }

        /// <summary>
        /// get the selected ID of the item seleced in the dropdownliat ddl
        /// </summary>
        /// <param name="ddl"></param>
        /// <returns></returns>
        public static string GetSelectedID(DropDownList ddl)
        {
            if (ddl == null)
            {
                return string.Empty;
            }

            if (ddl.SelectedIndex < 0)
            {
                return String.Empty;
            }

            if (ddl.SelectedValue == null)
            {
                return String.Empty;
            }

            //get the selected id from the gridview
            return ddl.SelectedValue;
        }

        /// <summary>
        /// get the selected id as a long
        /// </summary>
        /// <param name="ddl"></param>
        /// <returns></returns>
        public static long GetSelectedLongID(DropDownList ddl)
        {
            string strID = GetSelectedID(ddl);
            return CDataUtils.ToLong(strID);
        }
        
        /// <summary>
        /// render a dataset as a drop down list
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cbo"></param>
        /// <param name="strTextFields"></param>
        /// <param name="strIDField"></param>
        /// <returns></returns>
        public static CStatus RenderDataSet(
            DataSet ds,
            DropDownList cbo,
            string strTextFields,  //comma delimeted / LastName,FirstName
            string strIDField) //field used to uniquely id a row     
        {
            //clear exisiting Items, set properties
            try
            {
                cbo.DataSource = null;
                cbo.Items.Clear();
            }
            catch (Exception ew)
            {
                string str = ew.Message;
                //nothing to do
            }

            //our cbo's always have an empty item
            cbo.Items.Add(new ListItem("", "-1"));

            CStatus status = new CStatus();
            //return if null
            if (ds == null)
            {
                return status;
            }

            //split text fields used to load
            string[] splitTextFields = strTextFields.Split(new Char[] { ',' });
            if (splitTextFields.Length < 1)//nothing to do
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = "Unable to load dropdownlist, invalid paramaters!";
                return status;
            }

            //loop over the dataset and load the dropdownlist
            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    //build the cbo text
                    string strCBOText = "";
                    foreach (string txtField in splitTextFields)
                    {
                        if (!row.IsNull(txtField))
                        {
                            string strValue = Convert.ToString(row[txtField]);
                            strCBOText += strValue;
                            strCBOText += " - ";
                        }
                    }

                    //strip last " - "
                    if (strCBOText.Length > 4)
                    {
                        strCBOText = strCBOText.Substring(0, strCBOText.Length - 3);
                    }

                    //set item properties
                    ListItem cboItm = new ListItem();
                    if (!row.IsNull(strIDField))
                    {
                        string strValue = Convert.ToString(row[strIDField]);
                        cboItm.Value = strValue;
                    }
                    cboItm.Text = strCBOText;
                    cbo.Items.Add(cboItm);
                }
            }

            if (cbo.Items.Count == 0)
            {
                cbo.Items.Add(new ListItem("No results found.", "-1"));
            }

            return status;
        }
    }
}
