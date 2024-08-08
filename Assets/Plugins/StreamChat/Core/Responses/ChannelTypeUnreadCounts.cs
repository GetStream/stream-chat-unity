using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.Responses
{
    public sealed class ChannelTypeUnreadCounts  : ILoadableFrom<UnreadCountsChannelTypeInternalDTO, ChannelTypeUnreadCounts>
    {
        public int ChannelCount { get; private set; }

        public ChannelType ChannelType { get; private set; }

        public int UnreadCount { get; private set; }
        
        ChannelTypeUnreadCounts ILoadableFrom<UnreadCountsChannelTypeInternalDTO, ChannelTypeUnreadCounts>.LoadFromDto(UnreadCountsChannelTypeInternalDTO dto)
        {
            ChannelCount = dto.ChannelCount;
            ChannelType = new ChannelType(dto.ChannelType);
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}