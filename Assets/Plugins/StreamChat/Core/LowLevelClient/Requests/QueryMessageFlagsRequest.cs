using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class QueryMessageFlagsRequest : RequestObjectBase, ISavableTo<QueryMessageFlagsRequestInternalDTO>
    {
        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        QueryMessageFlagsRequestInternalDTO ISavableTo<QueryMessageFlagsRequestInternalDTO>.SaveToDto() =>
            new QueryMessageFlagsRequestInternalDTO
            {
                FilterConditions = FilterConditions,
                Limit = Limit,
                Offset = Offset,
                AdditionalProperties = AdditionalProperties,
            };
    }
}