using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Responses
{
    public sealed class StreamThreadUnreadCounts  : ILoadableFrom<UnreadCountsThreadInternalDTO, StreamThreadUnreadCounts>
    {
        public System.DateTimeOffset LastRead { get; private set; }

        public string LastReadMessageId { get; private set; }

        public string ParentMessageId { get; private set; }

        public int UnreadCount { get; private set; }
        
        StreamThreadUnreadCounts ILoadableFrom<UnreadCountsThreadInternalDTO, StreamThreadUnreadCounts>.LoadFromDto(UnreadCountsThreadInternalDTO dto)
        {
            LastRead = dto.LastRead;
            LastReadMessageId = dto.LastReadMessageId;
            ParentMessageId = dto.ParentMessageId;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}