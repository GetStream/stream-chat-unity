using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class TruncateChannelRequest : ResponseObjectBase, ISavableTo<TruncateChannelRequestInternalDTO>
    {
        /// <summary>
        /// Permanently delete channel data (messages, reactions, etc.)
        /// </summary>
        public bool? HardDelete { get; set; }

        public MessageRequest Message { get; set; }

        /// <summary>
        /// When `message` is set disables all push notifications for it
        /// </summary>
        public bool? SkipPush { get; set; }

        /// <summary>
        /// Truncate channel data up to `truncated_at`. The system message (if provided) creation time is always greater than `truncated_at`
        /// </summary>
        public System.DateTimeOffset? TruncatedAt { get; set; }

        TruncateChannelRequestInternalDTO ISavableTo<TruncateChannelRequestInternalDTO>.SaveToDto() =>
            new TruncateChannelRequestInternalDTO
            {
                HardDelete = HardDelete,
                Message = Message.TrySaveToDto(),
                SkipPush = SkipPush,
                TruncatedAt = TruncatedAt,
                AdditionalProperties = AdditionalProperties,
            };
    }
}