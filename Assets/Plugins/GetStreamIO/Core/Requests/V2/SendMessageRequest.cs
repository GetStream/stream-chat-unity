using GetStreamIO.Core.DTO.Requests;
using Plugins.GetStreamIO.Core.Helpers;

namespace Plugins.GetStreamIO.Core.Requests.V2
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

        public SendMessageRequestDTO SaveToDto() =>
            new SendMessageRequestDTO
            {
                Message = Message.TrySaveToDto(),
                SkipEnrichUrl = SkipEnrichUrl,
                SkipPush = SkipPush,
                AdditionalProperties = AdditionalProperties,
            };
    }
}