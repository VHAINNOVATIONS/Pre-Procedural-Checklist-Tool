﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Properties that pertain to a TIU user role entry
/// </summary>
public class CCLTIUDataItem
{
    //Constructor
    public CCLTIUDataItem()
    {
    }

    private long m_lChecklistID;
    private long m_lUserRoleID;

    public long ChecklistID
    {
        get { return m_lChecklistID; }
        set { m_lChecklistID = value; }
    }

    public long UserRoleID
    {
        get { return m_lUserRoleID; }
        set { m_lUserRoleID = value; }
    }
}
