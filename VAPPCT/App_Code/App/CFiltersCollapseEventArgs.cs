using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CFiltersCollapseEventArgs
/// </summary>
public class CFiltersCollapseEventArgs : EventArgs
{
    public bool Collapsed { get; private set; }
	public CFiltersCollapseEventArgs(bool bCollapsed)
	{
        Collapsed = bCollapsed;
	}
}
