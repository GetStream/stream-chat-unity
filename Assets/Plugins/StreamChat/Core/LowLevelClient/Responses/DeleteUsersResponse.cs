using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class DeleteUsersResponse : ResponseObjectBase, ILoadableFrom<DeleteUsersResponseInternalDTO, DeleteUsersResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public string TaskId { get; set; }

        DeleteUsersResponse ILoadableFrom<DeleteUsersResponseInternalDTO, DeleteUsersResponse>.LoadFromDto(DeleteUsersResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            TaskId = dto.TaskId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}