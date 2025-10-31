using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Event triggered when a poll vote is casted
    /// </summary>
    public class EventPollVoteCasted : EventBase, ILoadableFrom<PollVoteCastedEventInternalDTO, EventPollVoteCasted>
    {
        public string Cid { get; set; }

        public string MessageId { get; set; }

        public Poll Poll { get; set; }

        public PollVote PollVote { get; set; }

        public string Type { get; set; }

        EventPollVoteCasted ILoadableFrom<PollVoteCastedEventInternalDTO, EventPollVoteCasted>.LoadFromDto(PollVoteCastedEventInternalDTO dto)
        {
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            Poll = Poll.TryLoadFromDto<PollResponseDataInternalDTO, Poll>(dto.Poll);
            PollVote = PollVote.TryLoadFromDto<PollVoteResponseDataInternalDTO, PollVote>(dto.PollVote);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

