using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Sort;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Request to query polls
    /// </summary>
    public class StreamQueryPollsRequest : ISavableTo<QueryPollsRequestInternalDTO>
    {
        /// <summary>
        /// Filter conditions
        /// </summary>
        public IEnumerable<IFieldFilterRule> Filter { get; set; }

        /// <summary>
        /// Number of results to return
        /// </summary>
        public int? Limit { get; set; }

        public string Next { get; set; }

        public string Prev { get; set; }

        /// <summary>
        /// Sort parameters
        /// </summary>
        public PollsSortObject Sort { get; set; }

        QueryPollsRequestInternalDTO ISavableTo<QueryPollsRequestInternalDTO>.SaveToDto()
        {
            return new QueryPollsRequestInternalDTO
            {
                Filter = Filter?.Select(_ => _.GenerateFilterEntry()).ToDictionary(x => x.Key, x => x.Value),
                Limit = Limit,
                Next = Next,
                Prev = Prev,
                Sort = Sort?.ToSortParamRequestList()
            };
        }
    }
}

