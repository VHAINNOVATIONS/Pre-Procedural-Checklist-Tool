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
    public class CListBox
    {
        /// <summary>
        /// US:838
        /// filter list box by search options
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="strFilter"></param>
        public void FilterListBox(ListBox lb, string strFilter)
        {
            if (!String.IsNullOrEmpty(strFilter))
            {
                string strSearch = strFilter.ToLower();

                for (int i = 0; i < lb.Items.Count; i++)
                {
                    ListItem itm = lb.Items[i];
                    if (itm != null)
                    {
                        string strMatch = itm.Text.ToLower();
                        if (!String.IsNullOrEmpty(strMatch))
                        {
                            if (strMatch.IndexOf(strSearch) == -1)
                            {
                                lb.Items.Remove(itm);
                                i--;
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// US:838
        /// loads the listbox with data from a dataset
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="lst"></param>
        /// <param name="strEmptyRowLabel"></param>
        /// <param name="strTextFields"></param>
        /// <param name="strIDField"></param>
        /// <returns></returns>
        public CStatus RenderDataSet(DataSet ds,
                                     ListBox lst,
                                     string strEmptyRowLabel,
                                     string strTextFields,  //comma delimeted / LastName,FirstName
                                     string strIDField) //field used to uniquely id a row     
        {
            //clear exisiting Items, set properties
            try
            {
                lst.DataSource = null;
                lst.Items.Clear();
            }
            catch (Exception ew)
            {
                string str = ew.Message;
                //nothing to do
            }

            //our lst's always have an empty item
            ListItem itm = new ListItem();
            itm.Value = "-1";
            itm.Text = strEmptyRowLabel;
            lst.Items.Add(itm);

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
                    //build the lst text
                    string strlstText = "";
                    foreach (string txtField in splitTextFields)
                    {
                        if (!row.IsNull(txtField))
                        {
                            string strValue = Convert.ToString(row[txtField]);
                            strlstText += strValue;
                            strlstText += " - ";
                        }
                    }

                    //strip last " - "
                    if (strlstText.Length > 4)
                    {
                        strlstText = strlstText.Substring(0, strlstText.Length - 3);
                    }

                    //set item properties
                    ListItem lstItm = new ListItem();
                    if (!row.IsNull(strIDField))
                    {
                        string strValue = Convert.ToString(row[strIDField]);
                        lstItm.Value = strValue;
                    }
                    lstItm.Text = strlstText;
                    lst.Items.Add(lstItm);
                }
            }
            return status;
        }
    }
}
