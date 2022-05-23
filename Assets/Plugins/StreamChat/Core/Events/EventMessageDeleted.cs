using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public partial class EventMessageDeleted : EventBase, ILoadableFrom<EventMessageDeletedDTO, EventMessageDeleted>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public bool? HardDelete { get; set; }

        public Message Message { get; set; }

        public string Team { get; set; }

        public System.Collections.Generic.List<User> ThreadParticipants { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventMessageDeleted ILoadableFrom<EventMessageDeletedDTO, EventMessageDeleted>.LoadFromDto(EventMessageDeletedDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            HardDelete = dto.HardDelete;
            Message = Message.TryLoadFromDto(dto.Message);
            Team = dto.Team;
            ThreadParticipants = ThreadParticipants.TryLoadFromDtoCollection(dto.ThreadParticipants);
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}