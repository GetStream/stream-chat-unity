using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public class MessageResponse : ResponseObjectBase, ILoadableFrom<MessageResponseInternalDTO, MessageResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        MessageResponse ILoadableFrom<MessageResponseInternalDTO, MessageResponse>.LoadFromDto(MessageResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}