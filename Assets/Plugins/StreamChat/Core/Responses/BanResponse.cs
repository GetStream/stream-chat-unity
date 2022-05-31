using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class BanResponse : ResponseObjectBase, ILoadableFrom<BanResponseDTO, BanResponse>
    {
        public User BannedBy { get; set; }

        public Channel Channel { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public System.DateTimeOffset? Expires { get; set; }

        public string Reason { get; set; }

        public bool? Shadow { get; set; }

        public User User { get; set; }

        BanResponse ILoadableFrom<BanResponseDTO, BanResponse>.LoadFromDto(BanResponseDTO dto)
        {
            BannedBy = User.TryLoadFromDto<UserObjectDTO, User>(dto.BannedBy);
            Channel = Channel.TryLoadFromDto(dto.Channel);
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            Reason = dto.Reason;
            Shadow = dto.Shadow;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}