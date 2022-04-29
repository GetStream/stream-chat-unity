using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class FileUploadResponse : ResponseObjectBase, ILoadableFrom<FileUploadResponseDTO, FileUploadResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public string File { get; set; }

        FileUploadResponse ILoadableFrom<FileUploadResponseDTO, FileUploadResponse>.LoadFromDto(FileUploadResponseDTO dto)
        {
            Duration = dto.Duration;
            File = dto.File;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}