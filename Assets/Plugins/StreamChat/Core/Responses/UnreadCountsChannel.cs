using StreamChat.Core.InternalDTO.Extra;

namespace StreamChat.Core.Responses
{
    public sealed class UnreadCountsChannel  : ILoadableFrom<UnreadCountsChannelInternalDTO, UnreadCountsChannel>
    {
        public string ChannelId { get; private set; }

        public System.DateTimeOffset LastRead { get; private set; }

        public int UnreadCount { get; private set; }
        
        UnreadCountsChannel ILoadableFrom<UnreadCountsChannelInternalDTO, UnreadCountsChannel>.LoadFromDto(UnreadCountsChannelInternalDTO dto)
        {
            ChannelId = dto.ChannelId;
            LastRead = dto.LastRead;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}