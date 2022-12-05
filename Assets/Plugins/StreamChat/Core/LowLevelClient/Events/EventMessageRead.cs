using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    /// <summary>
    /// Trigger: when a channel is marked as read
    /// Recipients: clients watching the channel
    /// </summary>
    public partial class EventMessageRead : EventBase, ILoadableFrom<EventMessageReadInternalDTO, EventMessageRead>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string Team { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventMessageRead ILoadableFrom<EventMessageReadInternalDTO, EventMessageRead>.LoadFromDto(EventMessageReadInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Team = dto.Team;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}