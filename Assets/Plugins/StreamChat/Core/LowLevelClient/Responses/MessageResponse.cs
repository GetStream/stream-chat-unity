using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public class MessageResponse : ResponseObjectBase, ILoadableFrom<MessageResponseInternalDTO, MessageResponse>
    {
        public Message Message { get; set; }

        MessageResponse ILoadableFrom<MessageResponseInternalDTO, MessageResponse>.LoadFromDto(MessageResponseInternalDTO dto)
        {
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}