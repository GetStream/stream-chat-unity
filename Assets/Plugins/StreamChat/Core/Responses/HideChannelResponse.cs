using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Responses;

namespace Plugins.StreamChat.Core.Responses
{
    public partial class HideChannelResponse : ResponseObjectBase,
        ILoadableFrom<HideChannelResponseDTO, HideChannelResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public HideChannelResponse LoadFromDto(HideChannelResponseDTO dto)
        {
            Duration = dto.Duration;

            return this;
        }
    }
}