using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to query poll votes
    /// </summary>
    public partial class QueryPollVotesRequest : RequestObjectBase, ISavableTo<QueryPollVotesRequestInternalDTO>
    {
        //StreamTODO: replace later with filter query builder
        public Dictionary<string, object> Filter { get; set; }

        public int? Limit { get; set; }

        public string Next { get; set; }

        public string Prev { get; set; }

        public List<SortParamRequest> Sort { get; set; }

        QueryPollVotesRequestInternalDTO ISavableTo<QueryPollVotesRequestInternalDTO>.SaveToDto()
            => new QueryPollVotesRequestInternalDTO
            {
                Filter = Filter,
                Limit = Limit,
                Next = Next,
                Prev = Prev,
                Sort = Sort.TrySaveToDtoCollection<SortParamRequest, SortParamRequestInternalDTO>(),
            };
    }
}

