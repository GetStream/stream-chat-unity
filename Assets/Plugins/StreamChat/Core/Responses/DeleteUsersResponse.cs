using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class DeleteUsersResponse : ResponseObjectBase, ILoadableFrom<DeleteUsersResponseDTO, DeleteUsersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public string TaskId { get; set; }

        DeleteUsersResponse ILoadableFrom<DeleteUsersResponseDTO, DeleteUsersResponse>.LoadFromDto(DeleteUsersResponseDTO dto)
        {
            Duration = dto.Duration;
            TaskId = dto.TaskId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}