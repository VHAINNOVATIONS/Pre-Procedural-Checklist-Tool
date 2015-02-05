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
    /// This class contains methods to set/get checkbox state and properties
    /// </summary>
    public class CCheckBox
    {
        /// <summary>
        /// Checks or unchecks the checkbox (chk) based on the 
        /// value of active_id in the dataset (ds)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="chk"></param>
        public void SetActive(CheckBox chk, DataSet ds)
        {
            if(chk == null)
            {
                return;
            }

            if (CDataUtils.GetDSLongValue(ds, "ACTIVE_ID") == (long)k_ACTIVE.ACTIVE)
            {
                chk.Checked = true;
            }
            else
            {
                chk.Checked = false;
            }
        }

        /// <summary>
        /// returns active_id based on the checked state of the checkbox (chk)
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public k_ACTIVE GetActiveID(CheckBox chk)
        {
            if (chk == null)
            {
                return k_ACTIVE.INACTIVE;
            }

            if (chk.Checked)
            {
                return k_ACTIVE.ACTIVE;
            }

            return k_ACTIVE.INACTIVE;
        }
    }
}
