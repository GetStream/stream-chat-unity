using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class BanResponse : ResponseObjectBase, ILoadableFrom<BanResponseInternalDTO, BanResponse>
    {
        public User BannedBy { get; set; }

        public Channel Channel { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public System.DateTimeOffset? Expires { get; set; }

        public string Reason { get; set; }

        public bool? Shadow { get; set; }

        public User User { get; set; }

        BanResponse ILoadableFrom<BanResponseInternalDTO, BanResponse>.LoadFromDto(BanResponseInternalDTO dto)
        {
            BannedBy = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.BannedBy);
            Channel = Channel.TryLoadFromDto(dto.Channel);
            CreatedAt = dto.CreatedAt;
            Expires = dto.Expires;
            Reason = dto.Reason;
            Shadow = dto.Shadow;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}