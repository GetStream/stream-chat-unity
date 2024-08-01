using StreamChat.Core.InternalDTO.Extra;
using System.Collections.Generic;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Responses
{
    public sealed class UnreadCountsResponse : ILoadableFrom<WrappedUnreadCountsResponseInternalDTO, UnreadCountsResponse>
    {
        public IReadOnlyList<UnreadCountsChannelType> ChannelType => _channelType;
        public IReadOnlyList<UnreadCountsChannel> Channels => _channels;
        public IReadOnlyList<UnreadCountsThread> Threads => _threads;

        public int TotalUnreadCount { get; private set; }

        public int TotalUnreadThreadsCount { get; private set; }

        UnreadCountsResponse ILoadableFrom<WrappedUnreadCountsResponseInternalDTO, UnreadCountsResponse>.LoadFromDto(WrappedUnreadCountsResponseInternalDTO dto)
        {
            _channelType = _channelType.TryLoadFromDtoCollection(dto.ChannelType);
            _channels = _channels.TryLoadFromDtoCollection(dto.Channels);
            _threads = _threads.TryLoadFromDtoCollection(dto.Threads);

            TotalUnreadCount = dto.TotalUnreadCount;
            TotalUnreadThreadsCount = dto.TotalUnreadThreadsCount;

            return this;
        }
        
        private List<UnreadCountsChannelType> _channelType = new List<UnreadCountsChannelType>();
        private List<UnreadCountsChannel> _channels = new List<UnreadCountsChannel>();
        private List<UnreadCountsThread> _threads = new List<UnreadCountsThread>();
    }
}