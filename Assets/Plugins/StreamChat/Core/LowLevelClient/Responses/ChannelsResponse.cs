using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class ChannelsResponse : ResponseObjectBase, ILoadableFrom<QueryChannelsResponseInternalDTO, ChannelsResponse>
    {
        /// <summary>
        /// List of channels
        /// </summary>
        public System.Collections.Generic.List<ChannelState> Channels { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        ChannelsResponse ILoadableFrom<QueryChannelsResponseInternalDTO, ChannelsResponse>.LoadFromDto(QueryChannelsResponseInternalDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            Channels = Channels.TryLoadFromDtoCollection(dto.Channels);
            Duration = dto.Duration;

            return this;
        }
    }
}