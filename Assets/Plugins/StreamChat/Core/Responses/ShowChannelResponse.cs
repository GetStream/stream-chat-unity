using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class ShowChannelResponse : ResponseObjectBase,
        ILoadableFrom<ShowChannelResponseDTO, ShowChannelResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        ShowChannelResponse ILoadableFrom<ShowChannelResponseDTO, ShowChannelResponse>.LoadFromDto(ShowChannelResponseDTO dto)
        {
            Duration = dto.Duration;

            return this;
        }
    }
}