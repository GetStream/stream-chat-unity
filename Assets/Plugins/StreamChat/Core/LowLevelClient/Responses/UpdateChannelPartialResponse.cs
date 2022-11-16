using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class UpdateChannelPartialResponse : ResponseObjectBase, ILoadableFrom<UpdateChannelPartialResponseInternalDTO, UpdateChannelPartialResponse>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public System.Collections.Generic.List<ChannelMember> Members { get; set; }

        UpdateChannelPartialResponse ILoadableFrom<UpdateChannelPartialResponseInternalDTO, UpdateChannelPartialResponse>.LoadFromDto(UpdateChannelPartialResponseInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Duration = dto.Duration;
            Members = Members.TryLoadFromDtoCollection(dto.Members);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}