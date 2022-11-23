using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Request body to query banned users with <see cref="IStreamChatClient.QueryBannedUsersAsync"/>.
    /// Fill only the parameters you need
    /// </summary>
    public sealed class StreamQueryBannedUsersRequest : ISavableTo<QueryBannedUsersRequestInternalDTO>
    {
        /// <summary>
        /// Ban created after this date
        /// </summary>
        public DateTimeOffset? CreatedAtAfter { get; set; }

        /// <summary>
        /// Ban created after or equal this date
        /// </summary>
        public DateTimeOffset? CreatedAtAfterOrEqual { get; set; }

        /// <summary>
        /// Ban created before this date
        /// </summary>
        public DateTimeOffset? CreatedAtBefore { get; set; }

        /// <summary>
        /// Ban created before or equal this date
        /// </summary>
        public DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }

        /// <summary>
        /// Filter conditions
        /// </summary>
        public Dictionary<string, object> FilterConditions { get; set; }

        /// <summary>
        /// How many results to return
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// How many results to skip
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Sort
        /// </summary>
        public List<StreamSortParam> Sort { get; set; }

        QueryBannedUsersRequestInternalDTO ISavableTo<QueryBannedUsersRequestInternalDTO>.SaveToDto()
            => new QueryBannedUsersRequestInternalDTO
            {
                CreatedAtAfter = CreatedAtAfter,
                CreatedAtAfterOrEqual = CreatedAtAfterOrEqual,
                CreatedAtBefore = CreatedAtBefore,
                CreatedAtBeforeOrEqual = CreatedAtBeforeOrEqual,
                FilterConditions = FilterConditions,
                Limit = Limit,
                Offset = Offset,
                Sort = Sort.TrySaveToDtoCollection<StreamSortParam, SortParamInternalDTO>(),
            };
    }
}