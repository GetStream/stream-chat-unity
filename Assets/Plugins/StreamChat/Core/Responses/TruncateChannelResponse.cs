using StreamChat.Core;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Responses
{
    public partial class TruncateChannelResponse : ResponseObjectBase, ILoadableFrom<TruncateChannelResponseDTO, TruncateChannelResponse>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        public TruncateChannelResponse LoadFromDto(TruncateChannelResponseDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Duration = dto.Duration;
            Message = Message.TryLoadFromDto(dto.Message);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}