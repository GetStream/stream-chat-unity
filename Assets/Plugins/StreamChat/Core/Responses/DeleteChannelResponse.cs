using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class DeleteChannelResponse : ResponseObjectBase, ILoadableFrom<DeleteChannelResponseDTO, DeleteChannelResponse>
    {
        public Channel Channel { get; set; }

        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        DeleteChannelResponse ILoadableFrom<DeleteChannelResponseDTO, DeleteChannelResponse>.LoadFromDto(DeleteChannelResponseDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Duration = dto.Duration;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}