using GetStreamIO.Core.DTO.Events;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Events.V2
{
    public partial class EventMemberUpdated : EventBase, ILoadableFrom<EventMemberUpdatedDTO, EventMemberUpdated>
    {
        public string ChannelId { get; set; }

        public string ChannelType { get; set; }

        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public ChannelMember Member { get; set; }

        public string Team { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        public EventMemberUpdated LoadFromDto(EventMemberUpdatedDTO dto)
        {
            ChannelId = dto.ChannelId;
            ChannelType = dto.ChannelType;
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Member = Member.TryLoadFromDto(dto.Member);
            Team = dto.Team;
            Type = dto.Type;
            User = User.TryLoadFromDto(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}