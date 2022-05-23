using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class UpdateChannelResponse : ResponseObjectBase, ILoadableFrom<UpdateChannelResponseDTO, UpdateChannelResponse>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public System.Collections.Generic.List<ChannelMember> Members { get; set; }

        public Message Message { get; set; }

        UpdateChannelResponse ILoadableFrom<UpdateChannelResponseDTO, UpdateChannelResponse>.LoadFromDto(UpdateChannelResponseDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Duration = dto.Duration;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            Message = Message.TryLoadFromDto(dto.Message);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}