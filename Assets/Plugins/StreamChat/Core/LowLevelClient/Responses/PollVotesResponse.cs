using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    /// <summary>
    /// Response containing poll votes
    /// </summary>
    public partial class PollVotesResponse : ResponseObjectBase, ILoadableFrom<PollVotesResponseInternalDTO, PollVotesResponse>
    {
        public List<PollVote> Votes { get; set; }

        public string Next { get; set; }

        public string Prev { get; set; }

        PollVotesResponse ILoadableFrom<PollVotesResponseInternalDTO, PollVotesResponse>.LoadFromDto(PollVotesResponseInternalDTO dto)
        {
            Votes = Votes.TryLoadFromDtoCollection(dto.Votes);
            Next = dto.Next;
            Prev = dto.Prev;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

