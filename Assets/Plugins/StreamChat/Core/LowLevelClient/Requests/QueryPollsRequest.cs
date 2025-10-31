using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to query polls
    /// </summary>
    public partial class QueryPollsRequest : RequestObjectBase, ISavableTo<QueryPollsRequestInternalDTO>
    {
        public Dictionary<string, object> Filter { get; set; }

        public int? Limit { get; set; }

        public string Next { get; set; }

        public string Prev { get; set; }

        public List<SortParamRequest> Sort { get; set; }

        QueryPollsRequestInternalDTO ISavableTo<QueryPollsRequestInternalDTO>.SaveToDto()
            => new QueryPollsRequestInternalDTO
            {
                Filter = Filter,
                Limit = Limit,
                Next = Next,
                Prev = Prev,
                Sort = Sort.TrySaveToDtoCollection<SortParamRequest, SortParamRequestInternalDTO>(),
            };
    }
}

