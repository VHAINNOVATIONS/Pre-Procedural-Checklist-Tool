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
using System.Data;

public class CMPRefreshThread: CData
{
    //thread related properties
    public Hashtable HashCount;
    public ManualResetEvent eventX;
    public static int ThreadCount = 0;
    public static int ThreadMax = 0;
    
    //properties
    public long PatientChecklistID = 0;
    public CStatus Status = new CStatus();

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="strPatientID"></param>
    /// <param name="lChecklistID"></param>
    /// <param name="nThreadMax"></param>
    public CMPRefreshThread(CData Data,
                             long lPatChecklistID,
                             int nThreadMax) : base(Data)
	{
        //init thread related properties
        HashCount = new Hashtable(nThreadMax);
        ThreadMax = nThreadMax;
      
        //init properties
        PatientChecklistID = lPatChecklistID;
   	}

    /// <summary>
    /// do the actual work of assigning the checklist
    /// </summary>
    /// <param name="state"></param>
    public void DoWork(Object state)
    {
        //thread hashcount work
        lock (HashCount)
        {
            if (!HashCount.ContainsKey(Thread.CurrentThread.GetHashCode()))
            {
                HashCount.Add(Thread.CurrentThread.GetHashCode(), 0);
            }

            HashCount[Thread.CurrentThread.GetHashCode()] = ((int)HashCount[Thread.CurrentThread.GetHashCode()]) + 1;
        }
                
        //create a new connection for the thread
        CDataDBConn conn = new CDataDBConn();
        conn.Connect();
        CData data = new CData( conn,
                                this.ClientIP,
                                this.UserID,
                                this.SessionID,
                                this.WebSession,
                                this.MDWSTransfer);
        //do real work here
        
        //get the items for this pat checklist id
        CStatus status = new CStatus();
        DataSet dsChecklistItems = null;
        CVAPPCTCommData commData = new CVAPPCTCommData(data);
        status = commData.GetPatChecklistItemDS(PatientChecklistID,
                                                out dsChecklistItems);
        if (!status.Status)
        {
            //todo       
        }
        else
        {
            CCommunicator comm = new CCommunicator();

            //refresh the checklist items
            status = comm.RefreshPatientCheckList(conn,
                                                  data,
                                                  dsChecklistItems);
            if (!status.Status)
            {
                //todo:
            }
            else
            {
                //refresh the checklist collection items
                DataSet dsCLCollectionItems = null;
                status = commData.GetPatientCLCollectionItemDS(PatientChecklistID,
                                                               out dsCLCollectionItems);
                if (!status.Status)
                {
                    //todo:
                }
                else
                {
                    //refresh the checklist items
                    status = comm.RefreshPatientCheckList( conn,
                                                           data,
                                                           dsCLCollectionItems);
                    if (!status.Status)
                    {
                        //todo
                    }
                }
            }
        }

        //cleanup the database connection
        conn.Close();
        
        //signals we are done.
        Interlocked.Increment(ref ThreadCount);
        if (ThreadCount == ThreadMax)
        {
            if (eventX != null)
            {
                eventX.Set();
                ThreadCount = 0;
                ThreadMax = 0;
            }
        }
    }
}
