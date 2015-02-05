using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// Summary description for CAppUserControl
/// </summary>
public abstract class CAppUserControl : System.Web.UI.UserControl
{
    CParameterList m_plistOptions;
    protected CAppUserControl m_ucTimer;
    protected CBaseMaster m_BaseMstr;
    private GridView m_gv;
    protected AjaxControlToolkit.ModalPopupExtender m_MPE;
    protected AjaxControlToolkit.ModalPopupExtender m_ParentMPE;
    protected HtmlGenericControl m_divStatus;
    protected HtmlGenericControl m_divTitle;
    
    /// <summary>
    /// Options that can be used for initialization etc...
    /// </summary>
    public CParameterList Options
    {
        get
        {
            return m_plistOptions;
        }
    }

    /// <summary>
    /// property
    /// stores a timer control for user control
    /// </summary>
    public CAppUserControl UCTimer
    {
        get { return m_ucTimer; }
        set { m_ucTimer = value; }
    }
    
    /// <summary>
    /// Used by the control to cache a long identifier
    /// </summary>
    public long LongID
    {
        get
        {
            if (Session["UCLONGID" + this.ClientID] == null)
            {
                return -1;
            }
            else
            {
                return CDataUtils.ToLong(Session["UCLONGID" + this.ClientID].ToString());
            }
        }
        set
        {
            Session["UCLONGID" + this.ClientID] = value;
        }
    }
        
    /// <summary>
    /// Used by the control to cache a string identifier
    /// </summary>
    public string StringID
    {
        get
        {
            if (Session["UCSTRINGID" + this.ClientID] == null)
            {
                return String.Empty;
            }
            else
            {
                return Convert.ToString(Session["UCSTRINGID" + this.ClientID]);
            }
        }
        set
        {
            Session["UCSTRINGID" + this.ClientID] = value;
        }
    }

   
    /// <summary>
    /// Used by the control to cache a string identifier
    /// </summary>
    public string Title
    {
        get
        {
            if (Session["UCTITLE" + this.ClientID] == null)
            {
                return String.Empty;
            }
            else
            {
                return Convert.ToString(Session["UCTITLE" + this.ClientID]);
            }
        }
        set
        {
            Session["UCTITLE" + this.ClientID] = value;
        }
    }

    /// <summary>
    /// method
    /// shows the parent dialog and this control's dialog
    /// </summary>
    public void ShowMPE()
    {
        if (ParentMPE != null)
            ParentMPE.Show();

        if (MPE != null)
        {
            Visible = true;
            MPE.Show();
        }
    }

    /// <summary>
    /// method
    /// displays the parent dialog
    /// only call this when you do not want to show this control's dialog
    /// </summary>
    public void ShowParentMPE()
    {
        if (ParentMPE != null)
            ParentMPE.Show();

        if (MPE != null)
            MPE.Hide();

        Visible = false;
    }

    /// <summary>
    /// Property to hold a gridview of interest to the user control
    /// </summary>
    public GridView GView
    {
        get
        {
            return m_gv;
        }
        set
        {
            m_gv = value;
        }
    }
    
    /// <summary>
    /// Used by the control to cache the edit mode if needed
    /// </summary>
    public k_EDIT_MODE EditMode
    {
        get
        {
            if (Session["UCMODE" + this.ClientID] == null)
            {
                return k_EDIT_MODE.UNKNOWN;
            }
            else
            {
                return (k_EDIT_MODE)Convert.ToInt32(Session["UCMODE" + this.ClientID]);
            }
        }
        set 
        {
            Session["UCMODE" + this.ClientID] = value;
        }
    }

    
    /// <summary>
    /// get/set basemaster info
    /// </summary>
    public CBaseMaster BaseMstr
    {
        get { return m_BaseMstr; }
        set { m_BaseMstr = value; }
    }

    /// <summary>
    /// Get/Set modalpopup extender, for controls that need to re-display it
    /// after a postback
    /// </summary>
    public AjaxControlToolkit.ModalPopupExtender MPE
    {
        get { return m_MPE; }
        set { m_MPE = value; }
    }

