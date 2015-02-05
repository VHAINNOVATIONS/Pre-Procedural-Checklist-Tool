using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Configuration;

//our data access library
using VAPPCT.DA;

//MDWS web service
using VAPPCT.Data.MDWSEmrSvc;

/// <summary>
/// CMDWSStatus class
/// </summary>
public class CMDWSStatus : CStatus
{
    /// <summary>
    /// US:838 US:840
    /// MDWS status class that derives from our CStatus class so we 
    /// handle MDWS errors in a consistent way
    /// </summary>
    /// <param name="fault"></param>
    public CMDWSStatus(FaultTO fault)
    {
        if (fault != null)
        {
            this.Status = false;
            this.StatusCode = k_STATUS_CODE.Failed;
            this.StatusComment = fault.message;
        }
    }
}