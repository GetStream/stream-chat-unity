using StreamChat.Core.InternalDTO.Extra;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace Assets.StreamChat.Core.Responses
{
    public sealed class UnreadCountsChannel  : IStateLoadableFrom<UnreadCountsChannelInternalDTO, UnreadCountsChannel>
    {
        public string ChannelId { get; private set; }

        public System.DateTimeOffset LastRead { get; private set; }

        public int UnreadCount { get; private set; }
        
        UnreadCountsChannel IStateLoadableFrom<UnreadCountsChannelInternalDTO, UnreadCountsChannel>.LoadFromDto(UnreadCountsChannelInternalDTO dto, ICache cache)
        {
            ChannelId = dto.ChannelId;
            LastRead = dto.LastRead;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}