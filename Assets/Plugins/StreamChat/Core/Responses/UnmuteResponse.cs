using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class UnmuteResponse : ResponseObjectBase, ILoadableFrom<UnmuteResponseDTO, UnmuteResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }


        UnmuteResponse ILoadableFrom<UnmuteResponseDTO, UnmuteResponse>.LoadFromDto(UnmuteResponseDTO dto)
        {
            Duration = dto.Duration;

            return this;
        }
    }
}