using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

//access app.config
using System.Configuration;

//timer
using System.Threading;

//MDWS web service
using VAPPCT.Data.MDWSEmrSvc;

//our libraries
using VAPPCT.DA;
using VAPPCT.Data;

namespace VAPPCTComm
{
    public partial class VAPPCTCommService : ServiceBase
    {
        /// <summary>
        /// US:834 our 1 and only timer
        /// </summary>
        private System.Threading.Timer m_Timer;

        /// <summary>
        /// US:834 Communicator service constructor
        /// </summary>
        public VAPPCTCommService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// US:834 class to handle timer events
        /// </summary>
        class CCommTimer
        {
            /// <summary>
            /// US:834 constructor
            /// </summary>
            public CCommTimer()
            {

            }

            /// <summary>
            /// US:834 method  called by the timer delegate when the timer fires
            /// </summary>
            /// <param name="stateInfo"></param>
            public void OnTimer(Object stateInfo)
            {
                CStatus status = new CStatus();
                CCommunicator com = new CCommunicator();
                status = com.ProcessOpenChecklistItems();
            }

        }

        /// <summary>
        /// US:834 the windows service was started
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //get the timer cycle (300000 milliseconds or 5 minutes)
            int nTimerCycle = Convert.ToInt32(ConfigurationSettings.AppSettings["TIMER_CYCLE"].ToString());
            
            //create a new timer, will run once at 5 seconds and then 
            //every 5 minutes...
            AutoResetEvent autoEvent  = new AutoResetEvent(false);
            CCommTimer commTimer = new CCommTimer();
            TimerCallback tcb = commTimer.OnTimer;
            m_Timer = new Timer( tcb, 
                                 autoEvent, 
                                 5000,
                                 nTimerCycle);
                
        }

        /// <summary>
        /// US:834 The communicator service was stopped
        /// </summary>
        protected override void OnStop()
        {
     
        }
    }
}
