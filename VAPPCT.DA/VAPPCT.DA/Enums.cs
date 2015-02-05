using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAPPCT.DA
{
    /// <summary>
    /// This class is used to hols public enumerations related to data access
    /// </summary>
    class Enums
    {
    }
    
    /// <summary>
    /// enumeration of paramater types
    /// </summary>
    public enum k_DATA_PARAMETER_TYPE : int
    {
        StringParameter = 1,
        LongParameter = 2,
        DateParameter = 3,
        IntParameter = 4,
        BoolParameter = 5,
        DoubleParameter = 6,
        CLOBParameter = 7,
    };

    /// <summary>
    /// enumeration of status codes
    /// </summary>
    public enum k_STATUS_CODE : long
    {
        Success = 1,
        Failed = 0,
        Unknown = 2,
        NothingToDo = 3,
    };

    /// <summary>
    /// enumeration of edit modes
    /// </summary>
    public enum k_EDIT_MODE : long
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
        READ_ONLY = 3,
        REPORT = 4,
        UNKNOWN = 5,
        INITIALIZE = 6,
    };

    /// <summary>
    /// enumeration of events
    /// </summary>
    public enum k_EVENT : long
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
        ROLLBACK = 4,
        CANCEL = 5,
        DEFAULT = 6,
        SELECT = 7,
    };

    /// <summary>
    /// enumeration of active/inactive
    /// </summary>
    public enum k_ACTIVE : long
    {
        ACTIVE = 1,
        INACTIVE = 2,
    };

    /// <summary>
    /// enumeration for sex
    /// </summary>
    public enum k_SEX : long
    {
        MALE = 1,
        FEMALE = 2,
        UNKNOWN = 3,
    };

    /// <summary>
    /// enumeration for comparisons
    /// </summary>
    public enum k_COMPARE : long
    {
        INVALID = -1,
        EQUALTO = 0,
        GREATERTHAN = 1,
        LESSTHAN = 2,
    }
}
