using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class GuestResponse : ResponseObjectBase, ILoadableFrom<GuestResponseInternalDTO, GuestResponse>
    {
        /// <summary>
        /// Authentication token to use for guest user
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Created user object
        /// </summary>
        public User User { get; set; }

        GuestResponse ILoadableFrom<GuestResponseInternalDTO, GuestResponse>.LoadFromDto(GuestResponseInternalDTO dto)
        {
            AccessToken = dto.AccessToken;
            Duration = dto.Duration;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}