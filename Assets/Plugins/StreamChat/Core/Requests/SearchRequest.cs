﻿using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Requests
{
    public partial class SearchRequest : RequestObjectBase, ISavableTo<SearchRequestDTO>
    {
        /// <summary>
        /// Channel filter conditions
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> FilterConditions { get; set; } = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Number of messages to return
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Message filter conditions
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> MessageFilterConditions { get; set; }

        /// <summary>
        /// Pagination parameter. Cannot be used with non-zero offset.
        /// </summary>
        public string Next { get; set; }

        /// <summary>
        /// Pagination offset. Cannot be used with sort or next.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Search phrase
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Sort parameters. Cannot be used with non-zero offset
        /// </summary>
        public System.Collections.Generic.List<SortParam> Sort { get; set; }

        SearchRequestDTO ISavableTo<SearchRequestDTO>.SaveToDto() =>
            new SearchRequestDTO
            {
                FilterConditions = FilterConditions,
                Limit = Limit,
                MessageFilterConditions = MessageFilterConditions,
                Next = Next,
                Offset = Offset,
                Query = Query,
                Sort = Sort.TrySaveToDtoCollection<SortParam, SortParamDTO>(),
                AdditionalProperties = AdditionalProperties,
            };
    }
}