using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Events;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Trigger: when a thread (parent message) was updated. For example its title or custom data was changed.
    /// </summary>
    public partial class EventThreadUpdated : EventBase,
        ILoadableFrom<ThreadUpdatedEventInternalDTO, EventThreadUpdated>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public Dictionary<string, object> Custom { get; set; }

        public string Type { get; set; }

        EventThreadUpdated ILoadableFrom<ThreadUpdatedEventInternalDTO, EventThreadUpdated>.LoadFromDto(
            ThreadUpdatedEventInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Custom = dto.Custom;
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;
            return this;
        }
    }
}
