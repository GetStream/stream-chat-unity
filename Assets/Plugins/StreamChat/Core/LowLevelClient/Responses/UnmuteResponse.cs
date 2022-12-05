using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class UnmuteResponse : ResponseObjectBase, ILoadableFrom<UnmuteResponseInternalDTO, UnmuteResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        UnmuteResponse ILoadableFrom<UnmuteResponseInternalDTO, UnmuteResponse>.LoadFromDto(UnmuteResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}