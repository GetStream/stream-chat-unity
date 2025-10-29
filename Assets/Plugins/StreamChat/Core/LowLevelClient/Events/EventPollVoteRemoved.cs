using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Event triggered when a poll vote is removed
    /// </summary>
    public class EventPollVoteRemoved : EventBase, ILoadableFrom<PollVoteRemovedEventInternalDTO, EventPollVoteRemoved>
    {
        public string Cid { get; set; }

        public string MessageId { get; set; }

        public Poll Poll { get; set; }

        public PollVote PollVote { get; set; }

        public string Type { get; set; }

        EventPollVoteRemoved ILoadableFrom<PollVoteRemovedEventInternalDTO, EventPollVoteRemoved>.LoadFromDto(PollVoteRemovedEventInternalDTO dto)
        {
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            Poll = Poll.TryLoadFromDto(dto.Poll);
            PollVote = PollVote.TryLoadFromDto(dto.PollVote);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

