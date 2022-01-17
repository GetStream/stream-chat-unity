using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Models.V2;

namespace Plugins.GetStreamIO.Core.Responses
{
    public class MessageResponse : ResponseObjectBase, ILoadableFrom<MessageResponseDTO, MessageResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        public MessageResponse LoadFromDto(MessageResponseDTO dto)
        {
            Duration = dto.Duration;
            Message = Message.TryLoadFromDto(dto.Message);

            return this;
        }
    }
}