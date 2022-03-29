using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class StopWatchingResponse : ResponseObjectBase, ILoadableFrom<StopWatchingResponseDTO, StopWatchingResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }


        StopWatchingResponse ILoadableFrom<StopWatchingResponseDTO, StopWatchingResponse>.LoadFromDto(StopWatchingResponseDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}