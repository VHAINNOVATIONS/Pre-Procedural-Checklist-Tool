using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;

/// <summary>
/// Summary description for CPlaceHolderData
/// </summary>
public class CPlaceHolderData : CData
{
    public CPlaceHolderData(CData data)
        : base(data)
	{
	}

    public CStatus GetPlaceHolderDI(long lPlaceHolderID, out CPlaceHolderDataItem di)
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
        pList.AddInputParameter("pi_nPlaceHolderID", lPlaceHolderID);

        //get the dataset
        CDataSet cds = new CDataSet();
        DataSet ds = null;
        status = cds.GetOracleDataSet(DBConn,
                                      "PCK_PLACE_HOLDER.GetPlaceHolderDI",
                                      pList,
                                      out ds);
        if (!status.Status)
        {
            return status;
        }

        di = new CPlaceHolderDataItem(ds);

        return status;
    }

    public CStatus GetPlaceHolderParentDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                    "PCK_PLACE_HOLDER.GetPlaceHolderParentRS",
                                    pList,
                                    out ds);
    }

    public CStatus GetPlaceHolderChildDS(long lPlaceHolderID, out DataSet ds)
    {
        //initialize parameters
        ds = null;

        //create a status object and check for valid dbconnection
        CStatus status = DBConnValid();
        if (!status.Status)
        {
            return status;
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(SessionID, ClientIP, UserID);
        pList.AddInputParameter("pi_nPlaceHolderID", lPlaceHolderID);

        //get the dataset
        CDataSet cds = new CDataSet();
        return cds.GetOracleDataSet(DBConn,
                                    "PCK_PLACE_HOLDER.GetPlaceHolderChildRS",
                                    pList,
                                    out ds);
    }
}
