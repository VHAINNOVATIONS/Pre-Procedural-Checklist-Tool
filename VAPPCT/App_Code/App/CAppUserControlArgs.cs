using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Our data and ui libraries
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// Summary description for CAppUserControlArgs
/// </summary>
/// <summary>
/// Custom event arg for all of our user controls.
/// </summary>
public class CAppUserControlArgs : EventArgs
{
    //the event
    public k_EVENT Event { get; private set; }

    //status code for the event
    public k_STATUS_CODE StatusCode { get; private set; }

    //status comment for the event
    public string StatusComment { get; private set; }

    //data associated with the event,
    //ie. the id of the item we are updating etc.
    public string EventData { get; private set; }

    //constructor
    public CAppUserControlArgs(
        k_EVENT lEvent,
        k_STATUS_CODE lStatusCode,
        string strStatusComment,
        string strEventData)
    {
        Event = lEvent;
        StatusCode = lStatusCode;
        StatusComment = strStatusComment;
        EventData = strEventData;
    }
}