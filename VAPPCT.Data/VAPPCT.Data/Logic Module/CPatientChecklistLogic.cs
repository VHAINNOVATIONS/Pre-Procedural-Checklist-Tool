using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using VAPPCT.DA;
using VAPPCT.Data;

/// <summary>
/// Summary description for CPatientChecklistLogic
/// </summary>
public class CPatientChecklistLogic : CData
{
    //added so we can run logic on a thread
    public long PatChecklistID = 0;
    public void DoWork(Object state)
    {
        RunLogic(PatChecklistID);
    }

    /// <summary>
    /// constructor
    /// initializes data object
    /// </summary>
    /// <param name="Data"></param>
    public CPatientChecklistLogic(CData Data)
        : base(Data)
    {
    }

    /// <summary>
    /// method
    /// US:902
    /// runs the logic for all the checklists for the patient specified containing the item specified
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus RunLogic(string strPatientID, long lItemID)
    {
        CPatChecklistData pcl = new CPatChecklistData(this);
        DataSet dsPatientChecklists = null;
        CStatus status = pcl.GetPatChecklistDS(strPatientID, lItemID, out dsPatientChecklists);
        if (!status.Status)
        {
            return status;
        }

        CPatChecklistItemData pcli = new CPatChecklistItemData(this);
        foreach (DataRow drChecklist in dsPatientChecklists.Tables[0].Rows)
        {
            try
            {
                status = RunLogic(Convert.ToInt64(drChecklist["PAT_CL_ID"]));
            }
            catch
            {
                return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_RUN_LOGIC);
            }

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
    /// runs the logic for all the checklists for the patient specified
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus RunLogic(string strPatientID)
    {
        CPatChecklistData pcl = new CPatChecklistData(this);
        DataSet dsPatientChecklists = null;
        CStatus status = pcl.GetPatChecklistDS(strPatientID, out dsPatientChecklists);
        if (!status.Status)
        {
            return status;
        }

        CPatChecklistItemData pcli = new CPatChecklistItemData(this);
        foreach (DataRow drChecklist in dsPatientChecklists.Tables[0].Rows)
        {
            try
            {
                status = RunLogic(Convert.ToInt64(drChecklist["PAT_CL_ID"]));
            }
            catch
            {
                return new CStatus(false, k_STATUS_CODE.Failed, LogicModuleMessages.ERROR_RUN_LOGIC);
            }

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
    /// runs the logic for the specified patient checklist if it is open
    /// </summary>
    /// <param name="lPatCLID"></param>
    /// <returns></returns>
    public CStatus RunLogic(long lPatCLID)
    {
        CPatChecklistData pcl = new CPatChecklistData(this);
        CPatChecklistDataItem di = null;
        CStatus status = pcl.GetPatChecklistDI(lPatCLID, out di);
        if (!status.Status)
        {
            return status;
        }

        if (di.ChecklistStateID != k_CHECKLIST_STATE_ID.Open)
        {
            return new CStatus();
        }

        DataSet dsChecklistItems = null;
        CPatChecklistItemData pcli = new CPatChecklistItemData(this);

        status = pcli.GetPatCLItemsByPatCLIDDS(lPatCLID, out dsChecklistItems);
        if (!status.Status)
        {
            return status;
        }

        foreach (DataRow drItem in dsChecklistItems.Tables[0].Rows)
        {
            try
            {
                //get the item type id
                long lItemTypeID = Convert.ToInt64(drItem["ITEM_TYPE_ID"]);
                string strPatientID = drItem["PATIENT_ID"].ToString();
                long lPatChecklistID = Convert.ToInt64(drItem["PAT_CL_ID"]);
                long lChecklistID = Convert.ToInt64(drItem["CHECKLIST_ID"]);
                long lItemID = Convert.ToInt64(drItem["ITEM_ID"]);

                CExpressionList expList = new CExpressionList(
                    this,
                    drItem["PATIENT_ID"].ToString(),
                    Convert.ToInt64(drItem["PAT_CL_ID"]),
                    Convert.ToInt64(drItem["CHECKLIST_ID"]),
                    Convert.ToInt64(drItem["ITEM_ID"]));

                status = expList.Load(drItem["LOGIC"].ToString());
                if (!status.Status)
                {
                    return status;
                }

                status = expList.Evaluate();
                if (!status.Status)
                {
                    return status;
                }

                //todo: work in progress! this is on hold for now
                //if (lItemTypeID == (long)k_ITEM_TYPE_ID.Collection)
                //{
                //    status = RunCollectionLogic(strPatientID,
                //                                lPatChecklistID,
                //                                lChecklistID,
                //                                lItemID);
                //
                //}
            }
            catch (Exception)
            {
                return new CStatus(
                    false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_RUN_LOGIC);
            }
        }

        return new CStatus();
    }

    /// <summary>
    /// runs logic for a collection item this is na for now! 
    /// too many issues to do this. ON hOLD!
    /// </summary>
    /// <param name="strPatientID"></param>
    /// <param name="lPatCLID"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="lItemID"></param>
    /// <returns></returns>
    public CStatus RunCollectionLogic(string strPatientID,
                                      long lPatCLID,
                                      long lChecklistID,
                                      long lItemID)
    {
        CStatus status = new CStatus();

        DataSet dsColItems = null; 

        CItemCollectionData icd = new CItemCollectionData(this);
        icd.GetItemCollectionDS(lItemID, out dsColItems);
        foreach (DataRow drItem in dsColItems.Tables[0].Rows)
        {
            try
            {
                //get the item type id
                long lColItemID = Convert.ToInt64(drItem["ITEM_ID"]);
                
                long lSummaryStateID = -1;
                CPatientItemData pid = new CPatientItemData(this);
                pid.GetMostRecentPICSummaryStateID(strPatientID,
                                                    lColItemID,
                                                    out lSummaryStateID);

                CPatientItemDataItem di = null;
                pid.GetMostRecentPatientItemDI(strPatientID,
                                                lColItemID,
                                                out di);
                         
            }
            catch (Exception)
            {
                return new CStatus(
                    false,
                    k_STATUS_CODE.Failed,
                    LogicModuleMessages.ERROR_RUN_LOGIC);
            }
        }
        
        return status;
    }

}
