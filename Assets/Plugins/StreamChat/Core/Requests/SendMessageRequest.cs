using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Contains all information needed to send new message
    /// </summary>
    public class SendMessageRequest : RequestObjectBase, ISavableTo<SendMessageRequestDTO>
    {
        /// <summary>
        /// Make the message a pending message. This message will not be viewable to others until it is committed.
        /// </summary>
        public bool? IsPendingMessage { get; set; }

        public MessageRequest Message { get; set; } = new MessageRequest();

        public System.Collections.Generic.Dictionary<string, string> PendingMessageMetadata { get; set; }

        /// <summary>
        /// Do not try to enrich the links within message
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        /// <summary>
        /// Disables all push notifications for this message
        /// </summary>
        public bool? SkipPush { get; set; }

        SendMessageRequestDTO ISavableTo<SendMessageRequestDTO>.SaveToDto() =>
            new SendMessageRequestDTO
            {
                IsPendingMessage = IsPendingMessage,
                Message = Message.TrySaveToDto(),
                PendingMessageMetadata = PendingMessageMetadata,
                SkipEnrichUrl = SkipEnrichUrl,
                SkipPush = SkipPush,
                AdditionalProperties = AdditionalProperties,
            };
    }
}