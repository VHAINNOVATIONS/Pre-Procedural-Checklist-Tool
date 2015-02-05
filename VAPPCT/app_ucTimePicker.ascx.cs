using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using VAPPCT.DA;
using VAPPCT.UI;

public partial class app_ucTimePicker : System.Web.UI.UserControl
{
    /// <summary>
    /// sets the time given a date
    /// </summary>
    /// <param name="dt"></param>
    public void SetTime(DateTime dt)
    {
        if (dt == null)
        {
            return;
        }

        SetTime(dt.Hour, dt.Minute, dt.Second);
    }

    /// <summary>
    /// sets the time given hh,mm,ss
    /// </summary>
    /// <param name="lHH"></param>
    /// <param name="lMM"></param>
    /// <param name="lSS"></param>
    public void SetTime(long lHH, long lMM, long lSS)
    {
        string strHH = Convert.ToString(lHH);
        if (lHH < 10)
        {
            strHH = "0" + strHH;
        }
        CDropDownList.SelectItemByText(ddlHH, strHH);

        string strMM = Convert.ToString(lMM);
        if (lMM < 10)
        {
            strMM = "0" + strMM;
        }
        CDropDownList.SelectItemByText(ddlMM, strMM);

        string strSS = Convert.ToString(lSS);
        if (lSS < 10)
        {
            strSS = "0" + strSS;
        }
        CDropDownList.SelectItemByText(ddlSS, strSS);
    }

    /// <summary>
    /// readonly property
    /// </summary>
    public bool Enabled
    {
        get { return ddlHH.Enabled; }
        set
        {
            lblHH.Enabled = value;
            ddlHH.Enabled = value;
            lblMM.Enabled = value;
            ddlMM.Enabled = value;
            lblSS.Enabled = value;
            ddlSS.Enabled = value;
        }
    }
    
    /// <summary>
    /// hours property
    /// </summary>
    public int HH
    {
        get { return Convert.ToInt32(ddlHH.Text); }
        set
        {
            string strHH = (value < 10) ? "0" + value.ToString() : value.ToString();
            CDropDownList.SelectItemByText(ddlHH, strHH);
        }
    }

    /// <summary>
    /// minutes property
    /// </summary>
    public int MM
    {
        get { return Convert.ToInt32(ddlMM.Text); }
        set
        {
            string strMM = (value < 10) ? "0" + value.ToString() : value.ToString();
            CDropDownList.SelectItemByText(ddlMM, strMM);
        }
    }

    /// <summary>
    /// seconds property
    /// </summary>
    public int SS
    {
        get { return Convert.ToInt32(ddlSS.Text); }
        set
        {
            string strSS = (value < 10) ? "0" + value.ToString() : value.ToString();
            CDropDownList.SelectItemByText(ddlSS, strSS);
        }
    }

    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if(ddlHH.Items.Count<1)
        {
            //load the HH ddl
            for(int i = 0; i < 24; i++)
            {
                string strValue = (i < 10) ? "0" + i.ToString() : i.ToString();
                ddlHH.Items.Add(strValue);
            }

            //load the minutes ddl
            for (int i = 0; i < 60; i++)
            {
                string strValue = (i < 10) ? "0" + i.ToString() : i.ToString();
                ddlMM.Items.Add(strValue);
                ddlSS.Items.Add(strValue);
            }
        }
    }
}
