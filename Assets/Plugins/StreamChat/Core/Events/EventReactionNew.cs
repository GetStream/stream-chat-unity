using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Events
{
    public class EventReactionNew : EventBase, ILoadableFrom<EventReactionNewDTO, EventReactionNew>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public Message Message { get; set; }

        public Reaction Reaction { get; set; }

        public string Team { get; set; }

        public System.Collections.Generic.ICollection<User> ThreadParticipants { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventReactionNew ILoadableFrom<EventReactionNewDTO, EventReactionNew>.LoadFromDto(EventReactionNewDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Message = Message.TryLoadFromDto(dto.Message);
            Reaction = Reaction.TryLoadFromDto(dto.Reaction);
            Team = dto.Team;
            ThreadParticipants = ThreadParticipants.TryLoadFromDtoCollection(dto.ThreadParticipants);
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}