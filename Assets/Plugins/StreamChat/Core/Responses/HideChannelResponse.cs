using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class HideChannelResponse : ResponseObjectBase,
        ILoadableFrom<HideChannelResponseDTO, HideChannelResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        HideChannelResponse ILoadableFrom<HideChannelResponseDTO, HideChannelResponse>.LoadFromDto(HideChannelResponseDTO dto)
        {
            Duration = dto.Duration;

            return this;
        }
    }
}