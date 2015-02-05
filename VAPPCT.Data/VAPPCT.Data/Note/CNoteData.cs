using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

/// <summary>
/// Summary description for CNoteTitleData
/// </summary>
public class CNoteData: CData
{
    public CNoteData(CData data)
        : base(data)
    {
        //constructors are not inherited in c#!
    }
}
