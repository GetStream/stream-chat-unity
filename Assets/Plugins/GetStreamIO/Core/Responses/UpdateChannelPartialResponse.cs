using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Helpers;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Responses
{
    public partial class UpdateChannelPartialResponse : ResponseObjectBase, ILoadableFrom<UpdateChannelPartialResponseDTO, UpdateChannelPartialResponse>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public System.Collections.Generic.ICollection<ChannelMember> Members { get; set; }

        public UpdateChannelPartialResponse LoadFromDto(UpdateChannelPartialResponseDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Duration = dto.Duration;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            AdditionalProperties = dto.AdditionalProperties;
            
            return this;
        }
    }
}