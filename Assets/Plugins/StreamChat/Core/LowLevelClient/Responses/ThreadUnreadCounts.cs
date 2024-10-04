using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public sealed class ThreadUnreadCounts  : ILoadableFrom<UnreadCountsThreadInternalDTO, ThreadUnreadCounts>
    {
        public System.DateTimeOffset LastRead { get; private set; }

        public string LastReadMessageId { get; private set; }

        public string ParentMessageId { get; private set; }

        public int UnreadCount { get; private set; }
        
        ThreadUnreadCounts ILoadableFrom<UnreadCountsThreadInternalDTO, ThreadUnreadCounts>.LoadFromDto(UnreadCountsThreadInternalDTO dto)
        {
            LastRead = dto.LastRead;
            LastReadMessageId = dto.LastReadMessageId;
            ParentMessageId = dto.ParentMessageId;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}