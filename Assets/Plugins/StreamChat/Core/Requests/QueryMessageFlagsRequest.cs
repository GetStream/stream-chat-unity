using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Requests
{
    public partial class QueryMessageFlagsRequest : RequestObjectBase, ISavableTo<QueryMessageFlagsRequestDTO>
    {
        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; }

        public double? Limit { get; set; }

        public double? Offset { get; set; }

        QueryMessageFlagsRequestDTO ISavableTo<QueryMessageFlagsRequestDTO>.SaveToDto() =>
            new QueryMessageFlagsRequestDTO
            {
                FilterConditions = FilterConditions,
                Limit = Limit,
                Offset = Offset,
                AdditionalProperties = AdditionalProperties,
            };
    }
}