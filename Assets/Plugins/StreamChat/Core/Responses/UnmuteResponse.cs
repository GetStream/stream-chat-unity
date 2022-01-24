using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Responses;

namespace Plugins.StreamChat.Core.Responses
{
    public partial class UnmuteResponse : ResponseObjectBase, ILoadableFrom<UnmuteResponseDTO, UnmuteResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }


        public UnmuteResponse LoadFromDto(UnmuteResponseDTO dto)
        {
            Duration = dto.Duration;

            return this;
        }
    }
}