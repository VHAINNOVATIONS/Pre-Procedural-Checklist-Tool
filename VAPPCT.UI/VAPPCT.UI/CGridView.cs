using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Text;
using VAPPCT.DA;

namespace VAPPCT.UI
{
    public static class CGridView
    {
        /// <summary>
        /// method
        /// checks the specified checkbox in the gridview based on a ds active field
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strCheckBoxID"></param>
        /// <param name="ds"></param>
        /// <param name="strIDFieldName"></param>
        /// <param name="strActiveFieldName"></param>
        public static void SetActive(
            GridView gv,
            string strCheckBoxID,
            DataSet ds,
            string strIDFieldName,
            string strActiveFieldName)
        {
            if (gv == null || ds == null)
            {
                return;
            }

            foreach (GridViewRow gvr in gv.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl(strCheckBoxID);
                if (cb == null)
                {
                    return;
                }

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    long lDSIdValue = CDataUtils.GetDSLongValue(row, strIDFieldName);
                    if (lDSIdValue < 1)
                    {
                        continue;
                    }

                    long lGVIdValue = CDataUtils.ToLong(gv.DataKeys[gvr.RowIndex].Value.ToString());
                    if(lDSIdValue == lGVIdValue)
                    {
                        long lIsActive = CDataUtils.GetDSLongValue(row, strActiveFieldName);
                        cb.Checked = (lIsActive == 1) ? true : false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// US:931
        /// method
        /// selects the first row with a data key value matching the specified ID
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="lID"></param>
        public static void SetSelectedRow(
            GridView gv,
            long lID)
        {
            SetSelectedRow(gv, lID.ToString());
        }

        /// <summary>
        /// US:931
        /// method
        /// selects the first row with a data key value matching the specified ID
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strID"></param>
        public static void SetSelectedRow(
            GridView gv,
            string strID)
        {
            if (gv == null)
            {
                return;
            }

            gv.SelectedIndex = -1;

            if (string.IsNullOrEmpty(strID))
            {
                return;
            }

            foreach (GridViewRow gvr in gv.Rows)
            {
                if (gv.DataKeys[gvr.RowIndex].Value.ToString() == strID)
                {
                    gv.SelectedIndex = gvr.RowIndex;
                    break;
                }
            }
        }

        /// <summary>
        /// US:931
        /// method
        /// selects the first row with a data key value at the specified
        /// index matching the specified ID
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="nIndex"></param>
        /// <param name="lID"></param>
        public static void SetSelectedRow(
            GridView gv,
            int nIndex,
            long lID)
        {
            SetSelectedRow(gv, nIndex, lID.ToString());
        }

        /// <summary>
        /// US:931
        /// method
        /// selects the first row with a data key value at the specified
        /// index matching the specified ID
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="nIndex"></param>
        /// <param name="strID"></param>
        public static void SetSelectedRow(
            GridView gv,
            int nIndex,
            string strID)
        {
            if (gv == null)
            {
                return;
            }

            gv.SelectedIndex = -1;

            if (string.IsNullOrEmpty(strID))
            {
                return;
            }

            foreach (GridViewRow gvr in gv.Rows)
            {
                if (gv.DataKeys[gvr.RowIndex].Values[nIndex].ToString() == strID)
                {
                    gv.SelectedIndex = gvr.RowIndex;
                    break;
                }
            }
        }

        /// <summary>
        /// US:931
        /// method
        /// selects the first row with a data key matching the specified data key
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="dk"></param>
        public static void SetSelectedRow(
            GridView gv,
            DataKey dk)
        {
            if (gv == null)
            {
                return;
            }

            gv.SelectedIndex = -1;

            if (dk == null)
            {
                return;
            }

            foreach (GridViewRow gvr in gv.Rows)
            {
                if (gv.DataKeys[gvr.RowIndex] == dk)
                {
                    gv.SelectedIndex = gvr.RowIndex;
                    break;
                }
            }
        }

        /// <summary>
        /// US:931
        /// method
        /// returns a comma delimited string of data key values of all the rows that do not
        /// have the specified checkbox checked
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strCheckBoxID"></param>
        /// <returns></returns>
        public static string GetUncheckedRows(
            GridView gv,
            string strCheckBoxID)
        {
            if (gv == null || string.IsNullOrEmpty(strCheckBoxID))
            {
                return null;
            }

            StringBuilder sbIDs = new StringBuilder(",");

            foreach (GridViewRow gvr in gv.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl(strCheckBoxID);
                if (cb == null)
                {
                    return null;
                }

                if (!cb.Checked)
                {
                    sbIDs.Append(gv.DataKeys[gvr.RowIndex].Value.ToString() + ",");
                }
            }

            return sbIDs.ToString();
        }

        /// <summary>
        /// method
        /// returns a list of strings that do not have the specified checkbox checked
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strCheckBoxID"></param>
        /// <returns></returns>
        public static List<string> GetUncheckedRowsList(
            GridView gv,
            string strCheckBoxID)
        {
            if (gv == null || string.IsNullOrEmpty(strCheckBoxID))
            {
                return null;
            }

            List<string> lstIDs = new List<string>();

            foreach (GridViewRow gvr in gv.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl(strCheckBoxID);
                if (cb == null)
                {
                    return null;
                }

                if (!cb.Checked)
                {
                    lstIDs.Add(gv.DataKeys[gvr.RowIndex].Value.ToString());
                }
            }

            return lstIDs;
        }

        /// <summary>
        /// US:931
        /// method
        /// returns a comma delimited list of data key values of all the rows that
        /// have the specified checkbox checked;
        /// the list returned is an updated version of the list passed in
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strIDs"></param>
        /// <param name="strCheckBoxID"></param>
        /// <returns></returns>
        public static string GetCheckedRows(
            GridView gv,
            string strCheckBoxID)
        {
            if (gv == null || string.IsNullOrEmpty(strCheckBoxID))
            {
                return null;
            }

            StringBuilder sbIDs = new StringBuilder(",");

            foreach (GridViewRow gvr in gv.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl(strCheckBoxID);
                if (cb == null)
                {
                    return null;
                }

                if (cb.Checked)
                {
                    sbIDs.Append(gv.DataKeys[gvr.RowIndex].Value.ToString() + ",");
                }
            }

            return sbIDs.ToString();
        }

        /// <summary>
        /// US:931
        /// method
        /// checks the specified checkbox for a row if the row's data key
        /// value is found in the passed IDs
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strIDs"></param>
        /// <param name="strCheckBoxID"></param>
        public static void SetCheckedRows(
            GridView gv,
            string strIDs,
            string strCheckBoxID)
        {
            if (gv == null || string.IsNullOrEmpty(strCheckBoxID))
            {
                return;
            }

            foreach (GridViewRow gvr in gv.Rows)
            {
                CheckBox cb = (CheckBox)gvr.FindControl(strCheckBoxID);
                if (cb == null)
                {
                    return;
                }

                cb.Checked = (strIDs.Contains("," + gv.DataKeys[gvr.RowIndex].Value.ToString() + ",")) ? true : false;
            }
        }

        /// <summary>
        /// method
        /// searches the specified column for a match of the specified value
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="nCellIndex"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool CellValueExists(
            GridView gv,
            int nCellIndex,
            string strValue)
        {
            if (gv == null
                || nCellIndex >= gv.Columns.Count)
            {
                return false;
            }

            foreach (GridViewRow gvr in gv.Rows)
            {
                if (gvr.Cells[nCellIndex].Text.ToUpper() == strValue.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// method
        /// gets the cell text from the selected row for the specified column
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="nCellIndex"></param>
        /// <returns></returns>
        public static string GetSelectedCellValue(
            GridView gv,
            int nCellIndex)
        {
            if (gv == null
                || nCellIndex >= gv.Columns.Count)
            {
                return string.Empty;
            }

            GridViewRow gvr = gv.SelectedRow;
            if(gvr == null)
            {
                return string.Empty;
            }

            return gvr.Cells[nCellIndex].Text;
        }

        /// <summary>
        /// method
        /// formats all null dates in the grid using the label passed in
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strNullLabel"></param>
        public static void FormatNullDates(
            GridView gv,
            string strNullLabel)
        {
            foreach (GridViewRow gvr in gv.Rows)
            {
                foreach (TableCell cell in gvr.Cells)
                {
                    if (cell.Text.Equals("1/1/0001 12:00:00 AM"))
                    {
                        cell.Text = strNullLabel;
                    }
                }
            }
        }

        /// <summary>
        /// method
        /// set the fore color of the specified link button in the selected row
        /// in the specified grid view to the specified color
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="strLinkButtonID"></param>
        /// <param name="strSkinID"></param>
        public static void SetSelectedLinkButtonForeColor(
            GridView gv,
            string strID,
            Color clrForeColor)
        {
            GridViewRow gvrSelected = gv.SelectedRow;
            if (gvrSelected != null)
            {
                LinkButton lnkSelect = (LinkButton)gvrSelected.FindControl(strID);
                if (lnkSelect == null)
                {
                    return;
                }

                lnkSelect.ForeColor = clrForeColor;
                lnkSelect.Focus();
            }
        }
    }
}
