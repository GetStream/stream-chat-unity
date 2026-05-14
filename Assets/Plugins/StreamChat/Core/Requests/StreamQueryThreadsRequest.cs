using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Sort;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Request to query threads using <see cref="IStreamThreadsApi.QueryThreadsAsync"/>
    /// </summary>
    public class StreamQueryThreadsRequest : ISavableTo<QueryThreadsRequestInternalDTO>
    {
        /// <summary>
        /// Filter conditions
        /// </summary>
        public IEnumerable<IFieldFilterRule> Filter { get; set; }

        /// <summary>
        /// Sort parameters
        /// </summary>
        public ThreadsSortObject Sort { get; set; }

        /// <summary>
        /// Number of results per page
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Number of replies returned per thread
        /// </summary>
        public int? ReplyLimit { get; set; }

        /// <summary>
        /// Number of participants returned per thread
        /// </summary>
        public int? ParticipantLimit { get; set; }

        /// <summary>
        /// Number of channel members included with each thread's channel
        /// </summary>
        public int? MemberLimit { get; set; }

        /// <summary>
        /// Pagination cursor for the next page
        /// </summary>
        public string Next { get; set; }

        /// <summary>
        /// Pagination cursor for the previous page
        /// </summary>
        public string Prev { get; set; }

        /// <summary>
        /// Whether to start watching the channels each returned thread belongs to
        /// </summary>
        public bool? Watch { get; set; }

        QueryThreadsRequestInternalDTO ISavableTo<QueryThreadsRequestInternalDTO>.SaveToDto()
        {
            return new QueryThreadsRequestInternalDTO
            {
                Filter = Filter?.Select(_ => _.GenerateFilterEntry()).ToDictionary(x => x.Key, x => x.Value),
                Sort = Sort?.ToSortParamRequestList(),
                Limit = Limit,
                ReplyLimit = ReplyLimit,
                ParticipantLimit = ParticipantLimit,
                MemberLimit = MemberLimit,
                Next = Next,
                Prev = Prev,
                Watch = Watch,
            };
        }
    }
}
