using StreamChat.Core.Helpers;
using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class DeleteChannelsResponse : ResponseObjectBase, ILoadableFrom<DeleteChannelsResponseDTO, DeleteChannelsResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public System.Collections.Generic.IDictionary<string, DeleteChannelsResult> Result { get; set; }

        public string TaskId { get; set; }

        DeleteChannelsResponse ILoadableFrom<DeleteChannelsResponseDTO, DeleteChannelsResponse>.LoadFromDto(DeleteChannelsResponseDTO dto)
        {
            Duration = dto.Duration;
            Result = Result.TryLoadFromDtoDictionary(dto.Result);
            TaskId = dto.TaskId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}