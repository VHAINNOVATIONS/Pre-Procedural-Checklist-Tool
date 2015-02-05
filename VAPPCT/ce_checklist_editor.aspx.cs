using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ce_checklist_editor : System.Web.UI.Page
{
    /// <summary>
    /// event
    /// initializes popup user controsl
    /// sets page title
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucChecklistSelector.BaseMstr = Master;
        ucChecklistSelector.MPE = mpeChecklistSelect;

        ucSaveAs.BaseMstr = Master;
        ucSaveAs.MPE = mpeSaveAs;

        ucChecklistEntry.BaseMstr = Master;

        if (!IsPostBack)
        {
            Master.PageTitle = "Checklist Editor";

            CStatus status = ucChecklistEntry.LoadControl(k_EDIT_MODE.INITIALIZE);
            if(!status.Status)
            {
                Master.ShowStatusInfo(status);
            }
        }
    }

    /// <summary>
    /// event
    /// loads checklist selector user control
    /// displays checklist selector user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickLoad(object sender, EventArgs e)
    {
        CStatus status = ucChecklistSelector.LoadControl(k_EDIT_MODE.UPDATE);
        if(!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        //show the popup
        ucChecklistSelector.ShowMPE();
    }

    /// <summary>
    /// event
    /// clears the checklist controls of their values
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickNew(object sender, EventArgs e)
    {
        CStatus status = ucChecklistEntry.LoadControl(k_EDIT_MODE.INSERT);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        btnCLSave.Enabled = true;
        btnCLSaveAs.Enabled = true;
    }

    /// <summary>
    /// event
    /// validates the user's inputs
    /// saves the user's inputs if valid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSave(object sender, EventArgs e)
    {
        CParameterList pList = null;
        CStatus status = ucChecklistEntry.ValidateUserInput(out pList);
        if(!status.Status)
        {
            Master.ShowStatusInfo(status.StatusCode, pList);
            return;
        }

        status = ucChecklistEntry.SaveControl();
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        //check decision state changeable by and make sure its valid given the 
        //viewable and read only states of the checklist.


        Master.ShowStatusInfo(new CStatus());
    }

    /// <summary>
    /// event
    /// loads the checklist controls with the selected checklist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnChecklistSelect(object sender, CAppUserControlArgs e)
    {
        ucChecklistEntry.ChecklistID = Convert.ToInt64(e.EventData);
        CStatus status = ucChecklistEntry.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        btnCLSave.Enabled = true;
        btnCLSaveAs.Enabled = true;
    }

    /// <summary>
    /// event
    /// loads the checklist controls with the new checklist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSaveAsChecklist(object sender, CAppUserControlArgs e)
    {
        ucChecklistEntry.ChecklistID = Convert.ToInt64(e.EventData);
        CStatus status = ucChecklistEntry.LoadControl(k_EDIT_MODE.UPDATE);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        btnCLSave.Enabled = true;
        btnCLSaveAs.Enabled = true;
    }

    /// <summary>
    /// event
    /// loads the save as dialog
    /// displays the save as dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickSaveAs(object sender, EventArgs e)
    {
        ucSaveAs.ChecklistID = ucChecklistEntry.ChecklistID;
        CStatus status = ucSaveAs.LoadControl(k_EDIT_MODE.INSERT);
        if (!status.Status)
        {
            Master.ShowStatusInfo(status);
            return;
        }

        //show the popup
        ucSaveAs.ShowMPE();
    }
}
