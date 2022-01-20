using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public partial class Read : ModelBase, ILoadableFrom<ReadDTO, Read>
    {
        public System.DateTimeOffset? LastRead { get; set; }

        public double? UnreadMessages { get; set; }

        public User User { get; set; }

        public Read LoadFromDto(ReadDTO dto)
        {
            LastRead = dto.LastRead;
            UnreadMessages = dto.UnreadMessages;
            User = User.TryLoadFromDto(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}