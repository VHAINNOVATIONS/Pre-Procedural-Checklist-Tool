using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// US:1945 item type enum for labs, questions etc...
/// </summary>
public enum k_ITEM_TYPE_ID : long
{
    Laboratory = 1,
    QuestionSelection = 2,
    QuestionFreeText = 3,
    Collection = 4,
    NoteTitle = 5,
};

/// <summary>
/// user role enum for permissions
/// </summary>
public enum k_CHECKLIST_PERMISSION : long
{
    Viewable = 1,
    ReadOnly = 2,
    Closeable = 3,
    TIUNote = 4,
    DSOverride = 5,
};

/// <summary>
/// user role enum for permissions
/// </summary>
public enum k_USER_ROLE_ID : long
{
    Administrator = 1,
    Nurse = 2,
    Doctor = 3,
   
};

/// <summary>
/// active enum, active, inactive all
/// </summary>
public enum k_ACTIVE_ID : long
{ 
    Active = 1,
    Inactive = 2,
    All = 3,
};

/// <summary>
/// checklist state enum open, closed, cancelled etc...
/// </summary>
public enum k_CHECKLIST_STATE_ID : long
{
    Null = 0,
    Open = 1,
    Closed = 2,
    Cancelled = 3,
};

/// <summary>
/// state id enum, not selected, good, bad, unknown etc..
/// </summary>
public enum k_STATE_ID : long
{
    NotSelected = 1,
    Good = 2,
    Unknown = 3,
    Bad = 4,
};

/// <summary>
/// source type enum vista, vappct etc...
/// </summary>
public enum k_SOURCE_TYPE_ID : long
{
    VistA = 1,
    VAPPCT = 2,
}

/// <summary>
/// time unit enum day, hour, min etc...
/// </summary>
public enum k_TIME_UNIT_ID : long
{
    Day = 1,
    Hour = 2,
    Minute = 3,
}

/// <summary>
/// stat true false ids
/// True = 1; False = 2;
/// </summary>
public enum k_TRUE_FALSE_ID : long
{
    True = 1,
    False = 2,
}

/// <summary>
/// default state ids for items
/// Unknown = 1; Good = 2; Bad = 3;
/// </summary>
public enum k_DEFAULT_STATE_ID : long
{
    Unknown = 1,
    Good = 2,
    Bad = 3,
}

public enum k_MULTI_PAT_THREAD_TYPE : long
{
    Unknown = 1,
    Refresh = 2,
    Logic = 3,
}