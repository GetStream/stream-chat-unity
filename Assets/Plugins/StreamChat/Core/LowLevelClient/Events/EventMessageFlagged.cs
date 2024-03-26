using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Events
{
    public partial class EventMessageFlagged : EventBase, ILoadableFrom<MessageFlaggedEventInternalDTO, EventMessageFlagged>
    {
        public string Cid { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public Flag Flag { get; set; }

        public Message Message { get; set; }

        public System.Collections.Generic.List<User> ThreadParticipants { get; set; }

        public string Type { get; set; }

        public User User { get; set; }

        EventMessageFlagged ILoadableFrom<MessageFlaggedEventInternalDTO, EventMessageFlagged>.LoadFromDto(MessageFlaggedEventInternalDTO dto)
        {
            Cid = dto.Cid;
            CreatedAt = dto.CreatedAt;
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            Flag = Flag.TryLoadFromDto(dto.Flag);
            ThreadParticipants = ThreadParticipants.TryLoadFromDtoCollection(dto.ThreadParticipants);
            Type = dto.Type;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}