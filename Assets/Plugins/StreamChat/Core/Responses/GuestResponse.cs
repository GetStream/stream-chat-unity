using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class GuestResponse : ResponseObjectBase, ILoadableFrom<GuestResponseDTO, GuestResponse>
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

        GuestResponse ILoadableFrom<GuestResponseDTO, GuestResponse>.LoadFromDto(GuestResponseDTO dto)
        {
            AccessToken = dto.AccessToken;
            Duration = dto.Duration;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}