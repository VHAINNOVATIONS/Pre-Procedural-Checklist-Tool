using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAPPCT.DA;
using VAPPCT.Data;

public class CValidateExpression
{
    /// <summary>
    /// constructor
    /// does nothing
    /// </summary>
    public CValidateExpression()
    {
    }

    /// <summary>
    /// method
    /// US:902
    /// validates the syntax and placeholders for the expression passed in
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public CStatus Validate(CExpression exp)
    {
        CStatus status = ValidateSyntax(exp);
        if (!status.Status)
        {
            return status;
        }

        status = ValidatePlaceHolders(exp.GetIf());
        if (!status.Status)
        {
            return status;
        }

        status = ValidateActions(exp.GetThen());
        if (!status.Status)
        {
            return status;
        }

        string strElse = exp.GetElse();
        if (!string.IsNullOrEmpty(strElse))
        {
            status = ValidateActions(exp.GetElse());
            if (!status.Status)
            {
                return status;
            }
        }

        return status;
    }

    /// <summary>
    /// method
    /// US:902
    /// validates the syntax for the expression passed in
    /// </summary>
    /// <param name="strExp"></param>
    /// <returns></returns>
    private CStatus ValidateSyntax(CExpression Exp)
    {
        string strExp = Exp.Expression;
        if (string.IsNullOrEmpty(strExp))
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_NULL);
        }

        // if the if token is not at the beginning
        int nIfIndex = strExp.IndexOf(CExpression.IfTkn);
        if (nIfIndex != 0)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_IF);
        }

        // if there are two if tokens
        int nIfCount = strExp.Split(new string[] { CExpression.IfTkn }, StringSplitOptions.None).Count() - 1;
        if (nIfCount > 1)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_IF_DOUBLE);
        }

        // if the then token does not exist
        int nThenIndex = strExp.IndexOf(CExpression.ThenTkn);
        if (nThenIndex < 0)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_THEN);
        }

        // if there are two then tokens
        int nThenCount = strExp.Split(new string[] { CExpression.ThenTkn }, StringSplitOptions.None).Count() - 1;
        if (nThenCount > 1)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_THEN_DOUBLE);
        }

        if (string.IsNullOrEmpty(Exp.GetIf()))
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_IF_EMPTY);
        }

        // if the expression end token is not at the end
        int nExpEndIndex = strExp.IndexOf(CExpression.EndTkn);
        if (nExpEndIndex != strExp.Length - 1)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_SEMI);
        }

        // if there are two end tokens
        int nEndCount = strExp.Split(new char[] { CExpression.EndTkn }, StringSplitOptions.None).Count() - 1;
        if (nEndCount > 1)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_SEMI_DOUBLE);
        }

        if (string.IsNullOrEmpty(Exp.GetThen()))
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_THEN_EMPTY);
        }

        // the else token is not required
        // if the else token exists
        int nElseIndex = strExp.IndexOf(CExpression.ElseTkn);
        if (nElseIndex >= 0)
        {
            // if there are two else tokens
            int nElseCount = strExp.Split(new string[] { CExpression.ElseTkn }, StringSplitOptions.None).Count() - 1;
            if (nElseCount > 1)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_ELSE_DOUBLE);
            }

            // if the else token is before the then token
            if (nElseIndex < nThenIndex)
            {
                return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_IF_ELSE_THEN);
            }

            if (string.IsNullOrEmpty(Exp.GetElse()))
            {
                return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_ELSE_EMPTY);
            }
        }

        return new CStatus(true, k_STATUS_CODE.Success, LogicModuleMessages.SUCCESS_VALIDATE);
    }

    /// <summary>
    /// method
    /// US:902
    /// checks the place holder against the logic white list
    /// </summary>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private bool ValidatePlaceHolder(string strPlaceHolder)
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
            case "patient.age":
            case "patient.sex":
            case "temporalstate":
            case "temporalstate.state":
            case "outcomestate":
            case "outcomestate.state":
            case "decisionstate":
            case "decisionstate.state":
            case "tstimeperiod":
            case "tstimeperioddate":
            case "date":
            case "source":
            case "exists":
            case "summarystate.state":
            case "lookbacktime":
            case "defaultgoodstate":
            case "defaultunknownstate":
            case "defaultbadstate":
            case "value.selected":
            case "value.notselected":
                return true;
            default:
                return ValidateSpecifiedPlaceHolder(strPlaceHolder);
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// validates the placeholders for the expression passed in
    /// </summary>
    /// <param name="strExp"></param>
    /// <returns></returns>
    private CStatus ValidatePlaceHolders(string strExp)
    {
        string strPlaceHolder = CExpression.GetNextPlaceHolder(strExp);
        while (!string.IsNullOrEmpty(strPlaceHolder))
        {
            strPlaceHolder = strPlaceHolder.TrimStart(CExpression.BeginPHTkn).TrimEnd(CExpression.EndPHTkn);
            bool bValid = ValidatePlaceHolder(strPlaceHolder);
            if (!bValid)
            {
                return new CStatus(bValid, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_PLACEHOLDER + strPlaceHolder);
            }

            strExp = strExp.Replace(CExpression.BeginPHTkn + strPlaceHolder + CExpression.EndPHTkn, strPlaceHolder);
            strPlaceHolder = CExpression.GetNextPlaceHolder(strExp);
        }

        return new CStatus(true, k_STATUS_CODE.Success, LogicModuleMessages.SUCCESS_VALIDATE);
    }

    /// <summary>
    /// method
    /// US:902
    /// validates specified placeholders
    /// </summary>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private bool ValidateSpecifiedPlaceHolder(string strPlaceHolder)
    {
        string[] straSpecifiers = strPlaceHolder.Split(new char[] { CExpression.SpecifierTkn }, StringSplitOptions.RemoveEmptyEntries);
        long lSpecifierOne = 1;
        switch (straSpecifiers.Length)
        {
            case 2:
                return ValidateDoubleSpecified(lSpecifierOne, straSpecifiers[1]);
            case 3:
                return ValidateTripleSpecified(lSpecifierOne, straSpecifiers[1], straSpecifiers[2]);
            default:
                return false;
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// validates a double specified placeholder
    /// </summary>
    /// <param name="lFirstID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private bool ValidateDoubleSpecified(long lSpecifierOne, string strSpecifierTwo)
    {
        switch (strSpecifierTwo)
        {
            case "date":
            case "source":
            case "exists":
            case "lookbacktime":
            case "tstimeperiod":
            case "tstimeperioddate":
            case "value":
            case "legalmin":
            case "criticallow":
            case "low":
            case "high":
            case "criticalhigh":
            case "legalmax":
            case "state":
            case "temporalstate":
            case "outcomestate":
            case "decisionstate":
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// validates a triple specified placeholder
    /// </summary>
    /// <param name="lFirstID"></param>
    /// <param name="lSecondID"></param>
    /// <param name="strSpecifier"></param>
    /// <returns></returns>
    private bool ValidateTripleSpecified(long lSpecifierOne, string strSpecifierTwo, string strSpecifierThree)
    {
        switch (strSpecifierTwo)
        {
            case "temporalstate":
            case "outcomestate":
            case "decisionstate":
            case "summarystate":
                return (strSpecifierThree == "state") ? true : false;
        }

        switch (strSpecifierThree)
        {
            case "value":
            case "legalmin":
            case "criticallow":
            case "low":
            case "high":
            case "criticalhigh":
            case "legalmax":
            case "state":
                return true;
            default:
                return false;
        }
    }

    private CStatus ValidateParameters(string strExp)
    {
        if (string.IsNullOrEmpty(strExp))
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_NULL);
        }

        int nParamStartIndex = strExp.IndexOf(CExpression.ParamStartTkn);
        int nParamEndIndex = strExp.IndexOf(CExpression.ParamEndTkn);
        if (nParamStartIndex < 0
            || nParamEndIndex < 0
            || nParamEndIndex < nParamStartIndex)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_PARAM);
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:902
    /// validates the actions for the expression passed
    /// </summary>
    /// <param name="strExp"></param>
    /// <returns></returns>
    private CStatus ValidateActions(string strExp)
    {
        CStatus status = ValidateParameters(strExp);
        if (!status.Status)
        {
            return status;
        }

        bool bValid = false;
        switch (strExp)
        {
            case "checklist.cancel()":
            case "item.disable()":
                bValid = true;
                break;
            default:
                bValid = ValidateSpecifiedAction(strExp);
                break;
        }

        if (!bValid)
        {
            return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_VALIDATE_ACTION + strExp);
        }

        return new CStatus(true, k_STATUS_CODE.Success, LogicModuleMessages.SUCCESS_VALIDATE);
    }

    /// <summary>
    /// method
    /// US:902
    /// validates a specified action
    /// </summary>
    /// <param name="strPlaceHolder"></param>
    /// <returns></returns>
    private bool ValidateSpecifiedAction(string strPlaceHolder)
    {
        string strOnlySpecifierTokens = strPlaceHolder.Replace(CExpression.ParamStartTkn, CExpression.SpecifierTkn);
        strOnlySpecifierTokens = strOnlySpecifierTokens.Replace(CExpression.ParamEndTkn, CExpression.SpecifierTkn);
        string[] straSpecifiers = strOnlySpecifierTokens.Split(new char[] { CExpression.SpecifierTkn }, StringSplitOptions.RemoveEmptyEntries);
        switch (straSpecifiers.Length)
        {
            case 3:
                try
                {
                    return ValidateTriSpecifiedAction(straSpecifiers[0], straSpecifiers[1], straSpecifiers[2]);
                }
                catch (Exception)
                {
                    return false;
                }
            default:
                return false;
        }
    }

    /// <summary>
    /// method
    /// US:902
    /// validates a triple specified action
    /// </summary>
    /// <param name="strProperty"></param>
    /// <param name="strAction"></param>
    /// <param name="lID"></param>
    /// <returns></returns>
    private bool ValidateTriSpecifiedAction(string strProperty, string strAction, string strState)
    {
        switch (strAction)
        {
            case "set":
                switch (strProperty)
                {
                    case "temporalstate":
                    case "outcomestate":
                    case "decisionstate":
                        return true;
                    default:
                        return false;
                }
            default:
                return false;
        }
    }
}