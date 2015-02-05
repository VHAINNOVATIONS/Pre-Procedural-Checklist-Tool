using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

//our data access 
using VAPPCT.DA;

namespace VAPPCT.UI
{
    /// <summary>
    /// This class contains methods to load, set/get 
    /// Menu state and properties
    /// </summary>
    public class CMenu
    {
        /// <summary>
        /// US:837 uses the menu and dataset passed in to draw the menu
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="mnu"></param>
        /// <param name="strTextField"></param>
        /// <param name="strNavigateURLField"></param>
        /// <param name="strToolTipField"></param>
        /// <param name="lStatusCode"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public CStatus RenderDataSet( DataSet ds,
                                   Menu mnu,
                                   string strTextField,  
                                   string strNavigateURLField,
                                   string strToolTipField,
                                   string strPage)     
        {
            CStatus status = new CStatus();
            if (mnu == null)
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = "Menu parameter null";
                return status;
            }

            if (ds != null)
            {
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        MenuItem mi = new MenuItem();
                        if (!row.IsNull(strTextField))
                        {
                            mi.Text = Convert.ToString(row[strTextField]);
                            if (row[strNavigateURLField].ToString().ToLower().Contains(strPage.ToLower()))
                            {
                                mi.Text = "<u>" + mi.Text + "</u>";
                            }

                        }

                        if (!row.IsNull(strNavigateURLField))
                        {
                            mi.NavigateUrl = Convert.ToString(row[strNavigateURLField]);
                        }

                        if (!row.IsNull(strToolTipField))
                        {
                            mi.ToolTip = Convert.ToString(row[strToolTipField]);
                        }

                        mnu.Items.Add(mi);
                    }
                }
            }

            return status;            
        }
    }
}
