using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Properties that pertain to a checklist/item decision state
/// user role entry
/// </summary>
public class CCLIDSEditDataItem
{
	public CCLIDSEditDataItem()
	{
	}

    public long ChecklistID { get; set; }
    public long ItemID { get; set; }
    public long UserRoleID { get; set; }
}
