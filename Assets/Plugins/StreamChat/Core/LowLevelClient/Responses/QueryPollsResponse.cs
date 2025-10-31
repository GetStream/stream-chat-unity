using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    /// <summary>
    /// Response containing query polls results
    /// </summary>
    public partial class QueryPollsResponse : ResponseObjectBase, ILoadableFrom<QueryPollsResponseInternalDTO, QueryPollsResponse>
    {
        public List<Poll> Polls { get; set; }

        public string Next { get; set; }

        public string Prev { get; set; }

        QueryPollsResponse ILoadableFrom<QueryPollsResponseInternalDTO, QueryPollsResponse>.LoadFromDto(QueryPollsResponseInternalDTO dto)
        {
            Polls = Polls.TryLoadFromDtoCollection(dto.Polls);
            Next = dto.Next;
            Prev = dto.Prev;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

