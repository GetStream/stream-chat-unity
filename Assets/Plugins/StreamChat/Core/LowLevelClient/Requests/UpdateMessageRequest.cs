using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UpdateMessageRequest : RequestObjectBase, ISavableTo<UpdateMessageRequestInternalDTO>
    {
        public MessageRequest Message { get; set; }

        public System.Collections.Generic.Dictionary<string, string> PendingMessageMetadata { get; set; }

        /// <summary>
        /// Do not try to enrich the links within message
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        UpdateMessageRequestInternalDTO ISavableTo<UpdateMessageRequestInternalDTO>.SaveToDto() =>
            new UpdateMessageRequestInternalDTO
            {
                Message = Message.TrySaveToDto(),
                PendingMessageMetadata = PendingMessageMetadata,
                SkipEnrichUrl = SkipEnrichUrl
            };
    }
}