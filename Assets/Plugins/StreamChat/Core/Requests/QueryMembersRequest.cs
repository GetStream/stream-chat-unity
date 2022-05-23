using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Requests
{
    public partial class QueryMembersRequest : RequestObjectBase, ISavableTo<QueryMembersRequestDTO>
    {
        public System.DateTimeOffset? CreatedAtAfter { get; set; }

        public System.DateTimeOffset? CreatedAtAfterOrEqual { get; set; }

        public System.DateTimeOffset? CreatedAtBefore { get; set; }

        public System.DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }

        /// <summary>
        /// Filter to apply to members
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; } = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Channel ID to interact with
        /// </summary>
        public string Id { get; set; }

        public double? Limit { get; set; }

        /// <summary>
        /// List of members to search in distinct channels
        /// </summary>
        public System.Collections.Generic.List<ChannelMember> Members { get; set; }

        public double? Offset { get; set; }

        /// <summary>
        /// Array of sort parameters
        /// </summary>
        public System.Collections.Generic.List<SortParam> Sort { get; set; }

        /// <summary>
        /// Channel type to interact with
        /// </summary>
        public string Type { get; set; }

        public string UserIdGt { get; set; }

        public string UserIdGte { get; set; }

        public string UserIdLt { get; set; }

        public string UserIdLte { get; set; }

        QueryMembersRequestDTO ISavableTo<QueryMembersRequestDTO>.SaveToDto() =>
            new QueryMembersRequestDTO
            {
                CreatedAtAfter = CreatedAtAfter,
                CreatedAtAfterOrEqual = CreatedAtAfterOrEqual,
                CreatedAtBefore = CreatedAtBefore,
                CreatedAtBeforeOrEqual = CreatedAtBeforeOrEqual,
                FilterConditions = FilterConditions,
                Id = Id,
                Limit = Limit,
                Members = Members.TrySaveToDtoCollection<ChannelMember, ChannelMemberDTO>(),
                Offset = Offset,
                Sort = Sort.TrySaveToDtoCollection<SortParam, SortParamDTO>(),
                Type = Type,
                UserIdGt = UserIdGt,
                UserIdGte = UserIdGte,
                UserIdLt = UserIdLt,
                UserIdLte = UserIdLte,
                AdditionalProperties = AdditionalProperties,
            };
    }
}