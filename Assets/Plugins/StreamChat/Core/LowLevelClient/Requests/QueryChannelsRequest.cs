using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class QueryChannelsRequest : RequestObjectBase, ISavableTo<QueryChannelsRequestInternalDTO>
    {
        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; }

        /// <summary>
        /// Number of channels to limit
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Number of members to limit
        /// </summary>
        public int? MemberLimit { get; set; }

        /// <summary>
        /// Number of messages to limit
        /// </summary>
        public int? MessageLimit { get; set; }

        /// <summary>
        /// Channel pagination offset
        /// </summary>
        public int? Offset { get; set; }

        public bool? Presence { get; set; }

        /// <summary>
        /// List of sort parameters
        /// </summary>
        public System.Collections.Generic.List<SortParamRequest> Sort { get; set; } =
            new System.Collections.Generic.List<SortParamRequest>();

        /// <summary>
        /// Whether to update channel state or not
        /// </summary>
        public bool? State { get; set; } = true;

        /// <summary>
        /// Whether to start watching found channels or not
        /// </summary>
        public bool? Watch { get; set; } = true;

        QueryChannelsRequestInternalDTO ISavableTo<QueryChannelsRequestInternalDTO>.SaveToDto() =>
            new QueryChannelsRequestInternalDTO
            {
                FilterConditions = FilterConditions,
                Limit = Limit,
                MemberLimit = MemberLimit,
                MessageLimit = MessageLimit,
                Offset = Offset,
                Presence = Presence,
                Sort = Sort.TrySaveToDtoCollection<SortParamRequest, SortParamRequestInternalDTO>(),
                State = State,
                Watch = Watch,
                AdditionalProperties = AdditionalProperties
            };
    }
}