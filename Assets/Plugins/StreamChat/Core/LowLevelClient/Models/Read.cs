using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class Read : ModelBase, ILoadableFrom<ReadInternalDTO, Read>
    {
        public System.DateTimeOffset? LastRead { get; private set; }
        
        public string LastReadMessageId { get; private set; }

        public int? UnreadMessages { get; private set; }

        public User User { get; private set; }

        Read ILoadableFrom<ReadInternalDTO, Read>.LoadFromDto(ReadInternalDTO dto)
        {
            LastRead = dto.LastRead;
            LastReadMessageId = dto.LastReadMessageId;
            UnreadMessages = dto.UnreadMessages;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}