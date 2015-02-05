using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAPPCT.DA;
using VAPPCT.Data;

public class CExecuteExpression
{
    /// <summary>
    /// property
    /// gets/sets the base master for the instance
    /// </summary>
    public CData BaseData { get; set; }

    /// <summary>
    /// property
    /// gets/sets the patient id for the instance
    /// </summary>
    public string PatientID { get; set; }

    /// <summary>
    /// property
    /// gets/sets the patient checklist id for the instance
    /// </summary>
    public long PatCLID { get; set; }

    /// <summary>
    /// property
    /// gets/sets the checklist id for the instance
    /// </summary>
    public long ChecklistID { get; set; }

    /// <summary>
    /// property
    /// gets/sets the item id for the instance
    /// </summary>
    public long ItemID { get; set; }

    /// <summary>
    /// constructor
    /// initializes the instance
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="strPatientID"></param>
    /// <param name="lPatCLID"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    public CExecuteExpression(CData Data, string strPatientID, long lPatCLID, long lChecklistID, long lItemID)
    {
        BaseData = Data;
        PatientID = strPatientID;
        PatCLID = lPatCLID;
        ChecklistID = lChecklistID;
        ItemID = lItemID;
    }

    /// <summary>
    /// method
    /// US:902
    /// executes the actions in the expression passed in
    /// </summary>
    /// <param name="strExp"></param>
    /// <returns></returns>
    public CStatus Execute(string strExp)
    {
        CParseExpression ParseExp = new CParseExpression(BaseData, PatientID, PatCLID, ChecklistID, ItemID);
        CStringStatus ss = ParseExp.ParseAction(strExp);
        if (!ss.Status)
        {
            return ss;
        }

        switch (ss.Value)
        {
            case "checklist.cancel()":
                return CancelChecklist(PatCLID);
            case "item.disable()":
                return DisableChecklistItem(PatCLID, ItemID);
            default:
                return ParseSpecifiedAction(ss.Value);
        }
    }

    /// <summary>
    /// method
    /// US:911
    /// cancels the checklist specified by the patient checklist id
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <returns></returns>
    private CStatus CancelChecklist(long lPatCLID)
    {
        CPatChecklistData PatChecklist = new CPatChecklistData(BaseData);
        CPatChecklistDataItem di = null;
        CStatus status = PatChecklist.GetPatChecklistDI(lPatCLID, out di);
        if (!status.Status)
        {
            return status;
        }

        di.ChecklistStateID = k_CHECKLIST_STATE_ID.Cancelled;

        return PatChecklist.UpdatePatChecklist(di);
    }

    /// <summary>
    /// method
    /// US:909
    /// disables the patient checklist item specified by the patient checklist it and item id
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    private CStatus DisableChecklistItem(long lPatCLID, long lItemID)
    {
        CPatChecklistItemData PatChecklistItem = new CPatChecklistItemData(BaseData);
        CPatChecklistItemDataItem di = null;
        CStatus status = PatChecklistItem.GetPatCLItemDI(lPatCLID, lItemID, out di);
        if (!status.Status)
        {
            return status;
        }

        di.IsEnabled = k_TRUE_FALSE_ID.False;

        return PatChecklistItem.UpdatePatChecklistItem(di);
    }

    /// <summary>
    /// method
    /// US:902
    /// parses a specified action
    /// </summary>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private CStatus ParseSpecifiedAction(string strPlaceHolder)
    {
        string strOnlySpecifierTokens = strPlaceHolder.Replace(CExpression.ParamStartTkn, CExpression.SpecifierTkn);
        strOnlySpecifierTokens = strOnlySpecifierTokens.Replace(CExpression.ParamEndTkn, CExpression.SpecifierTkn);
        string[] straSpecifiers = strOnlySpecifierTokens.Split(new char[] { CExpression.SpecifierTkn }, StringSplitOptions.RemoveEmptyEntries);
        switch (straSpecifiers.Length)
        {
            case 3:
                try
                {
                    return ExecutePatCLI(PatCLID, ItemID, straSpecifiers[0], straSpecifiers[1], Convert.ToInt64(straSpecifiers[2]));
                }
                catch (Exception)
                {
                    return new CStatus(false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_EXE_EXP + strPlaceHolder);
                }
            default:
                return new CStatus(false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_EXE_EXP + strPlaceHolder);
        }
    }

    /// <summary>
    /// method
    /// US:910
    /// executes the action specified by the action string on the patient checklist item specified by the patient checklist id and item id
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strProperty"></param>
    /// <param name="strAction"></param>
    /// <param name="lID"></param>
    /// <returns></returns>
    private CStatus ExecutePatCLI(
        long lPatCLID,
        long lItemID,
        string strProperty,
        string strAction,
        long lID)
    {
        CPatChecklistItemData PatChecklistItem = new CPatChecklistItemData(BaseData);
        CPatChecklistItemDataItem di = null;
        CStatus status = PatChecklistItem.GetPatCLItemDI(lPatCLID, lItemID, out di);
        if (!status.Status)
        {
            return status;
        }

        switch (strAction)
        {
            case "set":
                switch (strProperty)
                {
                    case "temporalstate":
                        di.TSID = lID;
                        break;
                    case "outcomestate":
                        di.OSID = lID;
                        break;
                    case "decisionstate":
                        if (di.IsOverridden == k_TRUE_FALSE_ID.True)
                        {
                            CPatientItemData pi = new CPatientItemData(BaseData);
                            CPatientItemDataItem pidi = null;
                            status = pi.GetMostRecentPatientItemDI(
                                PatientID,
                                lItemID,
                                out pidi);
                            if (!status.Status)
                            {
                                return status;
                            }

                            if(di.OverrideDate < pidi.EntryDate)
                            {
                                di.IsOverridden = k_TRUE_FALSE_ID.False;
                                di.OverrideDate = CDataUtils.GetNullDate();
                                di.DSID = lID;
                            }
                        }
                        else
                        {
                            di.DSID = lID;
                        }
                        break;
                }
                return PatChecklistItem.UpdatePatChecklistItem(di);
            default:
                return new CStatus(false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_EXE_EXP + strProperty + CExpression.SpecifierTkn + strAction
                    + CExpression.ParamStartTkn + lID.ToString() + CExpression.ParamEndTkn);
        }
    }
}