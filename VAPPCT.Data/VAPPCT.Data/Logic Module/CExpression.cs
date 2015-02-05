using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAPPCT.DA;

public class CExpression
{
    public const string IfTkn = "if ";
    public const string ThenTkn = " then ";
    public const string ElseTkn = " else ";
    public const char EndTkn = ';';
    public const char BeginPHTkn = '{';
    public const char EndPHTkn = '}';
    public const char SpecifierTkn = '.';
    public const char ParamStartTkn = '(';
    public const char ParamEndTkn = ')';
    public const char DateTkn = '#';
    public const string NullTkn = "nu!!";
    public const char StringTkn = '\'';

    public const string DefaultTemporalLogic =
        "if {Date} < {TSTimePeriodDate} "
        + "then TemporalState.Set(DefaultBadState) "
        + "else TemporalState.Set(DefaultGoodState);";

    public const string DefaultOutcomeLogic =
        "if {SummaryState.State} = {State.Good} "
        + "then OutcomeState.Set(DefaultGoodState);"
        + "if {SummaryState.State} = {State.Unknown} "
        + "then OutcomeState.Set(DefaultUnknownState);"
        + "if {SummaryState.State} = {State.Bad} "
        + "then OutcomeState.Set(DefaultBadState);";

    public const string DefaultDecisionLogic =
        "if {TemporalState.State} <> {State.Good} or {OutcomeState.State} <> {State.Good} "
        + "then DecisionState.Set(DefaultBadState) "
        + "else DecisionState.Set(DefaultGoodState);";

    /// <summary>
    /// property
    /// gets/sets the stored expression string
    /// </summary>
    public string Expression { get; set; }

    /// <summary>
    /// constructor
    /// sets the expression string
    /// </summary>
    /// <param name="strExpression"></param>
    public CExpression(string strExpression)
    {
        Expression = strExpression.Trim().ToLower();
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the if portion of the expression
    /// </summary>
    /// <returns></returns>
    public string GetIf()
    {
        int nIfIndex = Expression.IndexOf(IfTkn);
        int nThenIndex = Expression.IndexOf(ThenTkn);
        if (nIfIndex < 0 || nThenIndex < 0)
        {
            return string.Empty;
        }

        return Expression.Substring(nIfIndex + IfTkn.Length, nThenIndex - nIfIndex - IfTkn.Length).Trim();
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the then portion of the expression
    /// </summary>
    /// <returns></returns>
    public string GetThen()
    {
        int nThenIndex = Expression.IndexOf(ThenTkn);
        int nEndIndex = Expression.IndexOf(ElseTkn);
        if (nEndIndex == -1)
        {
            nEndIndex = Expression.IndexOf(EndTkn);
        }

        if (nThenIndex < 0 || nEndIndex < 0)
        {
            return string.Empty;
        }

        return Expression.Substring(nThenIndex + ThenTkn.Length, nEndIndex - nThenIndex - ThenTkn.Length).Trim();
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the else portion of the expression
    /// </summary>
    /// <returns></returns>
    public string GetElse()
    {
        int nElseIndex = Expression.IndexOf(ElseTkn);
        int nEndIndex = Expression.IndexOf(EndTkn);
        if (nElseIndex < 0 || nEndIndex < 0)
        {
            return string.Empty;
        }

        return Expression.Substring(nElseIndex + ElseTkn.Length, nEndIndex - nElseIndex - ElseTkn.Length).Trim();
    }

    /// <summary>
    /// method
    /// US:902
    /// returns the next place holder in the strExp parameter
    /// </summary>
    /// <param name="strExp"></param>
    /// <returns></returns>
    public static string GetNextPlaceHolder(string strExp)
    {
        int nBeginIndex = strExp.IndexOf(BeginPHTkn);
        int nEndIndex = strExp.IndexOf(EndPHTkn);
        if (nBeginIndex < 0
            || nEndIndex < 0
            || nEndIndex < nBeginIndex)
        {
            return string.Empty;
        }

        return strExp.Substring(nBeginIndex, nEndIndex - nBeginIndex + 1);
    }
}