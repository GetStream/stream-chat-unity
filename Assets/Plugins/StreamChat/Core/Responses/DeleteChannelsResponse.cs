using StreamChat.Core.Helpers;
using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Responses;

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

        public DeleteChannelsResponse LoadFromDto(DeleteChannelsResponseDTO dto)
        {
            Duration = dto.Duration;
            Result = Result.TryLoadFromDtoDictionary(dto.Result);
            TaskId = dto.TaskId;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}