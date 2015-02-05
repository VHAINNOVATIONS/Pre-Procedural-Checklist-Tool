using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

public static class CLogic
{
    /// <summary>
    /// US:902
    /// evaluate the logic expression
    /// </summary>
    /// <param name="strExpression"></param>
    /// <returns></returns>
    public static int Evaluate(string strExpression)
    {
        try
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("EXPRESSION", Type.GetType("System.Boolean"), strExpression));

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            return Convert.ToInt32(Convert.ToBoolean(dr["EXPRESSION"]));
        }
        catch (Exception)
        {
            return 2;
        }
    }
}
