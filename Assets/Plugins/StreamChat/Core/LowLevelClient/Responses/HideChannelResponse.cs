using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class HideChannelResponse : ResponseObjectBase,
        ILoadableFrom<HideChannelResponseInternalDTO, HideChannelResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        HideChannelResponse ILoadableFrom<HideChannelResponseInternalDTO, HideChannelResponse>.LoadFromDto(HideChannelResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}