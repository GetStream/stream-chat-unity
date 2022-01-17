using GetStreamIO.Core.DTO.Requests;

namespace Plugins.GetStreamIO.Core.Requests.V2
{
    public class ActionRequest : RequestObjectBase, ISavableTo<ActionRequestDTO>
    {
        public string Name { get; set; }

        public string Style { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public ActionRequestDTO SaveToDto() =>
            new ActionRequestDTO
            {
                Name = Name,
                Style = Style,
                Text = Text,
                Type = Type,
                Value = Value,
                AdditionalProperties = AdditionalProperties
            };
    }

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
        public double? Limit { get; set; }

        /// <summary>
        /// Number of members to limit
        /// </summary>
        public double? MemberLimit { get; set; }

        /// <summary>
        /// Number of messages to limit
        /// </summary>
        public double? MessageLimit { get; set; }

        /// <summary>
        /// Channel pagination offset
        /// </summary>
        public double? Offset { get; set; }

        public bool? Presence { get; set; }

        /// <summary>
        /// List of sort parameters
        /// </summary>
        public System.Collections.Generic.ICollection<SortParamRequest> Sort { get; set; } =
            new System.Collections.ObjectModel.Collection<SortParamRequest>();

        /// <summary>
        /// Whether to update channel state or not
        /// </summary>
        public bool? State { get; set; }

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
        public bool? Watch { get; set; }

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