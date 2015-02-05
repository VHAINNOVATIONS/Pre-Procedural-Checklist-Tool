using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;
using VAPPCT.Data;

public class CParseExpression
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
    public CParseExpression(CData Data, string strPatientID, long lPatCLID, long lChecklistID, long lItemID)
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
    /// returns the parsed expression if no errors were found
    /// </summary>
    /// <param name="strExp"></param>
    /// <returns></returns>
    public CStringStatus Parse(string strExp)
    {
        string strPlaceHolder = CExpression.GetNextPlaceHolder(strExp);
        while (!string.IsNullOrEmpty(strPlaceHolder))
        {
            strPlaceHolder = strPlaceHolder.TrimStart(CExpression.BeginPHTkn).TrimEnd(CExpression.EndPHTkn);
            CStringStatus status = ParsePlaceHolder(strPlaceHolder);
            if (!status.Status)
            {
                return status;
            }

            strExp = strExp.Replace(CExpression.BeginPHTkn + strPlaceHolder + CExpression.EndPHTkn, status.Value);
            strPlaceHolder = CExpression.GetNextPlaceHolder(strExp);
        }

        return new CStringStatus(true, k_STATUS_CODE.Success, string.Empty, strExp);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the parsed action if no errors were found
    /// </summary>
    /// <param name="strExp"></param>
    /// <returns></returns>
    public CStringStatus ParseAction(string strExp)
    {
        if (string.IsNullOrEmpty(strExp))
        {
            return new CStringStatus(false, k_STATUS_CODE.Failed, "TODO", string.Empty);
        }

        int nParamStartIndex = strExp.IndexOf(CExpression.ParamStartTkn);
        int nParamEndIndex = strExp.IndexOf(CExpression.ParamEndTkn);
        if (nParamStartIndex < 0
            || nParamEndIndex < 0
            || nParamEndIndex < nParamStartIndex)
        {
            return new CStringStatus(true, k_STATUS_CODE.Success, string.Empty, strExp);
        }

        string strParam = strExp.Substring(nParamStartIndex + 1, nParamEndIndex - nParamStartIndex - 1);
        CStringStatus status = null;
        switch (strParam)
        {
            case "defaultunknownstate":
            case "defaultgoodstate":
            case "defaultbadstate":
                status = ParseStatic(strParam);
                break;
            default:
                status = ParseDynamicParam(strExp);
                break;
        }

        strExp = strExp.Replace(CExpression.ParamStartTkn + strParam + CExpression.ParamEndTkn,
                                CExpression.ParamStartTkn + status.Value + CExpression.ParamEndTkn);

        return new CStringStatus(true, k_STATUS_CODE.Success, string.Empty, strExp);
    }

    private CStringStatus ParseDynamicParam(string strExp)
    {
        string strOnlySpecifierTokens = strExp.Replace(CExpression.ParamStartTkn, CExpression.SpecifierTkn);
        strOnlySpecifierTokens = strOnlySpecifierTokens.Replace(CExpression.ParamEndTkn, CExpression.SpecifierTkn);

        string[] straSpecifiers = strOnlySpecifierTokens.Split(
            new char[] { CExpression.SpecifierTkn },
            StringSplitOptions.RemoveEmptyEntries);

        string strParamValue = string.Empty;

        switch (straSpecifiers.Length)
        {
            case 3:
                switch (straSpecifiers[0])
                {
                    case "temporalstate":
                        CTemporalStateData TemporalState = new CTemporalStateData(BaseData);
                        CTemporalStateDataItem diTS = null;
                        CStatus statusTS = TemporalState.GetTemporalStateDI(straSpecifiers[2], out diTS);
                        if (!statusTS.Status)
                        {
                            return new CStringStatus(statusTS, string.Empty);
                        }
                        strParamValue = (diTS.TSID > 0) ? diTS.TSID.ToString() : CExpression.NullTkn;
                        break;
                    case "outcomestate":
                        COutcomeStateData OutcomeState = new COutcomeStateData(BaseData);
                        COutcomeStateDataItem diOS = null;
                        CStatus statusOS = OutcomeState.GetOutcomeStateDI(straSpecifiers[2], out diOS);
                        if (!statusOS.Status)
                        {
                            return new CStringStatus(statusOS, string.Empty);
                        }
                        strParamValue = (diOS.OSID > 0) ? diOS.OSID.ToString() : CExpression.NullTkn;
                        break;
                    case "decisionstate":
                        CDecisionStateData DecisionState = new CDecisionStateData(BaseData);
                        CDecisionStateDataItem diDS = null;
                        CStatus statusDS = DecisionState.GetDecisionStateDI(straSpecifiers[2], out diDS);
                        if (!statusDS.Status)
                        {
                            return new CStringStatus(statusDS, string.Empty);
                        }
                        strParamValue = (diDS.DSID > 0) ? diDS.DSID.ToString() : CExpression.NullTkn;
                        break;
                    default:
                        return new CStringStatus(
                            false,
                            k_STATUS_CODE.Failed,
                            LogicModuleMessages.ERROR_EXE_EXP + strExp,
                            string.Empty);
                }
                break;
            default:
                return new CStringStatus(
                    false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_EXE_EXP + strExp,
                    string.Empty);
        }

        return new CStringStatus(
            true,
            k_STATUS_CODE.Success,
            string.Empty,
            strParamValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the value for a single place holder
    /// </summary>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private CStringStatus ParsePlaceHolder(string strPlaceHolder)
    {
        switch (strPlaceHolder)
        {
            case "state.good":
            case "state.unknown":
            case "state.bad":
            case "sex.male":
            case "sex.female":
            case "source.vista":
            case "source.vappct":
            case "defaultunknownstate":
            case "defaultgoodstate":
            case "defaultbadstate":
            case "value.selected":
            case "value.notselected":
                return ParseStatic(strPlaceHolder);
            case "patient.age":
            case "patient.sex":
                return ParsePatient(PatientID, strPlaceHolder);
            case "temporalstate":
            case "temporalstate.state":
            case "outcomestate":
            case "outcomestate.state":
            case "decisionstate":
            case "decisionstate.state":
                return ParsePatCLI(PatCLID, ItemID, strPlaceHolder);
            case "tstimeperiod":
            case "tstimeperioddate":
                return ParseCLI(ChecklistID, ItemID, strPlaceHolder);
            case "date":
            case "source":
            case "exists":
                return ParsePatItem(PatientID, ItemID, strPlaceHolder);
            case "summarystate.state":
                return ParsePICSummaryState(PatientID, ItemID, strPlaceHolder);
            case "lookbacktime":
                return ParseItem(ItemID, strPlaceHolder);
            default:
                return ParseSpecifiedPlaceHolder(strPlaceHolder);
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// parses a static place holder
    /// </summary>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private CStringStatus ParseStatic(string strPlaceHolder)
    {
        CStringStatus status = new CStringStatus();
        switch (strPlaceHolder)
        {
            case "state.good":
                status.Value = Convert.ToInt64(k_STATE_ID.Good).ToString();
                break;
            case "state.unknown":
                status.Value = Convert.ToInt64(k_STATE_ID.Unknown).ToString();
                break;
            case "state.bad":
                status.Value = Convert.ToInt64(k_STATE_ID.Bad).ToString();
                break;
            case "sex.male":
                status.Value = Convert.ToInt64(k_SEX.MALE).ToString();
                break;
            case "sex.female":
                status.Value = Convert.ToInt64(k_SEX.FEMALE).ToString();
                break;
            case "source.vista":
                status.Value = Convert.ToInt64(k_SOURCE_TYPE_ID.VistA).ToString();
                break;
            case "source.vappct":
                status.Value = Convert.ToInt64(k_SOURCE_TYPE_ID.VAPPCT).ToString();
                break;
            case "defaultunknownstate":
                status.Value = Convert.ToInt64(k_DEFAULT_STATE_ID.Unknown).ToString();
                break;
            case "defaultgoodstate":
                status.Value = Convert.ToInt64(k_DEFAULT_STATE_ID.Good).ToString();
                break;
            case "defaultbadstate":
                status.Value = Convert.ToInt64(k_DEFAULT_STATE_ID.Bad).ToString();
                break;
            case "value.selected":
                status.Value = Convert.ToInt64(k_TRUE_FALSE_ID.True).ToString();
                break;
            case "value.notselected":
                status.Value = Convert.ToInt64(k_TRUE_FALSE_ID.False).ToString();
                break;
            default:
                return new CStringStatus(
                    false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_PARSE + strPlaceHolder,
                    CExpression.NullTkn);
        }

        return status;
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified by the place holder for the patient specified by the patient id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private CStringStatus ParsePatient(string strPatientID, string strPlaceHolder)
    {
        CPatientData Patient = new CPatientData(BaseData);
        CPatientDataItem di = null;
        CStatus status = Patient.GetPatientDI(strPatientID, out di);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strPlaceHolder)
        {
            case "patient.age":
                strValue = (di.Age < 1) ? CExpression.NullTkn : di.Age.ToString();
                break;
            case "patient.sex":
                strValue = (Convert.ToInt64(di.Sex) < 1) ? CExpression.NullTkn : Convert.ToInt64(di.Sex).ToString();
                break;
        }

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified by the place holder for the patient item specified by the patient checklist id and item id
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private CStringStatus ParsePatCLI(long lPatCLID, long lItemID, string strPlaceHolder)
    {
        CPatChecklistItemData PatChecklistItem = new CPatChecklistItemData(BaseData);
        CPatChecklistItemDataItem di = null;
        CStatus status = PatChecklistItem.GetPatCLItemDI(lPatCLID, lItemID, out di);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strPlaceHolder)
        {
            case "temporalstate":
                strValue = (di.TSID < 1) ? CExpression.NullTkn : di.TSID.ToString();
                break;
            case "temporalstate.state":
                strValue = (di.TSStateID < 1) ? CExpression.NullTkn : di.TSStateID.ToString();
                break;
            case "outcomestate":
                strValue = (di.OSID < 1) ? CExpression.NullTkn : di.OSID.ToString();
                break;
            case "outcomestate.state":
                strValue = (di.OSStateID < 1) ? CExpression.NullTkn : di.OSStateID.ToString();
                break;
            case "decisionstate":
                strValue = (di.DSID < 1) ? CExpression.NullTkn : di.DSID.ToString();
                break;
            case "decisionstate.state":
                strValue = (di.DSStateID < 1) ? CExpression.NullTkn : di.DSStateID.ToString();
                break;
        }

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// parses a double/triple specified place holder
    /// </summary>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private CStringStatus ParseSpecifiedPlaceHolder(string strPlaceHolder)
    {
        string[] straSpecifiers = strPlaceHolder.Split(
            new char[] { CExpression.SpecifierTkn },
            StringSplitOptions.RemoveEmptyEntries);

        CItemData ItemData = new CItemData(BaseData);
        CItemDataItem di = null;
        CStatus status = ItemData.GetItemDI(straSpecifiers[0], out di);
        if (!status.Status)
        {
            return new CStringStatus(
                false,
                k_STATUS_CODE.Failed,
                LogicModuleMessages.ERROR_PARSE + strPlaceHolder,
                CExpression.NullTkn);
        }

        switch (straSpecifiers.Length)
        {
            case 2:
                return ParseDoubleSpecified(di.ItemID, straSpecifiers[1]);
            case 3:
                return ParseTripleSpecified(di.ItemID, straSpecifiers[1], straSpecifiers[2]);
            default:
                return new CStringStatus(
                    false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_PARSE + strPlaceHolder,
                    CExpression.NullTkn);
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// parses a double specified place holder
    /// </summary>
    /// <param name="lFirstID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParseDoubleSpecified(long lSpecifierOne, string strSpecifierTwo)
    {
        switch (strSpecifierTwo)
        {
            case "temporalstate":
            case "outcomestate":
            case "decisionstate":
                return ParsePatCLI(PatCLID, lSpecifierOne, strSpecifierTwo);
            case "date":
            case "source":
            case "exists":
                return ParsePatItem(PatientID, lSpecifierOne, strSpecifierTwo);
            case "lookbacktime":
                return ParseItem(lSpecifierOne, strSpecifierTwo);
            case "tstimeperiod":
            case "tstimeperioddate":
                return ParseCLI(ChecklistID, lSpecifierOne, strSpecifierTwo);
            case "value":
                return ParsePatItemComp(PatientID, ItemID, lSpecifierOne, strSpecifierTwo);
            case "legalmin":
            case "criticallow":
            case "low":
            case "high":
            case "criticalhigh":
            case "legalmax":
                return ParseItemCompRange(ItemID, lSpecifierOne, strSpecifierTwo);
            case "state":
                return ParseItemCompState(ItemID, lSpecifierOne, strSpecifierTwo);
            default:
                return new CStringStatus(false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_PARSE + lSpecifierOne.ToString() + CExpression.SpecifierTkn + strSpecifierTwo,
                    CExpression.NullTkn);
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified for the most recent patient item identified by the patient id and item id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParsePatItem(string strPatientID, long lItemID, string strSpecifier)
    {
        CPatientItemData PatientItem = new CPatientItemData(BaseData);
        CPatientItemDataItem di = null;
        CStatus status = PatientItem.GetMostRecentPatientItemDI(strPatientID, lItemID, out di);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strSpecifier)
        {
            case "date":
                strValue = (CDataUtils.IsDateNull(di.EntryDate)) ? CExpression.NullTkn : CExpression.DateTkn + di.EntryDate.ToString() + CExpression.DateTkn;
                break;
            case "source":
                strValue = (di.SourceTypeID < 1) ? CExpression.NullTkn : di.SourceTypeID.ToString();
                break;
            case "exists":
                strValue = (di.PatItemID < 1) ? false.ToString() : true.ToString();
                break;
        }

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the worst state of the patient's item's components
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParsePICSummaryState(string strPatientID, long lItemID, string strSpecifier)
    {
        CPatientItemData PatientItem = new CPatientItemData(BaseData);
        long lSummaryStateID = -1;
        CStatus status = PatientItem.GetMostRecentPICSummaryStateID(
            strPatientID,
            lItemID,
            out lSummaryStateID);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = (lSummaryStateID < 1) ? CExpression.NullTkn : lSummaryStateID.ToString();

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified for the item identified by the item id
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParseItem(long lItemID, string strSpecifier)
    {
        CItemData Item = new CItemData(BaseData);
        CItemDataItem di = null;
        CStatus status = Item.GetItemDI(lItemID, out di);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strSpecifier)
        {
            case "lookbacktime":
                strValue = (di.LookbackTime < 1) ? CExpression.NullTkn : di.LookbackTime.ToString();
                break;
        }

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the earilest date a patient item for this checklist item may have to still be considered complete
    /// </summary>
    /// <param name="di"></param>
    /// <returns></returns>
    private CStringStatus CalcTSTimePeriodDate(CChecklistItemDataItem di)
    {
        DateTime dtTSTimePeriodDate = DateTime.Now;
        switch (di.TimeUnitID)
        {
            case k_TIME_UNIT_ID.Day:
                dtTSTimePeriodDate -= TimeSpan.FromDays(di.CLITSTimePeriod);
                break;
            case k_TIME_UNIT_ID.Hour:
                dtTSTimePeriodDate -= TimeSpan.FromHours(di.CLITSTimePeriod);
                break;
            case k_TIME_UNIT_ID.Minute:
                dtTSTimePeriodDate -= TimeSpan.FromMinutes(di.CLITSTimePeriod);
                break;
            default:
                return new CStringStatus(
                    false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_PARSE_TSTIMEPERIODDATE,
                    CExpression.NullTkn);
        }

        return new CStringStatus(
            true,
            k_STATUS_CODE.Success,
            string.Empty,
            CExpression.DateTkn + dtTSTimePeriodDate.ToString() + CExpression.DateTkn);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified by the specifier for the checklist item specified by the checklist id and item id
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParseCLI(long lChecklistID, long lItemID, string strSpecifier)
    {
        CChecklistItemData ChecklistItem = new CChecklistItemData(BaseData);
        CChecklistItemDataItem di = null;
        CStatus status = ChecklistItem.GetCLItemDI(ChecklistID, lItemID, out di);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strSpecifier)
        {
            case "tstimeperiod":
                strValue = (di.CLITSTimePeriod < 1) ? CExpression.NullTkn : di.CLITSTimePeriod.ToString();
                break;
            case "tstimeperioddate":
                return CalcTSTimePeriodDate(di);
        }

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// parses a triple specified place holder
    /// </summary>
    /// <param name="lSpecifierOne"></param>
    /// <param name="strSpecifierTwo"></param>
    /// <param name="strSpecifierThree"></param>
    /// <returns></returns>
    private CStringStatus ParseTripleSpecified(long lSpecifierOne, string strSpecifierTwo, string strSpecifierThree)
    {
        switch (strSpecifierTwo)
        {
            case "temporalstate":
            case "outcomestate":
            case "decisionstate":
                return ParsePatCLI(PatCLID, lSpecifierOne, strSpecifierTwo + CExpression.SpecifierTkn + strSpecifierThree);
            case "summarystate":
                return ParsePICSummaryState(PatientID, lSpecifierOne, strSpecifierTwo + CExpression.SpecifierTkn + strSpecifierThree);
        }

        CItemComponentData ItemComponent = new CItemComponentData(BaseData);
        CItemComponentDataItem di = null;
        CStatus status = ItemComponent.GetItemComponentDI(lSpecifierOne, strSpecifierTwo, out di);
        if (!status.Status)
        {
            return new CStringStatus(false,
                k_STATUS_CODE.Failed,
                LogicModuleMessages.ERROR_PARSE + lSpecifierOne.ToString() + CExpression.SpecifierTkn
                + strSpecifierTwo + CExpression.SpecifierTkn + strSpecifierThree,
                CExpression.NullTkn);
        }

        switch (strSpecifierThree)
        {
            case "value":
                return ParsePatItemComp(PatientID, lSpecifierOne, di.ItemComponentID, strSpecifierThree);
            case "legalmin":
            case "criticallow":
            case "low":
            case "high":
            case "criticalhigh":
            case "legalmax":
                return ParseItemCompRange(lSpecifierOne, di.ItemComponentID, strSpecifierThree);
            case "state":
                return ParseItemCompState(lSpecifierOne, di.ItemComponentID, strSpecifierThree);
            default:
                return new CStringStatus(false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_PARSE + lSpecifierOne.ToString() + CExpression.SpecifierTkn
                    + di.ItemComponentID.ToString() + CExpression.SpecifierTkn + strSpecifierThree,
                    CExpression.NullTkn);
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified by the specifier for the patient item component
    /// specified by the patient id, item id and item component id
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <param name="lItemComponentID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParsePatItemComp(string strPatientID, long lItemID, long lItemComponentID, string strSpecifier)
    {
        CStatus status = null;

        CPatientItemData PatientItem = new CPatientItemData(BaseData);
        CPatientItemComponentDataItem diPatItem = null;
        status = PatientItem.GetMostRecentPatientItemCompDI(strPatientID, lItemID, lItemComponentID, out diPatItem);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strSpecifier)
        {
            case "value":
                strValue = (string.IsNullOrEmpty(diPatItem.ComponentValue)) ? CExpression.NullTkn : diPatItem.ComponentValue;
                break;
        }

        CItemData Item = new CItemData(BaseData);
        CItemDataItem diItem = null;
        status = Item.GetItemDI(lItemID, out diItem);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        if (diItem.ItemTypeID != (long)k_ITEM_TYPE_ID.Laboratory)
        {
            strValue = CExpression.StringTkn + strValue + CExpression.StringTkn;
        }

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified by the specifier for the item component range for
    /// the item component range specified by the item id and item component id
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="lItemComponentID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParseItemCompRange(long lItemID, long lItemComponentID, string strSpecifier)
    {
        CItemComponentData ItemComp = new CItemComponentData(BaseData);
        CICRangeDataItem di = null;
        CStatus status = ItemComp.GetICRangeDI(lItemID, lItemComponentID, out di);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strSpecifier)
        {
            case "legalmin":
                strValue = (di.LegalMin < 0) ? CExpression.NullTkn : di.LegalMin.ToString();
                break;
            case "criticallow":
                strValue = (di.CriticalLow < 0) ? CExpression.NullTkn : di.CriticalLow.ToString();
                break;
            case "low":
                strValue = (di.Low < 0) ? CExpression.NullTkn : di.Low.ToString();
                break;
            case "high":
                strValue = (di.High < 0) ? CExpression.NullTkn : di.High.ToString();
                break;
            case "criticalhigh":
                strValue = (di.CriticalHigh < 0) ? CExpression.NullTkn : di.CriticalHigh.ToString();
                break;
            case "legalmax":
                strValue = (di.LegalMax < 0) ? CExpression.NullTkn : di.LegalMax.ToString();
                break;
        }

        return new CStringStatus(status, strValue);
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the attribute specified by the specifier for the item component state
    /// for the item component state specified by the item id and item component id
    /// </summary>
    /// <param name="lItemID"></param>
    /// <param name="lItemComponentID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private CStringStatus ParseItemCompState(long lItemID, long lItemComponentID, string strSpecifier)
    {
        CItemComponentData ItemComp = new CItemComponentData(BaseData);
        CICStateDataItem di = null;
        CStatus status = ItemComp.GetICStateDI(lItemID, lItemComponentID, out di);
        if (!status.Status)
        {
            return new CStringStatus(status, CExpression.NullTkn);
        }

        string strValue = CExpression.NullTkn;
        switch (strSpecifier)
        {
            case "state":
                strValue = (di.StateID < 1) ? CExpression.NullTkn : di.StateID.ToString();
                break;
        }

        return new CStringStatus(status, strValue);
    }
}