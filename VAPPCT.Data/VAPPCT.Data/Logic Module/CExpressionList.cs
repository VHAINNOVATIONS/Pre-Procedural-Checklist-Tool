using System;
using System.Linq;
using System.Web;
using System.Collections;
using VAPPCT.DA;
using VAPPCT.Data;

public class CExpressionList : ArrayList
{
    /// <summary>
    /// property
    /// gets/sets the data for the instance
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
    public CExpressionList(CData Data, string strPatientID, long lPatCLID, long lChecklistID, long lItemID)
    {
        BaseData = Data;
        PatientID = strPatientID;
        PatCLID = lPatCLID;
        ChecklistID = lChecklistID;
        ItemID = lItemID;
        Capacity = 5;
    }

    /// <summary>
    /// override
    /// US:902
    /// adds an expression to the list
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override int Add(object value)
    {
        CExpression exp = value as CExpression;
        if (exp == null)
        {
            return -1;
        }
        return base.Add(value);
    }

    /// <summary>
    /// method
    /// US:902
    /// splits the expressions string and adds the expressions to the list
    /// </summary>
    /// <param name="strExpressions"></param>
    /// <returns></returns>
    private CStatus SplitAddLogicExpressions(string strExpressions)
    {
        string[] straSplit = strExpressions.Trim().Split(new char[] { CExpression.EndTkn }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string str in straSplit)
        {
            Add(new CExpression(str.Trim() + CExpression.EndTkn));
        }

        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:902
    /// loads all the logic expressions for an item into the list
    /// </summary>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus Load()
    {
        string strItemLogic = string.Empty;
        CChecklistItemData dta = new CChecklistItemData(BaseData);
        CChecklistItemDataItem di = null;
        CStatus status = dta.GetCLItemDI(ChecklistID, ItemID, out di);
        if (!status.Status)
        {
            return status;
        }

        SplitAddLogicExpressions(di.Logic);
        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:902
    /// loads all the logic expressions in the expressions string into the list
    /// </summary>
    /// <param name="strExpressions"></param>
    /// <returns></returns>
    public CStatus Load(string strExpressions)
    {
        return SplitAddLogicExpressions(strExpressions);
    }

    /// <summary>
    /// method
    /// US:902
    /// validates all the expressions in the list
    /// </summary>
    /// <returns></returns>
    public CStatus Validate()
    {
        CValidateExpression ValidateExp = new CValidateExpression();
        foreach (CExpression exp in this)
        {
            CStatus status = ValidateExp.Validate(exp);
            if (!status.Status)
            {
                return status;
            }
        }
        return new CStatus();
    }

    /// <summary>
    /// method
    /// US:902
    /// evaluates all the expressions in the list
    /// </summary>
    /// <returns></returns>
    public CStatus Evaluate()
    {
        CParseExpression ParseExp = new CParseExpression(BaseData, PatientID, PatCLID, ChecklistID, ItemID);
        CStatus status = new CStatus();
        foreach (CExpression exp in this)
        {
            CStringStatus ss = ParseExp.Parse(exp.GetIf());
            if (!ss.Status)
            {
                status = ss;
                break;
            }

            if (ss.Value.IndexOf(CExpression.NullTkn) >= 0)
            {
                CPatChecklistItemData PatChecklistItem = new CPatChecklistItemData(BaseData);
                CPatChecklistItemDataItem di = null;
                status = PatChecklistItem.GetPatCLItemDI(PatCLID, ItemID, out di);
                if (!status.Status)
                {
                    return status;
                }

                di.TSID = Convert.ToInt64(k_DEFAULT_STATE_ID.Bad);
                di.OSID = Convert.ToInt64(k_DEFAULT_STATE_ID.Unknown);
                di.IsOverridden = k_TRUE_FALSE_ID.False;
                di.OverrideDate = CDataUtils.GetNullDate();
                di.DSID = Convert.ToInt64(k_DEFAULT_STATE_ID.Bad);

                status = PatChecklistItem.UpdatePatChecklistItem(di);
                if (!status.Status)
                {
                    return status;
                }

                continue;
            }

            int nResult = CLogic.Evaluate(ss.Value);
            CExecuteExpression ExecuteExp = new CExecuteExpression(
                BaseData,
                PatientID,
                PatCLID,
                ChecklistID,
                ItemID);
            switch (nResult)
            {
                // false
                case 0:
                    string strElse = exp.GetElse();
                    if (!string.IsNullOrEmpty(strElse))
                    {
                        status = ExecuteExp.Execute(strElse);
                    }
                    break;
                // true
                case 1:
                    status = ExecuteExp.Execute(exp.GetThen());
                    break;
                // error
                case 2:
                    status.Status = false;
                    status.StatusCode = k_STATUS_CODE.Failed;
                    status.StatusComment = LogicModuleMessages.ERROR_LOGIC + exp.Expression;
                    break;
            }

            if (!status.Status)
            {
                break;
            }
        }

        return status;
    }
}