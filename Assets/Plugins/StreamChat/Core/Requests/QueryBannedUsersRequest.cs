using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Requests
{
    public partial class QueryBannedUsersRequest : RequestObjectBase, ISavableTo<QueryBannedUsersRequestDTO>
    {
        public System.DateTimeOffset? CreatedAtAfter { get; set; }

        public System.DateTimeOffset? CreatedAtAfterOrEqual { get; set; }

        public System.DateTimeOffset? CreatedAtBefore { get; set; }

        public System.DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }

        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; }

        public double? Limit { get; set; }

        public double? Offset { get; set; }

        public System.Collections.Generic.List<SortParam> Sort { get; set; }

        QueryBannedUsersRequestDTO ISavableTo<QueryBannedUsersRequestDTO>.SaveToDto()
        {
            return new QueryBannedUsersRequestDTO
            {
                CreatedAtAfter = CreatedAtAfter,
                CreatedAtAfterOrEqual = CreatedAtAfterOrEqual,
                CreatedAtBefore = CreatedAtBefore,
                CreatedAtBeforeOrEqual = CreatedAtBeforeOrEqual,
                FilterConditions = FilterConditions,
                Limit = Limit,
                Offset = Offset,
                Sort = Sort.TrySaveToDtoCollection<SortParam,SortParamDTO>(),
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}