    /// <summary>
    /// Get/Set PARENT modalpopup extender, for controls that 
    /// need to re-display the parents MPE 
    /// after a postback
    /// </summary>
    public AjaxControlToolkit.ModalPopupExtender ParentMPE
    {
        get { return m_ParentMPE; }
        set { m_ParentMPE = value; }
    }

    /// <summary>
    /// constructor
    /// </summary>
    public CAppUserControl()
	{
        m_BaseMstr = null;
        m_MPE = null;
        m_divStatus = new HtmlGenericControl();
        m_divTitle = new HtmlGenericControl();
        
        m_divStatus.InnerHtml = string.Empty;
        m_divTitle.InnerHtml = string.Empty;
        
        m_plistOptions = new CParameterList();
    }
    /// <summary>
    /// used to display status info
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    public void ShowStatusInfo(CStatus status)
    {
        if (status != null)
        {
            ShowStatusInfo(status.StatusCode, status.StatusComment);
        }
        else
        {
            ShowStatusInfo(k_STATUS_CODE.Failed, "Unable to retrieve status information");
        }
    }
    /// <summary>
    /// Used to display successful and unsuccessful status information
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatus"></param>
    public void ShowStatusInfo(k_STATUS_CODE lStatusCode, string strStatus)
    {
        if (string.IsNullOrEmpty(strStatus))
        {
            m_divStatus.InnerHtml = string.Empty;
            return;
        }

        StringBuilder sbHTML = new StringBuilder();
        sbHTML.Append("<table cellpadding=\"2\" width=\"99%\"><tr><td>");
        sbHTML.Append("<span style=\"font-family: verdana,arial; color: ");
        sbHTML.Append((lStatusCode == k_STATUS_CODE.Success) ? "darkgreen" : "darkred");
        sbHTML.Append(";\">");
        sbHTML.Append(Server.HtmlEncode(strStatus));
        sbHTML.Append("</span></td></tr></table><br />");

        if (m_divStatus != null)
        {
            m_divStatus.InnerHtml = sbHTML.ToString();
        }
    }

    /// <summary>
    /// Used to display successful and unsuccessful status information
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    public void ShowStatusInfo(k_STATUS_CODE lStatusCode, CParameterList plistStatus)
    {
        if (plistStatus.Count == 0)
        {
            m_divStatus.InnerHtml = string.Empty;
            return;
        }

        StringBuilder sbHTML = new StringBuilder();
        sbHTML.Append("<table cellpadding=\"2\" width=\"99%\"><tr><td>");
        sbHTML.Append("<span style=\"font-family: verdana,arial; color: ");
        sbHTML.Append((lStatusCode == k_STATUS_CODE.Success) ? "darkgreen" : "darkred");
        sbHTML.Append(";\">");

        for (int i = 0; i < plistStatus.Count; i++)
        {
            sbHTML.Append(Server.HtmlEncode(plistStatus[i].ToString()));
            sbHTML.Append("<br />");
        }

        sbHTML.Append("</span></td></tr></table><br />");

        if (m_divStatus != null)
        {
            m_divStatus.InnerHtml = sbHTML.ToString();
        }
    }

    /// <summary>
    /// Initializes Status Info
    /// </summary>
    public void ClearStatusInfo()
    {
        m_divStatus.InnerHtml = string.Empty;
    }
    
    /// <summary>
    /// Render status information
    /// </summary>
    /// <param name="output"></param>
    protected override void Render(HtmlTextWriter output)
    {
        //add the status div to the top of the control
        Controls.AddAt(0, m_divStatus);
        
        //render the control
        base.RenderChildren(output);
    }

    //"pure virtual" abstracts, must be overridden by the user control.
    
    /// <summary>
    /// Abstract to load the control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public abstract CStatus LoadControl(k_EDIT_MODE lEditMode);

    /// <summary>
    /// Abstract save control method
    /// </summary>
    /// <returns></returns>
    public abstract CStatus SaveControl();


    /// <summary>
    /// Abstract validate user input method
    /// </summary>
    /// <param name="BaseMstr"></param>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public abstract CStatus ValidateUserInput(out CParameterList plistStatus);
}
