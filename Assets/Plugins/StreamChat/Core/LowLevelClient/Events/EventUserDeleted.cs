using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public sealed class EventUserDeleted : EventBase,
        ILoadableFrom<UserDeletedEventInternalDTO, EventUserDeleted>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public bool? DeleteConversationChannels { get; set; }

        public bool? HardDelete { get; set; }

        public bool? MarkMessagesDeleted { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserDeleted ILoadableFrom<UserDeletedEventInternalDTO, EventUserDeleted>.LoadFromDto(
            UserDeletedEventInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            DeleteConversationChannels = dto.DeleteConversationChannels;
            HardDelete = dto.HardDelete;
            MarkMessagesDeleted = dto.MarkMessagesDeleted;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}