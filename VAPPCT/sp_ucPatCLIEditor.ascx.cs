using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucPatCLIEditor : CAppUserControlPopup 
{
    /// <summary>
    /// US:912
    /// property to get/set the patient id
    /// </summary>
    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID.ToString() + "PatientID"];
            return (obj != null) ? obj.ToString() : "-1";
        }
        set { ViewState[ClientID.ToString() + "PatientID"] = value; }
    }

    /// <summary>
    /// US:912
    /// property
    /// gets/sets a checklist id for the page
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
    /// US:912
    /// property
    /// gets/sets a checklist id for the page
    /// </summary>
    public long PatientChecklistID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientChecklistID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "PatientChecklistID"] = value; }
    }

    /// <summary>
    /// US:912
    /// property
    /// gets/sets a checklist item id
    /// </summary>
    public long ItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ItemID"] = value; }
    }

    /// <summary>
    /// US:912
    /// select the current ts,os and ds states
    /// </summary>
    /// <returns></returns>
    public CStatus LoadStates()
    {
        CPatChecklistItemDataItem di = null;
        CPatChecklistItemData cid = new CPatChecklistItemData(BaseMstr.BaseData);
        CStatus status = cid.GetPatCLItemDI(PatientChecklistID, ItemID, out di);
        if (!status.Status)
        {
            return status;
        }

        //temporal state
        CTemporalStateDataItem diTSi = new CTemporalStateDataItem();
        CTemporalStateData tsdi = new CTemporalStateData(BaseMstr.BaseData);
        status = tsdi.GetTemporalStateDI(di.TSID, out diTSi);
        if (!status.Status)
        {
            return status;
        }
        lblCurrentTS.Text = diTSi.TSLabel;

        //outcome state
        COutcomeStateDataItem diOSi = new COutcomeStateDataItem();
        COutcomeStateData osdi = new COutcomeStateData(BaseMstr.BaseData);
        status = osdi.GetOutcomeStateDI(di.OSID, out diOSi);
        if (!status.Status)
        {
            return status;
        }
        lblCurrentOS.Text = diOSi.OSLabel;

        //DS ddl select
        CDropDownList.SelectItemByValue(ddlDS, di.DSID);

        return new CStatus();
    }
    
    /// <summary>
    /// US:912
    /// page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucCollection.BaseMstr = BaseMstr;
        ucNoteTitle.BaseMstr = BaseMstr;

        if (!IsPostBack)
        {
            Title = "Override Patient Checklist Item";
        }
    }

    /// <summary>
    /// US:912
    /// load the comments history gridview
    /// </summary>
    public CStatus LoadCommentsGridView()
    {
        //itemdata for db work
        DataSet dsComments = null;
        CPatChecklistItemData item = new CPatChecklistItemData(BaseMstr.BaseData);
        CStatus status = item.GetPatItemOverrideCommmentDS(PatientChecklistID, ChecklistID, ItemID, out dsComments);
        if (!status.Status)
        {
            return status;
        }

        gvComments.DataSource = dsComments;
        gvComments.DataBind();

        return new CStatus();
    }

    /// <summary>
    /// US:1880 gets the most recent result, loops over all components and pieces together a string
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="pidi"></param>
    /// <param name="strResult"></param>
    /// <returns></returns>
    public CStatus GetMostRecentResult(out string strResult)
    {
        strResult = string.Empty;

        //load the last result for this item
        CPatientItemData pid = new CPatientItemData(BaseMstr.BaseData);
        CPatientItemDataItem pidi = new CPatientItemDataItem();
        CStatus status = pid.GetMostRecentPatientItemDI(PatientID, ItemID, out pidi);
        if (!status.Status)
        {
            return status;
        }

        if (pidi.PatItemID < 1)
        {
            return new CStatus();
        }

        DataSet dsComps = null;
        status = pid.GetPatientItemComponentDS(
            PatientID,
            pidi.PatItemID,
            pidi.ItemID,
            out dsComps);
        if (!status.Status)
        {
            return status;
        }

        foreach (DataRow row in dsComps.Tables[0].Rows)
        {
            string strLabel = CDataUtils.GetDSStringValue(row, "item_component_label");
            string strValue = CDataUtils.GetDSStringValue(row, "component_value");
            string strUnits = CDataUtils.GetDSStringValue(row, "units");

            string strWarning = String.Empty;

            //question selection
            if (pidi.ItemTypeID == (long)k_ITEM_TYPE_ID.QuestionSelection)
            {
                if (strValue == "1")
                {
                    strResult += "<font face=\"verdana,arial\" size=\"-1\">";
                    strResult += strLabel;
                    strResult += "<br /></font>";
                }
            }

            //question free text
            else if (pidi.ItemTypeID == (long)k_ITEM_TYPE_ID.QuestionFreeText)
            {
                strResult += "<font face=\"verdana,arial\" size=\"-1\">";
                strResult += strLabel;
                strResult += ": ";
                strResult += strValue;
                strResult += "<br /></font>";
            }

            //lab
            else if (pidi.ItemTypeID == (long)k_ITEM_TYPE_ID.Laboratory)
            {
                if (CDataUtils.IsNumeric(strValue))
                {
                    double dblLegalMin = CDataUtils.GetDSDoubleValue(row, "LEGAL_MIN");
                    double dblLow = CDataUtils.GetDSDoubleValue(row, "LOW");
                    double dblCritialLow = CDataUtils.GetDSDoubleValue(row, "CRITICAL_LOW");
                    double dblHigh = CDataUtils.GetDSDoubleValue(row, "HIGH");
                    double dblCriticalHigh = CDataUtils.GetDSDoubleValue(row, "CRITICAL_HIGH");
                    double dblLegalMax = CDataUtils.GetDSDoubleValue(row, "LEGAL_MAX");

                    double dblValue = Convert.ToDouble(strValue);
                    if (dblValue < dblLegalMin)
                    {
                        strWarning = "LESS THAN LEGAL MIN";
                    }
                    else if (dblValue < dblCritialLow)
                    {
                        strWarning = "CRITICAL LOW";
                    }
                    else if (dblValue < dblLow)
                    {
                        strWarning = "LOW";
                    }
                    else if (dblValue > dblLegalMax)
                    {
                        strWarning = "GREATER THAN LEGAL MAX";
                    }
                    else if (dblValue > dblCriticalHigh)
                    {
                        strWarning = "CRITICAL HIGH";
                    }
                    else if (dblValue > dblHigh)
                    {
                        strWarning = "HIGH";
                    }
                }

                strResult += "<font face=\"verdana,arial\" size=\"-1\">";
                strResult += strLabel;
                strResult += ": ";
                strResult += strValue;
                strResult += " ";
                strResult += strUnits;
                strResult += " ";
                strResult += strWarning;
                strResult += "<br /></font>";

                string strLegalMin = CDataUtils.GetDSStringValue(row, "LEGAL_MIN");
                string strLow = CDataUtils.GetDSStringValue(row, "LOW");
                string strCritialLow = CDataUtils.GetDSStringValue(row, "CRITICAL_LOW");
                string strHigh = CDataUtils.GetDSStringValue(row, "HIGH");
                string strCriticalHigh = CDataUtils.GetDSStringValue(row, "CRITICAL_HIGH");
                string strLegalMax = CDataUtils.GetDSStringValue(row, "LEGAL_MAX");

                strResult += "<font face=\"verdana,arial\" size=\"-2\">";
                strResult += "(Legal Min: " + strLegalMin + " ";
                strResult += "Low: " + strLow + " ";
                strResult += "Critical Low: " + strCritialLow + " ";
                strResult += "High: " + strHigh + " ";
                strResult += "Critical High: " + strCriticalHigh + " ";
                strResult += "Legal Max: " + strLegalMax + ") ";
                strResult += "<br /><br /></font>";
            }
        }
       
        return new CStatus();
    }
        
    /// <summary>
    /// US:912
    /// load the last result
    /// </summary>
    protected CStatus LoadLastResult()
    {
        pnlLastResult.Visible = false;
        ucCollection.Visible = false;
        ucNoteTitle.Visible = false;

        //get the raw item data for this item
        CItemDataItem idi = new CItemDataItem();
        CItemData id = new CItemData(BaseMstr.BaseData);
        CStatus status = id.GetItemDI(ItemID, out idi);
        if (!status.Status)
        {
            return status;
        }

        //note title
        if (idi.ItemTypeID == (long)k_ITEM_TYPE_ID.NoteTitle)
        {
            ucNoteTitle.Visible = true;
            ucNoteTitle.ItemID = ItemID;
            ucNoteTitle.PatientID = PatientID;
            status = ucNoteTitle.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                return status;
            }
        }
        else if (idi.ItemTypeID == (long)k_ITEM_TYPE_ID.Collection)
        {
            ucCollection.Visible = true;
            ucCollection.CollectionItemID = ItemID;
            ucCollection.PatientID = PatientID;
            status = ucCollection.LoadControl(k_EDIT_MODE.INITIALIZE);
            if (!status.Status)
            {
                return status;
            }
        }
        else
        {
            pnlLastResult.Visible = true;

            //get the most recent result for this checklist item
            string strResult = string.Empty;
            status = GetMostRecentResult(out strResult);
            if (!status.Status)
            {
                return status;
            }

            //display the result
            litLastResult.Text = strResult;
        }

        return new CStatus();
    }
    
    /// <summary>
    /// US:912
    /// load control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        ((app_ucTimer)UCTimer).StopRefresh();

        //load the item and description
        CItemData Item = new CItemData(BaseMstr.BaseData);
        CItemDataItem di = null;
        CStatus status = Item.GetItemDI(ItemID, out di);
        if(!status.Status)
        {
            return status;
        }

        lblItem.Text = di.ItemLabel;
        lblItemDescription.Text = di.ItemDescription;

        status = LoadLastResult();
        if (!status.Status)
        {
            return status;
        }

        //default the date to today
        DateTime dtNow = DateTime.Now;

        //load the dropdown lists
        status = CDecisionState.LoadCLIDecisionStatesDDL(
            BaseMstr.BaseData,
            ChecklistID,
            ItemID,
            ddlDS);
        if (!status.Status)
        {
            return status;
        }

        //load/select TS,OS,DS
        status = LoadStates();
        if (!status.Status)
        {
            return status;
        }

        //load the comments gridview
        status = LoadCommentsGridView();
        if (!status.Status)
        {
            return status;
        }

        //clear the comment data entry
        txtComment.Text = string.Empty;

        return new CStatus();
    }

     /// <summary>
    /// US:912
    /// Abstract save control method
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        //update the checklist item states...
        CPatChecklistItemDataItem diCLItem = null;
        CPatChecklistItemData cid = new CPatChecklistItemData(BaseMstr.BaseData);
        CStatus status = cid.GetPatCLItemDI(PatientChecklistID, ItemID, out diCLItem);
        if (!status.Status)
        {
            return status;
        }

        //per customer we only override decision state
        if(diCLItem.DSID != Convert.ToInt64(ddlDS.SelectedValue))
        {
            diCLItem.IsOverridden = k_TRUE_FALSE_ID.True;
            diCLItem.OverrideDate = DateTime.Now;
        }

        diCLItem.DSID = CDropDownList.GetSelectedLongID(ddlDS);

        //update
        status = cid.UpdatePatChecklistItem(diCLItem);
        if (!status.Status)
        {
            return status;
        }

        //keep a history of the ds override with comment
        if (diCLItem.IsOverridden == k_TRUE_FALSE_ID.True)
        {
            cid.OverridePatChecklistItem(diCLItem, txtComment.Text);
        }

        //TODO: this is obsolete, will delete after testing
        //we now keep a history of ds overrides with comment
        //using OverridePatChecklistItem above
        //
        //if (!string.IsNullOrEmpty(txtComment.Text))
        //{
            //CPatChecklistItemData item = new CPatChecklistItemData(BaseMstr.BaseData);
            //status = item.InsertPatientItemComment(
            //    PatientChecklistID,
           //     ChecklistID,
           //     ItemID,
           //     txtComment.Text);
           // if (!status.Status)
           // {
           //     return status;
           // }
       // }

        return new CStatus();
    }
    
    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        plistStatus = new CParameterList();

        if (string.IsNullOrEmpty(txtComment.Text))
        {
            plistStatus.AddInputParameter("TODO", "A comment is required to override the decision state for this item!");
        }

        CStatus status = new CStatus();

        if (plistStatus.Count > 0)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
        }

        return status;
    }
       
    /// <summary>
    /// US:912
    /// user clicked the ok button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        CParameterList plistStatus = null;

        //validate user input
        CStatus status = ValidateUserInput(out plistStatus);
        if (!status.Status)
        {
            ShowStatusInfo(status.StatusCode, plistStatus);
            ShowMPE();
            return;
        }

        //save the data
        status = SaveControl();
        if (!status.Status)
        {
            ShowMPE();
            return;
        }

        ((app_ucTimer)UCTimer).StartRefresh();
    }

    /// <summary>
    /// US:912
    /// user clicked the cancel button 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ShowParentMPE();
        ((app_ucTimer)UCTimer).StartRefresh();
    }
}
