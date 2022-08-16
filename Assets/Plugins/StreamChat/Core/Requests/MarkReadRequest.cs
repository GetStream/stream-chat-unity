using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class MarkReadRequest : RequestObjectBase, ISavableTo<MarkReadRequestDTO>
    {
        /// <summary>
        /// ID of the message that is considered last read by client. If empty the whole channel will be considered as read
        /// </summary>
        public string MessageId { get; set; }

        MarkReadRequestDTO ISavableTo<MarkReadRequestDTO>.SaveToDto() =>
            new MarkReadRequestDTO
            {
                MessageId = MessageId,
            };
    }
}