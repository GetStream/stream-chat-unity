using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class Read : ModelBase, ILoadableFrom<ReadDTO, Read>
    {
        public System.DateTimeOffset? LastRead { get; set; }

        public double? UnreadMessages { get; set; }

        public User User { get; set; }

        Read ILoadableFrom<ReadDTO, Read>.LoadFromDto(ReadDTO dto)
        {
            LastRead = dto.LastRead;
            UnreadMessages = dto.UnreadMessages;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}