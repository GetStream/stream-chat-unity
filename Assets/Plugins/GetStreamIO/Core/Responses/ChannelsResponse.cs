using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Helpers;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Utils;

namespace Plugins.GetStreamIO.Core.Responses
{
    public partial class ChannelsResponse : ResponseObjectBase, ILoadableFrom<ChannelsResponseDTO, ChannelsResponse>
    {
        /// <summary>
        /// List of channels
        /// </summary>
        public System.Collections.Generic.ICollection<ChannelState> Channels { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public ChannelsResponse LoadFromDto(ChannelsResponseDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            Channels = Channels.TryLoadFromDtoCollection(dto.Channels);
            Duration = dto.Duration;

            return this;
        }
    }
}