using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using VAPPCT.DA;

using System.Collections;
using System.Threading;

public class CAssignChecklistThreadPool: CData
{
    public CAssignChecklistThreadPool(CData Data)
        : base(Data)
	{
		//
		// TODO: Add constructor logic here
		//
	}

    /// <summary>
    /// thread assign checklist to the patients
    /// </summary>
    /// <param name="strPatientIDs"></param>
    /// <param name="lChecklistID"></param>
    /// <returns></returns>
    public CStatus ThreadAssignChecklist(
        string strPatientIDs,
        long lChecklistID)
    {
        //get the patient ids into an array
        string[] astrPatientIDs = strPatientIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
       
        // Allow a total of pListPatients.Count threads in the pool
        int nMaxCount = astrPatientIDs.Count();
        if (nMaxCount == 0)
            return new CStatus();
        
        // Mark the event as unsignaled.
        ManualResetEvent eventX = new ManualResetEvent(false);
        
        //array of thread objects to do work 
        CAssignChecklistThread[] threads = new CAssignChecklistThread[nMaxCount];

        //initialize the work items 
        for (int i=0; i < nMaxCount; i++)
        {
            //create a work item
            threads[i] = new CAssignChecklistThread(
                this,
                astrPatientIDs[i],
                lChecklistID,
                nMaxCount);

            // Make sure the work items have a reference to 
            //the signaling event.
            threads[i].eventX = eventX;
        }

        //queue the work items
        for (int i = 0; i < nMaxCount; i++)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(threads[i].DoWork));
        }

        // The call to exventX.WaitOne sets the event to wait until
        // eventX.Set() occurs.
        // Wait until event is fired, meaning eventX.Set() was called:
        eventX.WaitOne(Timeout.Infinite, true);

        // The WaitOne won't return until the event has been signaled.
        
        //done processing so see if we have any errors
        for (int i = 0; i < nMaxCount; i++)
        {
            if (!threads[i].Status.Status)
            {
                return threads[i].Status;
            }
        }

        return new CStatus();
    }
}
