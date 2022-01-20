using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Helpers;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Requests;

namespace Plugins.GetStreamIO.Core.Responses
{
    public partial class UpdateChannelResponse : ResponseObjectBase, ILoadableFrom<UpdateChannelResponseDTO, UpdateChannelResponse>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public System.Collections.Generic.ICollection<ChannelMember> Members { get; set; }

        public Message Message { get; set; }

        public UpdateChannelResponse LoadFromDto(UpdateChannelResponseDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Duration = dto.Duration;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            Message = Message.LoadFromDto(dto.Message);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}