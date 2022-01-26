using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class DeleteUserResponse : ResponseObjectBase, ILoadableFrom<DeleteUserResponseDTO, DeleteUserResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public User User { get; set; }

        public DeleteUserResponse LoadFromDto(DeleteUserResponseDTO dto)
        {
            Duration = dto.Duration;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}