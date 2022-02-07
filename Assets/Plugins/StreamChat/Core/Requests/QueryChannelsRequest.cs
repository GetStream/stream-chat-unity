using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public partial class QueryChannelsRequest : RequestObjectBase, ISavableTo<QueryChannelsRequestDTO>
    {
        /// <summary>
        /// Websocket connection ID to interact with. You can pass it as body or URL parameter
        /// </summary>
        public string ConnectionId { get; set; }

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
        /// **Server-side only**. User object which server acts upon
        /// </summary>
        public UserObjectRequest User { get; set; }

        /// <summary>
        /// **Server-side only**. User ID which server acts upon
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Whether to start watching found channels or not
        /// </summary>
        public bool? Watch { get; set; } = true;

        public QueryChannelsRequestDTO SaveToDto() =>
            new QueryChannelsRequestDTO
            {
                ConnectionId = ConnectionId,
                FilterConditions = FilterConditions,
                Limit = Limit,
                MemberLimit = MemberLimit,
                MessageLimit = MessageLimit,
                Offset = Offset,
                Presence = Presence,
                Sort = Sort.TrySaveToDtoCollection<SortParamRequest, SortParamRequestDTO>(),
                State = State,
                User = User.TrySaveToDto(),
                UserId = UserId,
                Watch = Watch,
                AdditionalProperties = AdditionalProperties
            };
    }
}