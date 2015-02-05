using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.Data;

public partial class ce_ucLogicSelector : CAppUserControl
{
    // multi view/radio button indecies
    private const int k_OPERATOR_INDEX = 0;
    private const int k_PATIENT_INDEX = 1;
    private const int k_PAT_ITEM_INDEX = 2;
    private const int k_STATIC_INDEX = 3;
    private const int k_ACTION_INDEX = 4;

    // stat place holder ids
    private const int k_BOOLEAN = 110;
    private const int k_ARITHMETIC = 130;
    private const int k_ITEM = 400;
    private const int k_ITEM_COMPONENT = 450;
    private const int k_IC_RANGE = 460;
    private const int k_IC_STATE = 480;
    private const int k_SET = 730;
    private const int k_TEMPORAL_STATE = 731;
    private const int k_OUTCOME_STATE = 732;
    private const int k_DECISION_STATE = 733;

    /// <summary>
    /// property
    /// stores a checklist id in session
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
    /// stores a checklist item id in session
    /// retrieves a checklist item id from session
    /// </summary>
    public long ChecklistItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ChecklistItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ChecklistItemID"] = value; }
    }

    /// <summary>
    /// property
    /// US:902
    /// gets/sets the text box that the user control will add logic to
    /// </summary>
    public TextBox LogicAddTarget { get; set; }

    /// <summary>
    /// event
    /// US:902
    /// loads the multi view for the selected catagory
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedCats(object sender, EventArgs e)
    {
        ShowMPE();
        mvLogic.ActiveViewIndex = rblLogic.SelectedIndex;
        CStatus status = null;
        switch (rblLogic.SelectedIndex)
        {
            case k_OPERATOR_INDEX :
                status = LoadOperatorView();
                break;
            case k_PATIENT_INDEX:
                status = LoadPatientView();
                break;
            case k_STATIC_INDEX:
                status = LoadStaticView();
                break;
            case k_PAT_ITEM_INDEX:
                status = LoadItemView();
                break;
            case k_ACTION_INDEX:
                status = LoadActionView();
                break;
            default:
                status = new CStatus(false, k_STATUS_CODE.Failed, "TODO");
                break;
        }

        if (!status.Status)
        {
            ShowStatusInfo(status);
        }

        string strScript = string.Format("document.getElementById('{0}_{1}').focus();", rblLogic.ClientID, rblLogic.SelectedIndex);

        if (ScriptManager.GetCurrent(Page) != null && ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            ScriptManager.RegisterStartupScript(rblLogic, typeof(RadioButtonList), rblLogic.ClientID, strScript, true);
        else
            Page.ClientScript.RegisterStartupScript(typeof(RadioButtonList), rblLogic.ClientID, strScript, true);
    }

    /// <summary>
    /// method
    /// US:902
    /// loads the operator view
    /// </summary>
    protected CStatus LoadOperatorView()
    {
        lbOperator.Items.Clear();
        lbOperatorSubGroup.Items.Clear();
        lblOperatorSubGroup.Visible = false;
        lbOperatorSubGroup.Visible = false;

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = PHData.GetPlaceHolderChildDS(Convert.ToInt64(rblLogic.SelectedValue), out ds);
        if (!status.Status)
        {
            return status;
        }

        lbOperator.DataTextField = "PLACE_HOLDER_LABEL";
        lbOperator.DataValueField = "PLACE_HOLDER_ID";
        lbOperator.DataSource = ds;
        lbOperator.DataBind();
        return new CStatus();
    } 

    /// <summary>
    /// method
    /// US:902
    /// loads the patient view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected CStatus LoadPatientView()
    {
        lbPatient.Items.Clear();

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = PHData.GetPlaceHolderChildDS(Convert.ToInt64(rblLogic.SelectedValue), out ds);
        if (!status.Status)
        {
            return status;
        }

        lbPatient.DataTextField = "PLACE_HOLDER_LABEL";
        lbPatient.DataValueField = "PLACE_HOLDER_ID";
        lbPatient.DataSource = ds;
        lbPatient.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:902
    /// loads the static view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected CStatus LoadStaticView()
    {
        lbStatic.Items.Clear();
        lbStaticSpecifier.Items.Clear();
        lblStaticSpecifier.Visible = false;
        lbStaticSpecifier.Visible = false;

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = PHData.GetPlaceHolderChildDS(Convert.ToInt64(rblLogic.SelectedValue), out ds);
        if (!status.Status)
        {
            return status;
        }

        lbStatic.DataTextField = "PLACE_HOLDER_LABEL";
        lbStatic.DataValueField = "PLACE_HOLDER_ID";
        lbStatic.DataSource = ds;
        lbStatic.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:902
    /// loads the item view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected CStatus LoadItemView()
    {
        lbItem.Items.Clear();
        lbItemPH.Items.Clear();
        lbItemComponent.Items.Clear();
        lbItemCompPH.Items.Clear();
        lblItemPH.Visible = false;
        lbItemPH.Visible = false;
        lblItemComponent.Visible = false;
        lbItemComponent.Visible = false;
        lblItemCompPH.Visible = false;
        lbItemCompPH.Visible = false;

        CItemData IData = new CItemData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = IData.GetItemDS(string.Empty, -1, -1, (long)k_ACTIVE_ID.All, ChecklistID, out ds);
        if (!status.Status)
        {
            return status;
        }

        lbItem.DataTextField = "ITEM_LABEL";
        lbItem.DataValueField = "ITEM_ID";
        lbItem.DataSource = ds;
        lbItem.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:902
    /// loads the action view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected CStatus LoadActionView()
    {
        lbAction.Items.Clear();
        lbActionSpecifier.Items.Clear();
        lbActionValue.Items.Clear();
        lblActionSpecifier.Visible = false;
        lbActionSpecifier.Visible = false;
        lblActionValue.Visible = false;
        lbActionValue.Visible = false;

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = PHData.GetPlaceHolderChildDS(Convert.ToInt64(rblLogic.SelectedValue), out ds);
        if (!status.Status)
        {
            return status;
        }

        lbAction.DataTextField = "PLACE_HOLDER_LABEL";
        lbAction.DataValueField = "PLACE_HOLDER_ID";
        lbAction.DataSource = ds;
        lbAction.DataBind();
        return new CStatus();
    }

    /// <summary>
    /// event
    /// US:902
    /// loads the static specifier list box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedStatic(object sender, EventArgs e)
    {
        ShowMPE();
        lbStaticSpecifier.Items.Clear();

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = PHData.GetPlaceHolderChildDS(Convert.ToInt64(lbStatic.SelectedValue), out ds);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (ds.Tables[0].Rows.Count > 0)
        {
            lbStaticSpecifier.DataTextField = "PLACE_HOLDER_LABEL";
            lbStaticSpecifier.DataValueField = "PLACE_HOLDER_ID";
            lbStaticSpecifier.DataSource = ds;
            lbStaticSpecifier.DataBind();
            lblStaticSpecifier.Visible = true;
            lbStaticSpecifier.Visible = true;
        }
        else
        {
            lblStaticSpecifier.Visible = false;
            lbStaticSpecifier.Visible = false;
        }
    }

    /// <summary>
    /// event
    /// US:902
    /// loads the item PH and item component list boxes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedItem(object sender, EventArgs e)
    {
        ShowMPE();
        lbItemPH.Items.Clear();
        lbItemComponent.Items.Clear();
        lbItemCompPH.Items.Clear();
        lblItemCompPH.Visible = false;
        lbItemCompPH.Visible = false;

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet dsItem = null;
        CStatus status = PHData.GetPlaceHolderChildDS(k_ITEM, out dsItem);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (dsItem.Tables[0].Rows.Count > 0)
        {
            lbItemPH.DataTextField = "PLACE_HOLDER_LABEL";
            lbItemPH.DataValueField = "PLACE_HOLDER_ID";
            lbItemPH.DataSource = dsItem;
            lbItemPH.DataBind();
            lblItemPH.Visible = true;
            lbItemPH.Visible = true;
        }
        else
        {
            lblItemPH.Visible = false;
            lbItemPH.Visible = false;
        }

        CItemComponentData ICData = new CItemComponentData(BaseMstr.BaseData);
        DataSet dsItemComp = null;
        status = ICData.GetItemComponentDS(Convert.ToInt64(lbItem.SelectedValue), k_ACTIVE_ID.Active, out dsItemComp);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (dsItemComp.Tables[0].Rows.Count > 0)
        {
            lbItemComponent.DataTextField = "ITEM_COMPONENT_LABEL";
            lbItemComponent.DataValueField = "ITEM_COMPONENT_ID";
            lbItemComponent.DataSource = dsItemComp;
            lbItemComponent.DataBind();
            lblItemComponent.Visible = true;
            lbItemComponent.Visible = true;
        }
        else
        {
            lblItemComponent.Visible = false;
            lbItemComponent.Visible = false;
        }
    }

    /// <summary>
    /// event
    /// US:902
    /// clears the selections from the item component list boxes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedItemPH(object sender, EventArgs e)
    {
        ShowMPE();
        lbItemComponent.ClearSelection();
        lbItemCompPH.Items.Clear();
        lblItemCompPH.Visible = false;
        lbItemCompPH.Visible = false;
    }

    /// <summary>
    /// event
    /// US:902
    /// loads the item component PH list box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedItemComp(object sender, EventArgs e)
    {
        ShowMPE();
        lbItemPH.ClearSelection();
        lbItemCompPH.Items.Clear();

        // bind standard options
        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;

        CStatus status = PHData.GetPlaceHolderChildDS(k_ITEM_COMPONENT, out ds);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (ds.Tables[0].Rows.Count > 0)
        {
            lbItemCompPH.DataTextField = "PLACE_HOLDER_LABEL";
            lbItemCompPH.DataValueField = "PLACE_HOLDER_ID";
            lbItemCompPH.DataSource = ds;
            lbItemCompPH.DataBind();
            lblItemCompPH.Visible = true;
            lbItemCompPH.Visible = true;
        }
        else
        {
            lblItemCompPH.Visible = false;
            lbItemCompPH.Visible = false;
        }

        // bind selection specific options
        CItemData IData = new CItemData(BaseMstr.BaseData);
        CItemDataItem di = null;
        long lItemID = Convert.ToInt64(lbItem.SelectedValue);
        status = IData.GetItemDI(lItemID, out di);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (di.ItemID != lItemID)
        {
            status.Status = false;
            status.StatusCode = k_STATUS_CODE.Failed;
            status.StatusComment = "TODO";
            ShowStatusInfo(status);
            return;
        }

        long lPlaceHolderID = -1;
        switch (di.ItemTypeID)
        {
            case (long)k_ITEM_TYPE_ID.Laboratory:
                lPlaceHolderID = k_IC_RANGE;
                break;
            case (long)k_ITEM_TYPE_ID.QuestionSelection:
                lPlaceHolderID = k_IC_STATE;
                break;
            case (long)k_ITEM_TYPE_ID.QuestionFreeText:
                // there are not any options specific to question free text
                return;
        }

        status = PHData.GetPlaceHolderChildDS(lPlaceHolderID, out ds);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        lbItemCompPH.DataSource = ds;
        lbItemCompPH.DataBind();
    }

    /// <summary>
    /// event
    /// US:902
    /// Loads the operator sub groups
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedOperator(object sender, EventArgs e)
    {
        ShowMPE();
        lbOperatorSubGroup.Items.Clear();

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = PHData.GetPlaceHolderChildDS(Convert.ToInt64(lbOperator.SelectedValue), out ds);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (ds.Tables[0].Rows.Count > 0)
        {
            lbOperatorSubGroup.DataTextField = "PLACE_HOLDER_LABEL";
            lbOperatorSubGroup.DataValueField = "PLACE_HOLDER_ID";
            lbOperatorSubGroup.DataSource = ds;
            lbOperatorSubGroup.DataBind();
            lblOperatorSubGroup.Visible = true;
            lbOperatorSubGroup.Visible = true;
        }
        else
        {
            lblOperatorSubGroup.Visible = false;
            lbOperatorSubGroup.Visible = false;
        }
    }

    /// <summary>
    /// event
    /// US:902
    /// loads the action set list box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedAction(object sender, EventArgs e)
    {
        ShowMPE();
        lbActionSpecifier.Items.Clear();
        lbActionValue.Items.Clear();
        lblActionValue.Visible = false;
        lbActionValue.Visible = false;

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        long lStatPlaceHolderID = Convert.ToInt64(lbAction.SelectedValue);
        CStatus status = PHData.GetPlaceHolderChildDS(lStatPlaceHolderID, out ds);

        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (ds.Tables[0].Rows.Count > 0)
        {
            lbActionSpecifier.DataTextField = "PLACE_HOLDER_LABEL";
            lbActionSpecifier.DataValueField = "PLACE_HOLDER_ID";
            lbActionSpecifier.DataSource = ds;
            lbActionSpecifier.DataBind();

            if (lStatPlaceHolderID == k_SET)
            {
                lblActionSpecifier.AccessKey = "S";
                lblActionSpecifier.Text = "<span class=access_key>S</span>pecifier(s)";
            }
            else
            {
                lblActionSpecifier.AccessKey = "P";
                lblActionSpecifier.Text = "Action <span class=access_key>P</span>lace Holder(s)";
            }

            lblActionSpecifier.Visible = true;
            lbActionSpecifier.Visible = true;
        }
        else
        {
            lblActionSpecifier.Visible = false;
            lbActionSpecifier.Visible = false;
        }
    }

    /// <summary>
    /// event
    /// US:902
    /// loads the action set state list box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelIndexChangedActionSpecifier(object sender, EventArgs e)
    {
        ShowMPE();
        lbActionValue.Items.Clear();

        long lSpecifierValue = Convert.ToInt64(lbActionSpecifier.SelectedValue);
        if (lSpecifierValue != k_TEMPORAL_STATE
            && lSpecifierValue != k_OUTCOME_STATE
            && lSpecifierValue != k_DECISION_STATE)
        {
            return;
        }

        //get the cli data and check the checkboxes 
        CChecklistItemData CLIData = new CChecklistItemData(BaseMstr.BaseData);
        DataSet ds = null;
        string strDataTextField = string.Empty;
        string strDataValueField = string.Empty;
        CStatus status = null;

        switch (lSpecifierValue)
        {
            case k_TEMPORAL_STATE:
                status = CLIData.GetTemporalStateDS(ChecklistID, ChecklistItemID, out ds);
                strDataTextField = "TS_LABEL";
                strDataValueField = "TS_ID";
                break;
            case k_OUTCOME_STATE:
                status = CLIData.GetOutcomeStateDS(ChecklistID, ChecklistItemID, out ds);
                strDataTextField = "OS_LABEL";
                strDataValueField = "OS_ID";
                break;
            case k_DECISION_STATE:
                status = CLIData.GetDecisionStateDS(ChecklistID, ChecklistItemID, out ds);
                strDataTextField = "DS_LABEL";
                strDataValueField = "DS_ID";
                break;
        }

        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        if (ds.Tables[0].Rows.Count > 0)
        {
            lbActionValue.DataTextField = strDataTextField;
            lbActionValue.DataValueField = strDataValueField;
            lbActionValue.DataSource = ds;
            lbActionValue.DataBind();
            lblActionValue.Visible = true;
            lbActionValue.Visible = true;
        }
        else
        {
            lblActionValue.Visible = false;
            lbActionValue.Visible = false;
        }
    }

    /// <summary>
    /// method
    /// US:874 US:902
    /// gets the syntax for the operator place holder
    /// </summary>
    /// <returns></returns>
    protected string GetOperatorSyntax()
    {
        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        CPlaceHolderDataItem di = null;
        string strSyntax = string.Empty;
        if (lbOperatorSubGroup.SelectedIndex >= 0)
        {
            PHData.GetPlaceHolderDI(Convert.ToInt64(lbOperatorSubGroup.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax = di.PlaceHolderSyntax;
        }
        else if(lbOperator.SelectedIndex >= 0
            && Convert.ToInt32(lbOperator.SelectedValue) != k_BOOLEAN
            && Convert.ToInt32(lbOperator.SelectedValue) != k_ARITHMETIC)
        {
            PHData.GetPlaceHolderDI(Convert.ToInt64(lbOperator.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax = di.PlaceHolderSyntax;
        }

        return strSyntax;
    }

    /// <summary>
    /// method
    /// US:874 US:902
    /// gets the syntax for the patient place holder
    /// </summary>
    /// <returns></returns>
    protected string GetPatientSyntax()
    {
        if (rblLogic.SelectedIndex < 0 || lbPatient.SelectedIndex < 0)
        {
            return string.Empty;
        }

        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        CPlaceHolderDataItem di = null;
        CStatus status = PHData.GetPlaceHolderDI(Convert.ToInt64(rblLogic.SelectedValue), out di);
        if (di == null)
        {
            return string.Empty;
        }

        string strSyntax = di.PlaceHolderSyntax;

        status = PHData.GetPlaceHolderDI(Convert.ToInt64(lbPatient.SelectedValue), out di);
        if (di == null)
        {
            return string.Empty;
        }
        strSyntax += CExpression.SpecifierTkn + di.PlaceHolderSyntax;
        return CExpression.BeginPHTkn + strSyntax + CExpression.EndPHTkn;
    }

    /// <summary>
    /// method
    /// US:874 US:902
    /// gets the syntax for the static place holder
    /// </summary>
    /// <returns></returns>
    protected string GetStaticSyntax()
    {
        if (lbStaticSpecifier.SelectedIndex < 0)
        {
            return string.Empty;
        }

        string strSyntax = string.Empty;
        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        CPlaceHolderDataItem di = null;
        CStatus status = PHData.GetPlaceHolderDI(Convert.ToInt64(lbStatic.SelectedValue), out di);
        if (di == null)
        {
            return string.Empty;
        }
        strSyntax = di.PlaceHolderSyntax;

        status = PHData.GetPlaceHolderDI(Convert.ToInt64(lbStaticSpecifier.SelectedValue), out di);
        if (di == null)
        {
            return string.Empty;
        }
        strSyntax += CExpression.SpecifierTkn + di.PlaceHolderSyntax;
        return CExpression.BeginPHTkn + strSyntax + CExpression.EndPHTkn;
    }

    /// <summary>
    /// method
    /// US:874 US:902
    /// gets the syntax for the item place holder
    /// </summary>
    /// <returns></returns>
    protected string GetItemSyntax()
    {
        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        CPlaceHolderDataItem di = null;
        CStatus status = new CStatus();
        string strSyntax = string.Empty;
        if (lbItemPH.SelectedIndex >= 0 && lbItem.SelectedIndex >= 0)
        {
            status = PHData.GetPlaceHolderDI(Convert.ToInt64(lbItemPH.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax = lbItem.SelectedItem.Text + CExpression.SpecifierTkn + di.PlaceHolderSyntax;
            strSyntax = CExpression.BeginPHTkn + strSyntax + CExpression.EndPHTkn;
        }
        else if (lbItemCompPH.SelectedIndex >= 0
            && lbItemComponent.SelectedIndex >= 0
            && lbItem.SelectedIndex >= 0)
        {
            status = PHData.GetPlaceHolderDI(Convert.ToInt64(lbItemCompPH.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax = lbItem.SelectedItem.Text + CExpression.SpecifierTkn + lbItemComponent.SelectedItem.Text + CExpression.SpecifierTkn + di.PlaceHolderSyntax;
            strSyntax = CExpression.BeginPHTkn + strSyntax + CExpression.EndPHTkn;
        }
        return strSyntax;
    }

    /// <summary>
    /// method
    /// US:874 US:902
    /// gets the syntax for the action place holder
    /// </summary>
    /// <returns></returns>
    protected string GetActionSyntax()
    {
        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        CPlaceHolderDataItem di = null;
        string strSyntax = string.Empty;
        if (lbActionValue.SelectedIndex >= 0
            && lbActionSpecifier.SelectedIndex >= 0
            && lbAction.SelectedIndex >= 0)
        {
            PHData.GetPlaceHolderDI(Convert.ToInt64(lbActionSpecifier.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax = di.PlaceHolderSyntax;

            PHData.GetPlaceHolderDI(Convert.ToInt64(lbAction.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax += CExpression.SpecifierTkn + di.PlaceHolderSyntax;
            strSyntax += CExpression.ParamStartTkn + lbActionValue.SelectedItem.Text + CExpression.ParamEndTkn;
        }
        else if (lbActionSpecifier.SelectedIndex >= 0
            && Convert.ToInt32(lbActionSpecifier.SelectedValue) != k_TEMPORAL_STATE
            && Convert.ToInt32(lbActionSpecifier.SelectedValue) != k_OUTCOME_STATE
            && Convert.ToInt32(lbActionSpecifier.SelectedValue) != k_DECISION_STATE
            && lbAction.SelectedIndex >= 0)
        {
            PHData.GetPlaceHolderDI(Convert.ToInt64(lbActionSpecifier.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax = di.PlaceHolderSyntax;

            PHData.GetPlaceHolderDI(Convert.ToInt64(lbAction.SelectedValue), out di);
            if (di == null)
            {
                return string.Empty;
            }
            strSyntax += CExpression.SpecifierTkn + di.PlaceHolderSyntax + CExpression.ParamStartTkn + CExpression.ParamEndTkn;
        }
        return strSyntax;
    }

    /// <summary>
    /// method
    /// US:874 US:902
    /// gets the selected syntax from the current view
    /// </summary>
    /// <returns></returns>
    protected string GetSelectedSyntax()
    {
        switch (mvLogic.ActiveViewIndex)
        {
            case k_OPERATOR_INDEX:
                return GetOperatorSyntax();
            case k_PATIENT_INDEX:
                return GetPatientSyntax();
            case k_STATIC_INDEX:
                return GetStaticSyntax();
            case k_PAT_ITEM_INDEX:
                return GetItemSyntax();
            case k_ACTION_INDEX:
                return GetActionSyntax();
        }
        return string.Empty;
    }

    /// <summary>
    /// event
    /// US:874 US:902
    /// adds the syntax for the selected list box item to the associated text box control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickAdd(object sender, EventArgs e)
    {
        ShowMPE();
        string strSyntax = GetSelectedSyntax();
        if (string.IsNullOrEmpty(strSyntax))
        {
            return;
        }

        LogicAddTarget.Text = LogicAddTarget.Text.Trim();
        if (!string.IsNullOrEmpty(LogicAddTarget.Text))
        {
            if (LogicAddTarget.Text.EndsWith(CExpression.EndTkn.ToString()))
            {
                LogicAddTarget.Text += "\r\n";
            }
            else if (strSyntax != CExpression.EndTkn.ToString())
            {
                LogicAddTarget.Text += " ";
            }
        }

        LogicAddTarget.Text += strSyntax;
        LogicAddTarget.Text += (strSyntax == CExpression.EndTkn.ToString()) ? "\r\n" : " ";
    }

    /// <summary>
    /// method
    /// US:902
    /// loads the logic categories into the radio button list
    /// </summary>
    /// <returns></returns>
    protected CStatus LoadLogicCategories()
    {
        CPlaceHolderData PHData = new CPlaceHolderData(BaseMstr.BaseData);
        DataSet ds = null;
        CStatus status = PHData.GetPlaceHolderParentDS(out ds);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return status;
        }

        rblLogic.DataTextField = "PLACE_HOLDER_LABEL";
        rblLogic.DataValueField = "PLACE_HOLDER_ID";
        rblLogic.DataSource = ds;
        rblLogic.DataBind();
        return status;
    }

    /// <summary>
    /// override
    /// US:902
    /// initializes the control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;
        CStatus status = LoadLogicCategories();
        if (!status.Status)
        {
            return status;
        }

        rblLogic.SelectedIndex = k_OPERATOR_INDEX;

        status = LoadOperatorView();
        if (!status.Status)
        {
            return status;
        }
        
        mvLogic.ActiveViewIndex = k_OPERATOR_INDEX;

        return status;
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }
}
