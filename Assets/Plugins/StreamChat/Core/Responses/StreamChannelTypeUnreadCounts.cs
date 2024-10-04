using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Responses
{
    public sealed class StreamChannelTypeUnreadCounts  : ILoadableFrom<UnreadCountsChannelTypeInternalDTO, StreamChannelTypeUnreadCounts>
    {
        public int ChannelCount { get; private set; }

        public ChannelType ChannelType { get; private set; }

        public int UnreadCount { get; private set; }
        
        StreamChannelTypeUnreadCounts ILoadableFrom<UnreadCountsChannelTypeInternalDTO, StreamChannelTypeUnreadCounts>.LoadFromDto(UnreadCountsChannelTypeInternalDTO dto)
        {
            ChannelCount = dto.ChannelCount;
            ChannelType = new ChannelType(dto.ChannelType);
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}