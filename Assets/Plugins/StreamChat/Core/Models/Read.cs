using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class Read : ModelBase, ILoadableFrom<ReadInternalDTO, Read>
    {
        public System.DateTimeOffset? LastRead { get; set; }

        public int? UnreadMessages { get; set; }

        public User User { get; set; }

        Read ILoadableFrom<ReadInternalDTO, Read>.LoadFromDto(ReadInternalDTO dto)
        {
            LastRead = dto.LastRead;
            UnreadMessages = dto.UnreadMessages;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}