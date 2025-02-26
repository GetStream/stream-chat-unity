using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    public partial class PendingMessage : ModelBase, ILoadableFrom<PendingMessageResponseInternalDTO, PendingMessage>
    {
        /// <summary>
        /// The message
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// Additional data attached to the pending message. This data is discarded once the pending message is committed.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> Metadata { get; set; }

        PendingMessage ILoadableFrom<PendingMessageResponseInternalDTO, PendingMessage>.LoadFromDto(PendingMessageResponseInternalDTO dto)
        {
            Message = Message.TryLoadFromDto<MessageResponseInternalDTO, Message>(dto.Message);
            Metadata = dto.Metadata;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}