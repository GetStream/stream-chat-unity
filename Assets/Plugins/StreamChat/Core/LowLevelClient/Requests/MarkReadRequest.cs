using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class MarkReadRequest : RequestObjectBase, ISavableTo<MarkReadRequestInternalDTO>
    {
        /// <summary>
        /// ID of the message that is considered last read by client. If empty the whole channel will be considered as read
        /// </summary>
        public string MessageId { get; set; }

        MarkReadRequestInternalDTO ISavableTo<MarkReadRequestInternalDTO>.SaveToDto() =>
            new MarkReadRequestInternalDTO
            {
                MessageId = MessageId,
            };
    }
}