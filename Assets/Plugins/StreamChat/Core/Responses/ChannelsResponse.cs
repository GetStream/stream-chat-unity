using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class ChannelsResponse : ResponseObjectBase, ILoadableFrom<ChannelsResponseDTO, ChannelsResponse>
    {
        /// <summary>
        /// List of channels
        /// </summary>
        public System.Collections.Generic.List<ChannelState> Channels { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        ChannelsResponse ILoadableFrom<ChannelsResponseDTO, ChannelsResponse>.LoadFromDto(ChannelsResponseDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            Channels = Channels.TryLoadFromDtoCollection(dto.Channels);
            Duration = dto.Duration;

            return this;
        }
    }
}