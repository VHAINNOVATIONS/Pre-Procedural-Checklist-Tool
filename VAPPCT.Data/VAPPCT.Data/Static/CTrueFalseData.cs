using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CTrueFalseData
/// </summary>
public class CTrueFalseData : CData
{
    public CTrueFalseData(CData data)
        : base(data)
	{

	}

    public CStatus GetTrueFalseDI(long lTrueFalseID, out CTrueFalseDataItem di)
    {
        //initialize parameters
        di = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);
        pList.AddInputParameter("pi_nTrueFalseID", lTrueFalseID);


        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_STAT.GetTrueFalseDI",
                                      pList,
                                      out ds);
        if (status.Status)
        {
            di = new CTrueFalseDataItem(ds);
        }

        return status;
    }
}
