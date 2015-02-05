using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Summary description for CNoteTitleData
/// </summary>
public class CNoteTitleData : CData
{
    public CNoteTitleData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }


    /// <summary>
    /// US:1880 US:885 saves a note title
    /// </summary>
    /// <param name="lXferSystemID"></param>
    /// <param name="lNoteTitleID"></param>
    /// <param name="strNoteTitleLabel"></param>
    /// <param name="strNoteTitleDetails"></param>
    /// <returns></returns>
    public CStatus SaveNoteTitle(long lXferSystemID,
                                 long lNoteTitleTag,
                                 string strNoteTitleLabel,
                                 string strNoteTitleDetails)
    {
        CStatus status = new CStatus();

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        pList.AddInputParameter("pi_nXferSystemID", lXferSystemID);
        pList.AddInputParameter("pi_nNoteTitleTag", lNoteTitleTag);
        pList.AddInputParameter("pi_vNoteTitleLabel", strNoteTitleLabel);
        pList.AddInputParameter("pi_vNoteTitleDetails", strNoteTitleDetails);

        return DBConn.ExecuteOracleSP("PCK_NOTE_TITLE.SaveNoteTitle", pList);
    }

    /// <summary>
    ///  US:1880 US:885 gets the note title ien (tag) for the note title passed in
    /// </summary>
    /// <param name="strNoteTitle"></param>
    /// <param name="lNoteTitleIEN"></param>
    /// <returns></returns>
    public CStatus GetNoteTitleIEN(string strNoteTitle, out long lNoteTitleIEN)
    {
        lNoteTitleIEN = 0;
        CStatus status = new CStatus();

        //make sure we have a valid note title
        if (String.IsNullOrEmpty(strNoteTitle))
        {
            //todo error?
            return status;
        }

        DataSet dsNoteTitles = null;
        GetNoteTitleDS(out dsNoteTitles);

        //loop and find the title and return the ien
        if (!CDataUtils.IsEmpty(dsNoteTitles))
        {
            foreach (DataTable table in dsNoteTitles.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    string strTitle = Convert.ToString(dr["note_title_label"]);
                    if (strTitle.ToLower().Trim() ==
                        strNoteTitle.ToLower().Trim())
                    {
                        lNoteTitleIEN = CDataUtils.ToLong(Convert.ToString(dr["note_title_tag"]));
                        break;
                    }
                }
            }
        }

        return status;
    }

    /// <summary>
    ///  US:1880 US:885 gets note title dataset
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    public CStatus GetNoteTitleDS(out DataSet ds)
    {
        //initialize parameters
        ds = null;
        CStatus status = new CStatus();

        //transfer from MDWS if needed
        if (MDWSTransfer)
        {
            long lCount = 0;
            CMDWSOps ops = new CMDWSOps(this);
            status = ops.GetMDWSNoteTitles(out lCount);
            if (!status.Status)
            {
                return status;
            }
        }

        //load the paramaters list
        CParameterList pList = new CParameterList(base.SessionID,
                                                  base.ClientIP,
                                                  base.UserID);

        //get the dataset
        CDataSet cds = new CDataSet();
        status = cds.GetOracleDataSet(base.DBConn,
                                       "PCK_NOTE_TITLE.GetNoteTitleRS",
                                       pList,
                                       out ds);
        return status;
    }
}
