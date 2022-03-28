using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public partial class QueryChannelsRequest : RequestObjectBase, ISavableTo<QueryChannelsRequestDTO>
    {
        public System.Collections.Generic.IDictionary<string, object> FilterConditions { get; set; }

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
        public System.Collections.Generic.ICollection<SortParamRequest> Sort { get; set; } =
            new System.Collections.ObjectModel.Collection<SortParamRequest>();

        /// <summary>
        /// Whether to update channel state or not
        /// </summary>
        public bool? State { get; set; } = true;

        /// <summary>
        /// Whether to start watching found channels or not
        /// </summary>
        public bool? Watch { get; set; } = true;

        QueryChannelsRequestDTO ISavableTo<QueryChannelsRequestDTO>.SaveToDto() =>
            new QueryChannelsRequestDTO
            {
                FilterConditions = FilterConditions,
                Limit = Limit,
                MemberLimit = MemberLimit,
                MessageLimit = MessageLimit,
                Offset = Offset,
                Presence = Presence,
                Sort = Sort.TrySaveToDtoCollection<SortParamRequest, SortParamRequestDTO>(),
                State = State,
                Watch = Watch,
                AdditionalProperties = AdditionalProperties
            };
    }
}