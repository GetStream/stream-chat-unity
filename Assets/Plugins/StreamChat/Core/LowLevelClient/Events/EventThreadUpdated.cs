using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.LowLevelClient.Models;

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

        /// <summary>
        /// The updated thread payload.
        /// </summary>
        public Thread Thread { get; set; }

        public string Type { get; set; }

        EventThreadUpdated ILoadableFrom<ThreadUpdatedEventInternalDTO, EventThreadUpdated>.LoadFromDto(
            ThreadUpdatedEventInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Custom = dto.Custom;
            Thread = Thread.TryLoadFromDto(dto.Thread);
            Type = dto.Type;
            AdditionalProperties = dto.AdditionalProperties;
            return this;
        }
    }
}
