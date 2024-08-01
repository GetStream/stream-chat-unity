using StreamChat.Core.InternalDTO.Extra;

namespace StreamChat.Core.Responses
{
    public sealed class UnreadCountsChannelType  : ILoadableFrom<UnreadCountsChannelTypeInternalDTO, UnreadCountsChannelType>
    {
        public int ChannelCount { get; private set; }

        public ChannelType ChannelType { get; private set; }

        public int UnreadCount { get; private set; }
        
        UnreadCountsChannelType ILoadableFrom<UnreadCountsChannelTypeInternalDTO, UnreadCountsChannelType>.LoadFromDto(UnreadCountsChannelTypeInternalDTO dto)
        {
            ChannelCount = dto.ChannelCount;
            ChannelType = new ChannelType(dto.ChannelType);
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}