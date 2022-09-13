﻿using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Requests
{
    public partial class QueryMessageFlagsRequest : RequestObjectBase, ISavableTo<QueryMessageFlagsRequestDTO>
    {
        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

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