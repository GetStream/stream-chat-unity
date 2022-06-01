using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class ApiResponse : ResponseObjectBase, ILoadableFrom<ResponseDTO, ApiResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        ApiResponse ILoadableFrom<ResponseDTO, ApiResponse>.LoadFromDto(ResponseDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}