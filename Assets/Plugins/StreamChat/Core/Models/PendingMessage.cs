using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Models
{
    public partial class PendingMessage : ModelBase, ILoadableFrom<PendingMessageDTO, PendingMessage>
    {
        /// <summary>
        /// The message
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// Additional data attached to the pending message. This data is discarded once the pending message is committed.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> Metadata { get; set; }

        PendingMessage ILoadableFrom<PendingMessageDTO, PendingMessage>.LoadFromDto(PendingMessageDTO dto)
        {
            Message = Message.TryLoadFromDto<MessageDTO, Message>(dto.Message);
            Metadata = dto.Metadata;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

    }
}