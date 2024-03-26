using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public partial class PendingMessage : ModelBase, ILoadableFrom<PendingMessageInternalDTO, PendingMessage>
    {
        /// <summary>
        /// The message
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// Additional data attached to the pending message. This data is discarded once the pending message is committed.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> Metadata { get; set; }

        PendingMessage ILoadableFrom<PendingMessageInternalDTO, PendingMessage>.LoadFromDto(PendingMessageInternalDTO dto)
        {
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            Metadata = dto.Metadata;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}