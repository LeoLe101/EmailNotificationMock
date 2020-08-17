using System;
using System.ComponentModel.DataAnnotations;

namespace EmailServer
{
    public class EmailRequest
    {
        public string InspectorName { get; set; }

        [Required, EmailAddress]
        public string InspectorEmail { get; set; }

        public string ReceiverName { get; set; }

        [Required, EmailAddress]
        public string ReceiverEmail { get; set; }

        [Required]
        public DateTime ScheduledTime { get; set; }
    }

    public class GetListRequest
    {
        public string Search { get; set; }

        public string CompanyID { get; set; }

        public ActiveState ActiveState { get; set; }

        public ListReturnType ReturnType { get; set; }

        public ListSortType SortType { get; set; }

        public ListSortDirection SortDirection { get; set; }

        public static GetListRequest Standardize(GetListRequest entity)
        {
            return new GetListRequest
            {
                Search = entity.Search,

                CompanyID = !String.IsNullOrWhiteSpace(entity.CompanyID) ?
                    entity.CompanyID.Trim() :
                    null,

                ActiveState = ((entity?.ActiveState != null) && Enum.IsDefined(typeof(ActiveState), entity?.ActiveState)) ?
                    entity.ActiveState :
                    ActiveState.All,

                ReturnType = ((entity?.ReturnType != null) && Enum.IsDefined(typeof(ListReturnType), entity?.ReturnType)) ?
                    entity.ReturnType :
                    ListReturnType.List,

                SortType = ((entity?.SortType != null) && Enum.IsDefined(typeof(ListSortType), entity?.SortType)) ?
                    entity.SortType :
                    ListSortType.ScheduledDate,

                SortDirection = ((entity?.SortDirection != null) && Enum.IsDefined(typeof(ListSortDirection), entity?.SortDirection)) ?
                    entity.SortDirection :
                    ListSortDirection.DESC,
            };
        }
    }
}