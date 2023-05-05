using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public partial class EventUserFlagged : EventBase, ILoadableFrom<UserFlaggedEventInternalDTO, EventUserFlagged>
    {
        public System.DateTimeOffset? CreatedAt { get; set; }

        public string TargetUser { get; set; }

        public System.Collections.Generic.List<string> TargetUsers { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventUserFlagged ILoadableFrom<UserFlaggedEventInternalDTO, EventUserFlagged>.LoadFromDto(UserFlaggedEventInternalDTO dto)
        {
            CreatedAt = dto.CreatedAt;
            TargetUser = dto.TargetUser;
            TargetUsers = dto.TargetUsers;
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}