using StreamChat.Core.InternalDTO.Extra;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace Assets.StreamChat.Core.Responses
{
    public sealed class UnreadCountsThread  : IStateLoadableFrom<UnreadCountsThreadInternalDTO, UnreadCountsThread>
    {
        public System.DateTimeOffset LastRead { get; private set; }

        public string LastReadMessageId { get; private set; }

        public string ParentMessageId { get; private set; }

        public int UnreadCount { get; private set; }
        
        UnreadCountsThread IStateLoadableFrom<UnreadCountsThreadInternalDTO, UnreadCountsThread>.LoadFromDto(UnreadCountsThreadInternalDTO dto, ICache cache)
        {
            LastRead = dto.LastRead;
            LastReadMessageId = dto.LastReadMessageId;
            ParentMessageId = dto.ParentMessageId;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}