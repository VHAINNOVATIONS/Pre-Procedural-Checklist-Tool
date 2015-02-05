using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

public class CChecklistItemPermissionsDataItem
{
    public bool DSOverrideAdministrator { get; set; }
    public bool DSOverrideDoctor { get; set; }
    public bool DSOverrideNurse { get; set; }

    /// <summary>
    /// US:865 does the user have a specific checklist item permission
    /// </summary>
    /// <param name="user"></param>
    /// <param name="kPermission"></param>
    /// <returns></returns>
    public bool HasPermission(CAppUser user,
                               k_CHECKLIST_PERMISSION kPermission)
    {
        //viewable permissions - most liberal role takes precident
        if (kPermission == k_CHECKLIST_PERMISSION.DSOverride)
        {
            bool bOverride = false;

            if (user.IsAdministrator)
            {
                if (DSOverrideAdministrator)
                {
                    bOverride = true;
                }
            }
            if (user.IsDoctor)
            {
                if (DSOverrideDoctor)
                {
                    bOverride = true;
                }
            }
            if (user.IsNurse)
            {
                if (DSOverrideNurse)
                {
                    bOverride = true;
                }
            }

            return bOverride;
        }

        return false;
    }

    /// <summary>
    /// US:865 constructor sets all permissions to false to start with
    /// </summary>
    public CChecklistItemPermissionsDataItem()
    {
        DSOverrideAdministrator = false;
        DSOverrideDoctor = false;
        DSOverrideNurse = false;
    }

    /// <summary>
    /// US:865 loads the data item from datasets
    /// </summary>
    /// <param name="dsViewable"></param>
    /// <param name="dsReadOnly"></param>
    /// <param name="DsCloseable"></param>
    public CStatus LoadRoles(DataSet dsDSOverride)
    {
        CStatus status = new CStatus();

        status = LoadDSOverrideRoles(dsDSOverride);
       
        return status;
    }

    /// <summary>
    /// US:865 loads override properties from the dataset
    /// </summary>
    /// <param name="dsDSOverride"></param>
    protected CStatus LoadDSOverrideRoles(DataSet dsDSOverride)
    {
        CStatus status = new CStatus();

        if (CDataUtils.IsEmpty(dsDSOverride))
        {
            return status;
        }

        foreach (DataTable table in dsDSOverride.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                long lRoleID = CDataUtils.GetDSLongValue(dr, "USER_ROLE_ID");
                if (lRoleID == (long)k_USER_ROLE_ID.Administrator)
                {
                    DSOverrideAdministrator = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Doctor)
                {
                    DSOverrideDoctor = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Nurse)
                {
                    DSOverrideNurse = true;
                }
            }
        }

        return status;
    }
}
