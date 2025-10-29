using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    /// <summary>
    /// Response containing a poll vote
    /// </summary>
    public partial class PollVoteResponse : ResponseObjectBase, ILoadableFrom<PollVoteResponseInternalDTO, PollVoteResponse>
    {
        public PollVote Vote { get; set; }

        PollVoteResponse ILoadableFrom<PollVoteResponseInternalDTO, PollVoteResponse>.LoadFromDto(PollVoteResponseInternalDTO dto)
        {
            Vote = Vote.TryLoadFromDto<PollVoteResponseDataInternalDTO, PollVote>(dto.Vote);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

