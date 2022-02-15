using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Contains all information needed to send new message
    /// </summary>
    public class SendMessageRequest : RequestObjectBase, ISavableTo<SendMessageRequestDTO>
    {
        public MessageRequest Message { get; set; } = new MessageRequest();

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
                Message = Message.TrySaveToDto(),
                SkipEnrichUrl = SkipEnrichUrl,
                SkipPush = SkipPush,
                AdditionalProperties = AdditionalProperties,
            };
    }
}