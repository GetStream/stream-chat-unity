using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class Read : ModelBase, ILoadableFrom<ReadStateResponseInternalDTO, Read>
    {
        public System.DateTimeOffset? LastRead { get; set; }

        public int? UnreadMessages { get; set; }

        public User User { get; set; }

        Read ILoadableFrom<ReadStateResponseInternalDTO, Read>.LoadFromDto(ReadStateResponseInternalDTO dto)
        {
            LastRead = dto.LastRead;
            UnreadMessages = dto.UnreadMessages;
            User = User.TryLoadFromDto<UserResponseInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}