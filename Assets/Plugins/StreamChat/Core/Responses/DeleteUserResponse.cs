using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class DeleteUserResponse : ResponseObjectBase, ILoadableFrom<DeleteUserResponseInternalDTO, DeleteUserResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public string TaskId { get; set; }

        public User User { get; set; }

        DeleteUserResponse ILoadableFrom<DeleteUserResponseInternalDTO, DeleteUserResponse>.LoadFromDto(DeleteUserResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            TaskId = dto.TaskId;
            User = User.TryLoadFromDto<UserObjectInternalInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}