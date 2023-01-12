using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class QueryMembersRequest : RequestObjectBase, ISavableTo<QueryMembersRequestInternalDTO>
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

        /// <summary>
        /// Number of records to return
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// List of members to search in distinct channels
        /// </summary>
        public System.Collections.Generic.List<ChannelMember> Members { get; set; }

        /// <summary>
        /// Number of records to offset
        /// </summary>
        public int? Offset { get; set; }

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

        QueryMembersRequestInternalDTO ISavableTo<QueryMembersRequestInternalDTO>.SaveToDto() =>
            new QueryMembersRequestInternalDTO
            {
                CreatedAtAfter = CreatedAtAfter,
                CreatedAtAfterOrEqual = CreatedAtAfterOrEqual,
                CreatedAtBefore = CreatedAtBefore,
                CreatedAtBeforeOrEqual = CreatedAtBeforeOrEqual,
                FilterConditions = FilterConditions,
                Id = Id,
                Limit = Limit,
                Members = Members.TrySaveToDtoCollection<ChannelMember, ChannelMemberInternalDTO>(),
                Offset = Offset,
                Sort = Sort.TrySaveToDtoCollection<SortParam, SortParamInternalDTO>(),
                Type = Type,
                UserIdGt = UserIdGt,
                UserIdGte = UserIdGte,
                UserIdLt = UserIdLt,
                UserIdLte = UserIdLte,
                AdditionalProperties = AdditionalProperties,
            };
    }
}