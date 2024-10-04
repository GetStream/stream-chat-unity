using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Responses
{
    public sealed class StreamChannelUnreadCounts  : ILoadableFrom<UnreadCountsChannelInternalDTO, StreamChannelUnreadCounts>
    {
        /// <summary>
        /// CID of the channel
        /// </summary>
        public string ChannelCid { get; private set; }

        /// <summary>
        /// DateTimeOffset of the last read message
        /// </summary>
        public System.DateTimeOffset LastRead { get; private set; }

        /// <summary>
        /// Count of unread messages
        /// </summary>
        public int UnreadCount { get; private set; }
        
        StreamChannelUnreadCounts ILoadableFrom<UnreadCountsChannelInternalDTO, StreamChannelUnreadCounts>.LoadFromDto(UnreadCountsChannelInternalDTO dto)
        {
            ChannelCid = dto.ChannelId;
            LastRead = dto.LastRead;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}