using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Event triggered when a poll vote is changed
    /// </summary>
    public class EventPollVoteChanged : EventBase, ILoadableFrom<PollVoteChangedEventInternalDTO, EventPollVoteChanged>
    {
        public string Cid { get; set; }

        public string MessageId { get; set; }

        public Poll Poll { get; set; }

        public PollVote PollVote { get; set; }

        public string Type { get; set; }

        EventPollVoteChanged ILoadableFrom<PollVoteChangedEventInternalDTO, EventPollVoteChanged>.LoadFromDto(PollVoteChangedEventInternalDTO dto)
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

