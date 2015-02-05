using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

//access resource.resx
using System.Resources;

//our data access class library
using VAPPCT.DA;

public  class CChecklistPermissionsDataItem
{
    public bool ViewableAdministrator { get; set; }
    public bool ViewableDoctor { get; set; }
    public bool ViewableNurse { get; set; }

    public bool ReadOnlyAdministrator { get; set; }
    public bool ReadOnlyDoctor { get; set; }
    public bool ReadOnlyNurse { get; set; }

    public bool CloseableAdministrator { get; set; }
    public bool CloseableDoctor { get; set; }
    public bool CloseableNurse { get; set; }

    public bool TIUAdministrator { get; set; }
    public bool TIUDoctor { get; set; }
    public bool TIUNurse { get; set; }

    /// <summary>
    /// US:864 US:876 does the user have a specific checklist permission
    /// </summary>
    /// <param name="user"></param>
    /// <param name="kPermission"></param>
    /// <returns></returns>
    public bool HasPermission( CAppUser user,
                               k_CHECKLIST_PERMISSION kPermission)
    {
        //viewable permissions - most liberal role takes precident
        if (kPermission == k_CHECKLIST_PERMISSION.Viewable)
        {
            bool bViewable = false;

            if (user.IsDoctor)
            {
                if (ViewableDoctor)
                {
                    bViewable = true;
                }
            }
            if (user.IsNurse)
            {
                if (ViewableNurse)
                {
                    bViewable = true;
                }
            }
            if (user.IsAdministrator)
            {
                if (ViewableAdministrator)
                {
                   bViewable = true;
                }
            }

            return bViewable;
        }

        //readonly permissions - most liberal takes precidence
        if (kPermission == k_CHECKLIST_PERMISSION.ReadOnly)
        {
            bool bReadOnly = true;

            if (user.IsDoctor)
            {
                if (!ReadOnlyDoctor)
                {
                    bReadOnly = false;
                }
            }
            if (user.IsNurse)
            {
                if (!ReadOnlyNurse)
                {
                    bReadOnly = false;
                }
            }
            if (user.IsAdministrator)
            {
                if (!ReadOnlyAdministrator)
                {
                    bReadOnly = false;
                }
            }

            return bReadOnly;
        }

        //closeable permissions - most liberal role takes precident
        if (kPermission == k_CHECKLIST_PERMISSION.Closeable)
        {
            bool bCloseable = false;

            if (user.IsDoctor)
            {
                if (CloseableDoctor)
                {
                    bCloseable = true;
                }
            }
            if (user.IsNurse)
            {
                if (CloseableNurse)
                {
                    bCloseable = true;
                }
            }
            if (user.IsAdministrator)
            {
                if (CloseableAdministrator)
                {
                    bCloseable = true;
                }
            }

            return bCloseable;
        }

        //TIU Note permissions - most liberal role takes precident
        if (kPermission == k_CHECKLIST_PERMISSION.TIUNote)
        {
            bool bTIUNote = false;

            if (user.IsDoctor)
            {
                if (TIUDoctor)
                {
                    bTIUNote = true;
                }
            }
            if (user.IsNurse)
            {
                if (TIUNurse)
                {
                    bTIUNote = true;
                }
            }
            if (user.IsAdministrator)
            {
                if (TIUAdministrator)
                {
                    bTIUNote = true;
                }
            }

            return bTIUNote;
        }

        
        return false;
    }

    /// <summary>
    /// US:864 US:876 constructor sets all permissions to false to start with
    /// </summary>
    public CChecklistPermissionsDataItem()
    {    
        ViewableAdministrator = false;
        ViewableDoctor = false;
        ViewableNurse = false;
        
        ReadOnlyAdministrator = false;
        ReadOnlyDoctor = false;
        ReadOnlyNurse = false;
        
        CloseableAdministrator = false;
        CloseableDoctor = false;
        CloseableNurse = false;

        TIUAdministrator = false;
        TIUDoctor = false;
        TIUNurse = false;
    }

