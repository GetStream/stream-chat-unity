using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class FileDeleteResponse : ResponseObjectBase, ILoadableFrom<FileDeleteResponseInternalDTO, FileDeleteResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        FileDeleteResponse ILoadableFrom<FileDeleteResponseInternalDTO, FileDeleteResponse>.LoadFromDto(FileDeleteResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}