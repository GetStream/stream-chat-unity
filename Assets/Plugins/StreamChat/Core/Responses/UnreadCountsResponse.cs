using StreamChat.Core.InternalDTO.Extra;
using System.Collections.Generic;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Responses
{
    public sealed class UnreadCountsResponse : ILoadableFrom<WrappedUnreadCountsResponseInternalDTO, UnreadCountsResponse>
    {
        /// <summary>
        /// Unread status grouped by <see cref="ChannelType"/>. Each entry represents a channel type with unread messages among all channels of that type
        /// </summary>
        public IReadOnlyList<ChannelTypeUnreadCounts> UnreadChannelsByType => _unreadChannelsByType;
        
        /// <summary>
        /// Unread status per channel. Each entry represents a channel with unread messages
        /// </summary>
        public IReadOnlyList<ChannelUnreadCounts> UnreadChannels => _unreadChannels;
        
        /// <summary>
        /// Unread status per thread. Each entry represents a thread with unread messages
        /// </summary>
        public IReadOnlyList<ThreadUnreadCounts> UnreadThreads => _unreadThreads;

        /// <summary>
        /// Total unread messages count
        /// </summary>
        public int TotalUnreadCount { get; private set; }

        /// <summary>
        /// Total unread threads count
        /// </summary>
        public int TotalUnreadThreadsCount { get; private set; }

        UnreadCountsResponse ILoadableFrom<WrappedUnreadCountsResponseInternalDTO, UnreadCountsResponse>.LoadFromDto(WrappedUnreadCountsResponseInternalDTO dto)
        {
            _unreadChannelsByType = _unreadChannelsByType.TryLoadFromDtoCollection(dto.ChannelType);
            _unreadChannels = _unreadChannels.TryLoadFromDtoCollection(dto.Channels);
            _unreadThreads = _unreadThreads.TryLoadFromDtoCollection(dto.Threads);

            TotalUnreadCount = dto.TotalUnreadCount;
            TotalUnreadThreadsCount = dto.TotalUnreadThreadsCount;

            return this;
        }
        
        private List<ChannelTypeUnreadCounts> _unreadChannelsByType = new List<ChannelTypeUnreadCounts>();
        private List<ChannelUnreadCounts> _unreadChannels = new List<ChannelUnreadCounts>();
        private List<ThreadUnreadCounts> _unreadThreads = new List<ThreadUnreadCounts>();
    }
}