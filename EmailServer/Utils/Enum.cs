using System.ComponentModel;

namespace EmailServer
{
    public enum AuthUserType
    {
        [Description("GlobalAdmin")]
        GlobalAdmin, // Can change everything
        [Description("ServerAdmin")]
        ServerAdmin  // Cylindex main API admin
    }

    public enum EmailStatus
    {
        InActive,
        Pending,
        Scheduled,
        Completed,
        Failed
    }

    public enum ActiveState
    {
        All,
        Active,
        Inactive
    }

    public enum ListReturnType
    {
        KVP,
        List
    }

    public enum ListSortDirection
    {
        ASC,
        DESC
    }

    public enum ListSortType
    {
        FromEmail,
        ToEmail,
        ScheduledDate
    }

}
