using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

/// <summary>
/// Summary description for CPatientItemCompList
/// </summary>
public class CPatientItemCompList : ArrayList
{
	public CPatientItemCompList()
	{
	}

    public override int Add(object value)
    {
        CPatientItemComponentDataItem di = value as CPatientItemComponentDataItem;
        if (di == null)
        {
            return -1;
        }
        return base.Add(value);
    }
}
