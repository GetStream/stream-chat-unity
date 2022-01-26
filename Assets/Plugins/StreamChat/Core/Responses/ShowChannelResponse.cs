using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class ShowChannelResponse : ResponseObjectBase,
        ILoadableFrom<ShowChannelResponseDTO, ShowChannelResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public ShowChannelResponse LoadFromDto(ShowChannelResponseDTO dto)
        {
            Duration = dto.Duration;

            return this;
        }
    }
}