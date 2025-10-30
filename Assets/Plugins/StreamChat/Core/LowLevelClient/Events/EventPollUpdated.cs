using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Event triggered when a poll is updated
    /// </summary>
    public class EventPollUpdated : EventBase, ILoadableFrom<PollUpdatedEventInternalDTO, EventPollUpdated>
    {
        public string Cid { get; set; }

        public string MessageId { get; set; }

        public Poll Poll { get; set; }

        public string Type { get; set; }

        EventPollUpdated ILoadableFrom<PollUpdatedEventInternalDTO, EventPollUpdated>.LoadFromDto(PollUpdatedEventInternalDTO dto)
        {
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            Poll = Poll.TryLoadFromDto<PollResponseDataInternalDTO, Poll>(dto.Poll);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

