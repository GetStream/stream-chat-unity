using Assets.StreamChat.Core.Responses;
using StreamChat.Core.InternalDTO.Extra;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Responses
{
    public sealed class UnreadCountsResponse  : IStateLoadableFrom<WrappedUnreadCountsResponseInternalDTO, UnreadCountsResponse>
    {
        public System.Collections.Generic.List<UnreadCountsChannelType> ChannelType { get; private set; }

        public System.Collections.Generic.List<UnreadCountsChannel> Channels { get; private set; }

        public System.Collections.Generic.List<UnreadCountsThread> Threads { get; private set; }

        public int TotalUnreadCount { get; private set; }

        public int TotalUnreadThreadsCount { get; private set; }
        
        UnreadCountsResponse IStateLoadableFrom<WrappedUnreadCountsResponseInternalDTO, UnreadCountsResponse>.LoadFromDto(WrappedUnreadCountsResponseInternalDTO dto, ICache cache)
        {
            ChannelType = ChannelType.TryLoadFromDtoCollection(dto.ChannelType, cache);
            Channels = Channels.TryLoadFromDtoCollection(dto.Channels, cache);
            Threads = Threads.TryLoadFromDtoCollection(dto.Threads, cache);
            TotalUnreadCount = dto.TotalUnreadCount;
            TotalUnreadThreadsCount = dto.TotalUnreadThreadsCount;

            return this;
        }
    }
}