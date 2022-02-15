using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public class MessageResponse : ResponseObjectBase, ILoadableFrom<MessageResponseDTO, MessageResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        MessageResponse ILoadableFrom<MessageResponseDTO, MessageResponse>.LoadFromDto(MessageResponseDTO dto)
        {
            Duration = dto.Duration;
            Message = Message.TryLoadFromDto(dto.Message);

            return this;
        }
    }
}