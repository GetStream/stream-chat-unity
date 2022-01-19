using GetStreamIO.Core.DTO.Events;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Events
{
    public partial class EventMessageUpdated : EventBase, ILoadableFrom<EventMessageUpdatedDTO, EventMessageUpdated>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public Message Message { get; set; }

        public string Team { get; set; }

        public System.Collections.Generic.ICollection<User> ThreadParticipants { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        public EventMessageUpdated LoadFromDto(EventMessageUpdatedDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Message = Message.TryLoadFromDto(dto.Message);
            Team = dto.Team;
            ThreadParticipants = ThreadParticipants.TryLoadFromDtoCollection(dto.ThreadParticipants);
            Type = dto.Type;
            User = User.TryLoadFromDto(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}