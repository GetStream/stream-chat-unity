using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.Responses
{
    public sealed class ChannelUnreadCounts  : ILoadableFrom<UnreadCountsChannelInternalDTO, ChannelUnreadCounts>
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
        
        ChannelUnreadCounts ILoadableFrom<UnreadCountsChannelInternalDTO, ChannelUnreadCounts>.LoadFromDto(UnreadCountsChannelInternalDTO dto)
        {
            ChannelCid = dto.ChannelId;
            LastRead = dto.LastRead;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}