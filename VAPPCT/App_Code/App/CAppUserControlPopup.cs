using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

//our data and ui libraries
using VAPPCT.DA;
using VAPPCT.UI;


/// <summary>
/// Summary description for CAppUserControlPopup
/// </summary>
public abstract class CAppUserControlPopup : CAppUserControl
{
    //Constructor
	public CAppUserControlPopup()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    
    /// <summary>
    /// Override of base render method
    /// </summary>
    /// <param name="output"></param>
    protected override void Render(HtmlTextWriter output)
    {
        m_divTitle.InnerHtml = "<div class=\"PopupTitle\">" + Title + "</div><div class=\"PopupSpacer\"></div>";

        //add the title div to the top of the control
        Controls.AddAt(0, m_divTitle);

        //add the status div to the top of the control
        Controls.AddAt(1, m_divStatus);

        //our custom rendering before the rest of the controls items
        output.Write("<div class=\"PopupBody\">");

        //render the control
        base.RenderChildren(output);

        //more custom rendering after the controls items
        output.Write("</div>");
    }
}
