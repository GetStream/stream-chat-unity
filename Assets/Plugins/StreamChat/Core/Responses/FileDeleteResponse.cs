using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class FileDeleteResponse : ResponseObjectBase, ILoadableFrom<FileDeleteResponseDTO, FileDeleteResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        FileDeleteResponse ILoadableFrom<FileDeleteResponseDTO, FileDeleteResponse>.LoadFromDto(FileDeleteResponseDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}