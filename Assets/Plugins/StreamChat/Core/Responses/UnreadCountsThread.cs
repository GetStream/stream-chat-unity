using StreamChat.Core.InternalDTO.Extra;

namespace StreamChat.Core.Responses
{
    public sealed class UnreadCountsThread  : ILoadableFrom<UnreadCountsThreadInternalDTO, UnreadCountsThread>
    {
        public System.DateTimeOffset LastRead { get; private set; }

        public string LastReadMessageId { get; private set; }

        public string ParentMessageId { get; private set; }

        public int UnreadCount { get; private set; }
        
        UnreadCountsThread ILoadableFrom<UnreadCountsThreadInternalDTO, UnreadCountsThread>.LoadFromDto(UnreadCountsThreadInternalDTO dto)
        {
            LastRead = dto.LastRead;
            LastReadMessageId = dto.LastReadMessageId;
            ParentMessageId = dto.ParentMessageId;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}