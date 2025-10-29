using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Event triggered when a poll is closed
    /// </summary>
    public class EventPollClosed : EventBase, ILoadableFrom<PollClosedEventInternalDTO, EventPollClosed>
    {
        public string Cid { get; set; }

        public string MessageId { get; set; }

        public Poll Poll { get; set; }

        public string Type { get; set; }

        EventPollClosed ILoadableFrom<PollClosedEventInternalDTO, EventPollClosed>.LoadFromDto(PollClosedEventInternalDTO dto)
        {
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            Poll = Poll.TryLoadFromDto(dto.Poll);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

