using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class StopWatchingResponse : ResponseObjectBase, ILoadableFrom<StopWatchingResponseInternalDTO, StopWatchingResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }


        StopWatchingResponse ILoadableFrom<StopWatchingResponseInternalDTO, StopWatchingResponse>.LoadFromDto(StopWatchingResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}