using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Contains all information needed to send new message
    /// </summary>
    public class SendMessageRequest : RequestObjectBase, ISavableTo<SendMessageRequestInternalDTO>
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

        SendMessageRequestInternalDTO ISavableTo<SendMessageRequestInternalDTO>.SaveToDto() =>
            new SendMessageRequestInternalDTO
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