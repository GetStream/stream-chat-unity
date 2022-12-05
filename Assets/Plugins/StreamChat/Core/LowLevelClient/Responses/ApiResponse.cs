using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class ApiResponse : ResponseObjectBase, ILoadableFrom<ResponseInternalDTO, ApiResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        ApiResponse ILoadableFrom<ResponseInternalDTO, ApiResponse>.LoadFromDto(ResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}