using GetStreamIO.Core.DTO.Events;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Events.V2
{
    public partial class EventMessageNew : EventBase, ILoadableFrom<EventMessageNewDTO, EventMessageNew>
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

        public double? WatcherCount { get; set; }

        public EventMessageNew LoadFromDto(EventMessageNewDTO dto)
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
            WatcherCount = dto.WatcherCount;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}