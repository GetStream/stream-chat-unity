using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class DeleteChannelResponse : ResponseObjectBase, ILoadableFrom<DeleteChannelResponseInternalDTO, DeleteChannelResponse>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        DeleteChannelResponse ILoadableFrom<DeleteChannelResponseInternalDTO, DeleteChannelResponse>.LoadFromDto(DeleteChannelResponseInternalDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}