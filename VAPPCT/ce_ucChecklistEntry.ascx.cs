using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class ce_ucChecklistEntry : CAppUserControl
{
    /// <summary>
    /// property
    /// stores a checklist id in the session
    /// retrieves a checklist id from session
    /// </summary>
    public long ChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistID"] = value; }
    }

    /// <summary>
    /// property
    /// US:1951 US:1945 US:1883 enables the checklist controls
    /// </summary>
    /// <param name="bEnable"></param>
    public bool Enable
    {
        get { return txtCLLabel.Enabled; }
        set
        {
            txtCLLabel.Enabled = value;
            ddlCLService.Enabled = value;
            txtCLDesc.Enabled = value;
            btnMap.Enabled = value;
            chkActive.Enabled = value;
            cblViewable.Enabled = value;
            cblReadOnly.Enabled = value;
            cblCloseable.Enabled = value;
            cblTIUNote.Enabled = value;
            ucItemEntry.AddEnabled = value;
            ddlClinics.Enabled = value;
            txtCLNoteTitle.Enabled = value;
        }
    }

    /// <summary>
    /// US:1945 event
    /// initializes child user controls
    /// loads static control data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucItemEntry.BaseMstr = BaseMstr;

        ucMapNoteTitle.BaseMstr = BaseMstr;
        ucMapNoteTitle.MPE = mpeMap;
    }

    /// <summary>
    /// method
    /// loads the checklist's user role check box lists
    /// </summary>
    /// <returns></returns>
    protected CStatus LoadChecklistCBLs()
    {
        if (cblViewable.Items.Count < 1 || 
            cblReadOnly.Items.Count < 1 || 
            cblCloseable.Items.Count < 1 || 
            cblTIUNote.Items.Count < 1)
        {
            CSTATData STAT = new CSTATData(BaseMstr.BaseData);
            DataSet dsUserRoles = null;
            CStatus status = STAT.GetUserRolesDS(out dsUserRoles);
            if (!status.Status)
            {
                return status;
            }

            cblViewable.DataSource = dsUserRoles;
            cblViewable.DataBind();

            cblReadOnly.DataSource = dsUserRoles;
            cblReadOnly.DataBind();

            cblCloseable.DataSource = dsUserRoles;
            cblCloseable.DataBind();

            cblTIUNote.DataSource = dsUserRoles;
            cblTIUNote.DataBind();

        }

        return new CStatus();
    }

    /// <summary>
    /// US:1951 method
    /// loads the checklist's service drop down list
    /// </summary>
    /// <returns></returns>
    protected CStatus LoadChecklistDDLs()
    {
        if (ddlCLService.Items.Count < 1)
        {
            CStatus status = CService.LoadServiceDDL(
                BaseMstr.BaseData,
                k_ACTIVE_ID.Active,
                ddlCLService);
            if (!status.Status)
            {
                return status;
            }
        }

        if (ddlClinics.Items.Count < 1)
        {
            CStatus status = CClinic.LoadClinicDLL(BaseMstr.BaseData, ddlClinics);
            if (!status.Status)
            {
                return status;
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// US:1951 US:1945 method
    /// loads checklist data from the database into the checklist's controls
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    protected CStatus LoadChecklist()
    {
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        CChecklistDataItem clData = null;
        CStatus status = cld.GetCheckListDI(ChecklistID, out clData);
        if(!status.Status)
        {
            return status;
        }

        ChecklistID = clData.ChecklistID;
        txtCLLabel.Text = clData.ChecklistLabel;
        
        txtCLNoteTitle.Text = clData.NoteTitleTag;
        if(txtCLNoteTitle.Text == "-1")
        {
            txtCLNoteTitle.Text = "";
        }

        chkActive.Checked = (clData.ActiveID == k_ACTIVE_ID.Active) ? true : false;
        txtCLDesc.Text = clData.ChecklistDescription;

        try
        {
            ddlClinics.SelectedValue = Convert.ToString(clData.NoteTitleClinicID);
        }
        catch (Exception)
        {
            ddlClinics.SelectedIndex = -1;
        }

        try
        {
            ddlCLService.SelectedValue = Convert.ToString(clData.ServiceID);
        }
        catch (Exception)
        {
            ddlCLService.SelectedIndex = -1;
        }

        DataSet dsViewable = null;
        status = cld.GetCLViewableRolesDS(ChecklistID, out dsViewable);
        if(!status.Status)
        {
            return status;
        }
        CCheckBoxList cbl = new CCheckBoxList();
        cbl.CheckSelected(cblViewable, dsViewable);

        DataSet dsReadOnly = null;
        status = cld.GetCLReadOnlyRolesDS(ChecklistID, out dsReadOnly);
        if(!status.Status)
        {
            return status;
        }
        cbl.CheckSelected(cblReadOnly, dsReadOnly);

        DataSet dsCloseable = null;
        status = cld.GetCLCloseableRolesDS(ChecklistID, out dsCloseable);
        if(!status.Status)
        {
            return status;
        }
        cbl.CheckSelected(cblCloseable, dsCloseable);

        DataSet dsTIU = null;
        status = cld.GetCLTIURolesDS(ChecklistID, out dsTIU);
        if (!status.Status)
        {
            return status;
        }
        cbl.CheckSelected(cblTIUNote, dsTIU);

        return new CStatus();
    }

    /// <summary>
    /// US:1951 US:1945 override
    /// loads a specified checklist onto the screen's controls
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        CStatus status = LoadChecklistDDLs();
        if(!status.Status)
        {
            return status;
        }

        status = LoadChecklistCBLs();
        if (!status.Status)
        {
            return status;
        }

        bool bEnable = false;
        if (EditMode == k_EDIT_MODE.UPDATE)
        {
            status = LoadChecklist();
            if(!status.Status)
            {
                return status;
            }

            ucItemEntry.ChecklistID = ChecklistID;
            status = ucItemEntry.LoadControl(EditMode);
            if (!status.Status)
            {
                return status;
            }
            
            bEnable = true;
        }
        else if (EditMode == k_EDIT_MODE.INSERT)
        {
            ChecklistID = 0;
            txtCLLabel.Text = string.Empty;
            ddlCLService.SelectedIndex = 0;
            txtCLDesc.Text = string.Empty;
            chkActive.Checked = true;
            txtCLNoteTitle.Text = string.Empty;
            ddlClinics.SelectedIndex = 0;
            cblViewable.ClearSelection();
            cblReadOnly.ClearSelection();
            cblCloseable.ClearSelection();
            cblTIUNote.ClearSelection();

            status = ucItemEntry.LoadControl(EditMode);
            if (!status.Status)
            {
                return status;
            }

            bEnable = true;
        }

        Enable = bEnable;

        return new CStatus();
    }

    /// <summary>
    /// US:1951 US:1945 method
    /// saves the values in the checklist controls as a checklist in the database
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    protected CStatus SaveChecklist()
    {
        CChecklistDataItem cldi = new CChecklistDataItem();
        cldi.ChecklistLabel = txtCLLabel.Text;
        cldi.ChecklistDescription = txtCLDesc.Text;
        cldi.NoteTitleTag = string.IsNullOrEmpty(txtCLNoteTitle.Text) ? "-1" : txtCLNoteTitle.Text;
        cldi.NoteTitleClinicID = CDataUtils.ToLong(ddlClinics.SelectedValue);

        cldi.ServiceID = CDataUtils.ToLong(ddlCLService.SelectedValue);
        cldi.ActiveID = (chkActive.Checked) ? k_ACTIVE_ID.Active : k_ACTIVE_ID.Inactive;

        CChecklistData CLData = new CChecklistData(BaseMstr.BaseData);
        CStatus status = new CStatus();
        if (EditMode == k_EDIT_MODE.INSERT)
        {
            long lChecklistID = 0;
            status = CLData.InsertChecklist(cldi, out lChecklistID);
            if (status.Status)
            {
                EditMode = k_EDIT_MODE.UPDATE;
                ChecklistID = lChecklistID;
            }
            else
            {
                EditMode = k_EDIT_MODE.INSERT;
                ChecklistID = 0;
            }
        }
        else if (EditMode == k_EDIT_MODE.UPDATE)
        {
            cldi.ChecklistID = ChecklistID;
            status = CLData.UpdateChecklist(cldi);
        }

        return status;
    }

    /// <summary>
    /// method
    /// saves the viewable roles for the checklist in the database
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    protected CStatus SaveCLViewable()
    {
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        CStatus status = cld.DeleteAllCLViewableRoles(ChecklistID);
        if(!status.Status)
        {
            return status;
        }

        foreach (ListItem li in cblViewable.Items)
        {
            if (li.Selected)
            {
                CCLViewableDataItem di = new CCLViewableDataItem();
                di.ChecklistID = ChecklistID;
                di.UserRoleID = Convert.ToInt32(li.Value);

                status = cld.InsertCLViewableRole(di);
                if(!status.Status)
                {
                    return status;
                }
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// saves the checklist's read only roles in the database
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    protected CStatus SaveCLReadOnly()
    {
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        CStatus status = cld.DeleteAllCLReadOnlyRoles(ChecklistID);
        if(!status.Status)
        {
            return status;
        }

        foreach (ListItem li in cblReadOnly.Items)
        {
            if (li.Selected)
            {
                CCLReadOnlyDataItem di = new CCLReadOnlyDataItem();
                di.ChecklistID = ChecklistID;
                di.UserRoleID = Convert.ToInt32(li.Value);

                status = cld.InsertCLReadOnlyRole(di);
                if(!status.Status)
                {
                    return status;
                }
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// saves the checklist's closeable roles in the database
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    protected CStatus SaveCLCloseable()
    {
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        CStatus status = cld.DeleteAllCLCloseableRoles(ChecklistID);
        if(!status.Status)
        {
            return status;
        }

        foreach (ListItem li in cblCloseable.Items)
        {
            if (li.Selected)
            {
                CCLCloseableDataItem di = new CCLCloseableDataItem();
                di.ChecklistID = ChecklistID;
                di.UserRoleID = Convert.ToInt32(li.Value);

                status = cld.InsertCLCloseableRole(di);
                if(!status.Status)
                {
                    return status;
                }
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// saves checklist TIU permissions
    /// </summary>
    /// <returns></returns>
    protected CStatus SaveCLTIU()
    {
        CChecklistData cld = new CChecklistData(BaseMstr.BaseData);
        CStatus status = cld.DeleteAllCLTIURoles(ChecklistID);
        if (!status.Status)
        {
            return status;
        }

        foreach (ListItem li in cblTIUNote.Items)
        {
            if (li.Selected)
            {
                CCLTIUDataItem di = new CCLTIUDataItem();
                di.ChecklistID = ChecklistID;
                di.UserRoleID = Convert.ToInt32(li.Value);

                status = cld.InsertCLTIURole(di);
                if (!status.Status)
                {
                    return status;
                }
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// override
    /// calls the saves methods for the checklist and items
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        CStatus status = SaveChecklist();
        if (!status.Status)
        {
            return status;
        }

        status = SaveCLViewable();
        if (!status.Status)
        {
            return status;
        }

        status = SaveCLReadOnly();
        if (!status.Status)
        {
            return status;
        }

        status = SaveCLCloseable();
        if (!status.Status)
        {
            return status;
        }

        status = SaveCLTIU();
        if (!status.Status)
        {
            return status;
        }

        ucItemEntry.ChecklistID = ChecklistID;
        status = ucItemEntry.SaveControl();
        if (!status.Status)
        {
            return status;
        }

        return new CStatus();
    }

    /// <summary>
    /// override
    /// validates the user's input for the checklist fields
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    protected CStatus ValidateChecklist(out CParameterList plistStatus)
    {
        CStatus status = new CStatus();
        plistStatus = new CParameterList();

        if (String.IsNullOrEmpty(txtCLLabel.Text))
        {
            plistStatus.AddInputParameter("ERROR_CE_LABEL", Resources.ErrorMessages.ERROR_CE_LABEL);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        if (ddlCLService.SelectedIndex == 0)
        {
            plistStatus.AddInputParameter("ERROR_CE_SERVICE", Resources.ErrorMessages.ERROR_CE_SERVICE);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        if (String.IsNullOrEmpty(txtCLDesc.Text))
        {
            plistStatus.AddInputParameter("ERROR_CE_DESCRIPTION", Resources.ErrorMessages.ERROR_CE_DESCRIPTION);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        if (cblViewable.SelectedIndex == -1)
        {
            plistStatus.AddInputParameter("ERROR_CE_VIEWABLE", Resources.ErrorMessages.ERROR_CE_VIEWABLE);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        if (cblCloseable.SelectedIndex == -1)
        {
            plistStatus.AddInputParameter("ERROR_CE_CLOSEABLE", Resources.ErrorMessages.ERROR_CE_CLOSEABLE);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        if (cblTIUNote.SelectedIndex == -1)
        {
            plistStatus.AddInputParameter("ERROR_CE_TIU", Resources.ErrorMessages.ERROR_CE_TIU);
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        //make sure the ds changeble 
        //match the permissions for the checklist
        status = ucItemEntry.ValidateDSChangeableRoles(cblViewable, cblReadOnly);
        if (!status.Status)
        {
            plistStatus.AddInputParameter("ERROR_DS_ROLES", "Please select a valid decision state changeable by value, roles must match checklist roles!");
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }


        return status;
    }

    /// <summary>
    /// override
    /// calls the validate methods for the checklist and items
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = null;
        CStatus status = ValidateChecklist(out plistStatus);
        if (status.Status)
        {
            status = ucItemEntry.ValidateUserInput(out plistStatus);
        }

        return status;
    }

    /// <summary>
    /// event
    /// US:1945 US:1880 displays the map note title dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickMap(object sender, EventArgs e)
    {
        CStatus status = ucMapNoteTitle.LoadControl(k_EDIT_MODE.INITIALIZE);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        ucMapNoteTitle.ShowMPE();
    }

    /// <summary>
    /// event
    /// US:1945 US:1880 displays the selected note title text
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelectNoteTitle(object sender, CAppUserControlArgs e)
    {
        txtCLNoteTitle.Text = ucMapNoteTitle.SelectedText;
    }

    //constants
    private const int nIndexAdministrator = 0;
    private const int nIndexDoctor = 0;
    private const int nIndexNurse = 0;

    /// <summary>
    /// if the user unchecks the viewable option then they cant do anything
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cblViewable_SelectedIndexChanged(object sender, EventArgs e)
    {
        CCheckBoxList cbl = new CCheckBoxList();
        int nIndex = cbl.GetIndexJustSelected(Request, cblViewable);
        if (nIndex != -1)
        {
            //0 = administrator 
            //1 = doctor 
            //2 = nurse 
            //
            
            //not viewable cant do anything
            if (!cblViewable.Items[nIndex].Selected)
            {
                cblReadOnly.Items[nIndex].Selected = false;
                cblCloseable.Items[nIndex].Selected = false;
                cblTIUNote.Items[nIndex].Selected = false;
            }
        }
    }
    
    /// <summary>
    /// if the user picks the ReadOnly option then they can't close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cblReadOnly_SelectedIndexChanged(object sender, EventArgs e)
    {
        CCheckBoxList cbl = new CCheckBoxList();
        int nIndex = cbl.GetIndexJustSelected(Request, cblReadOnly);
        if (nIndex != -1)
        {
            //0 = administrator
            //1 = doctor
            //2 = nurse
            //
            //read only can't close it or write a note...
            //
            if (cblReadOnly.Items[nIndex].Selected)
            {
                cblCloseable.Items[nIndex].Selected = false;
                cblTIUNote.Items[nIndex].Selected = false;
                //
                //read only viewable is true
                cblViewable.Items[nIndex].Selected = true;
            }
        }
    }

    /// <summary>
    /// Click closeable uncheck read only
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cblCloseable_SelectedIndexChanged(object sender, EventArgs e)
    {
        CCheckBoxList cbl = new CCheckBoxList();
        int nIndex = cbl.GetIndexJustSelected(Request, cblCloseable);
        if (nIndex != -1)
        {
            //0 = administrator
            //1 = doctor
            //2 = nurse
            //
            if (cblCloseable.Items[nIndex].Selected)
            {
                cblReadOnly.Items[nIndex].Selected = false;
                cblViewable.Items[nIndex].Selected = true;
            }
        }
    }

    /// <summary>
    /// click tiu note uncheck read only
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cblTIUNote_SelectedIndexChanged(object sender, EventArgs e)
    {
        CCheckBoxList cbl = new CCheckBoxList();
        int nIndex = cbl.GetIndexJustSelected(Request, cblTIUNote);
        if (nIndex != -1)
        {
            //0 = administrator
            //1 = doctor
            //2 = nurse
            //
            if (cblTIUNote.Items[nIndex].Selected)
            {
                cblReadOnly.Items[nIndex].Selected = false;
                cblViewable.Items[nIndex].Selected = true;
            }
        }
    }

    protected void OnClickCollapseFields(object sender, EventArgs e)
    {
        pnlChecklistFields.Visible = !pnlChecklistFields.Visible;
        if (pnlChecklistFields.Visible)
        {
            btnCollapseFields.Text = "-";
            ucItemEntry.Height = 137;
        }
        else
        {
            btnCollapseFields.Text = "+";
            ucItemEntry.Height = 445;
        }
    }
}