    /// <summary>
    /// US:864 US:876 loads the data item from datasets
    /// </summary>
    /// <param name="dsViewable"></param>
    /// <param name="dsReadOnly"></param>
    /// <param name="DsCloseable"></param>
    public CStatus LoadRoles(DataSet dsViewable,
                             DataSet dsReadOnly,
                             DataSet DsCloseable,
                             DataSet dsTIU)
    {
        CStatus status = new CStatus();

        status = LoadViewableRoles(dsViewable);
        status = LoadReadOnlyRoles(dsReadOnly);
        status = LoadCloseableRoles(DsCloseable);
        status = LoadTIURoles(dsTIU);

        return status;
    }

    /// <summary>
    /// US:864 US:876 loads viewable properties from the dataset
    /// </summary>
    /// <param name="dsViewable"></param>
    protected CStatus LoadViewableRoles(DataSet dsViewable)
    {
        CStatus status = new CStatus();

        if (CDataUtils.IsEmpty(dsViewable))
        {
            return status;
        }

        foreach (DataTable table in dsViewable.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                long lRoleID = CDataUtils.GetDSLongValue(dr, "USER_ROLE_ID");
                if (lRoleID == (long)k_USER_ROLE_ID.Administrator)
                {
                    ViewableAdministrator = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Doctor)
                {
                    ViewableDoctor = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Nurse)
                {
                    ViewableNurse = true;
                }
            }
        }

        return status;
    }

    /// <summary>
    /// US:864 US:876 load tiu roles
    /// </summary>
    /// <param name="dsTIU"></param>
    /// <returns></returns>
    protected CStatus LoadTIURoles(DataSet dsTIU)
    {
        CStatus status = new CStatus();

        if (CDataUtils.IsEmpty(dsTIU))
        {
            return status;
        }

        foreach (DataTable table in dsTIU.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                long lRoleID = CDataUtils.GetDSLongValue(dr, "USER_ROLE_ID");
                if (lRoleID == (long)k_USER_ROLE_ID.Administrator)
                {
                    TIUAdministrator = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Doctor)
                {
                    TIUDoctor = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Nurse)
                {
                    TIUNurse = true;
                }
            }
        }

        return status;
    }

    /// <summary>
    /// US:864 US:876 loads readonly properties from the dataset
    /// </summary>
    /// <param name="dsViewable"></param>
    protected CStatus LoadReadOnlyRoles(DataSet dsReadOnly)
    {
        CStatus status = new CStatus();

        if (CDataUtils.IsEmpty(dsReadOnly))
        {
            return status;
        }

        foreach (DataTable table in dsReadOnly.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                long lRoleID = CDataUtils.GetDSLongValue(dr, "USER_ROLE_ID");
                if (lRoleID == (long)k_USER_ROLE_ID.Administrator)
                {
                    ReadOnlyAdministrator = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Doctor)
                {
                    ReadOnlyDoctor = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Nurse)
                {
                    ReadOnlyNurse = true;
                }
            }
        }

        return status;
    }

    /// <summary>
    /// US:864 US:876 loads closeable properties from the dataset
    /// </summary>
    /// <param name="dsViewable"></param>
    protected CStatus LoadCloseableRoles(DataSet dsCloseable)
    {
        CStatus status = new CStatus();

        if (CDataUtils.IsEmpty(dsCloseable))
        {
            return status;
        }

        foreach (DataTable table in dsCloseable.Tables)
        {
            foreach (DataRow dr in table.Rows)
            {
                long lRoleID = CDataUtils.GetDSLongValue(dr, "USER_ROLE_ID");
                if (lRoleID == (long)k_USER_ROLE_ID.Administrator)
                {
                    CloseableAdministrator = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Doctor)
                {
                    CloseableDoctor = true;
                }
                else if (lRoleID == (long)k_USER_ROLE_ID.Nurse)
                {
                    CloseableNurse = true;
                }
            }
        }

        return status;
    }

}
