using StreamChat.Core.InternalDTO.Extra;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Responses
{
    public sealed class UnreadCountsChannelType  : IStateLoadableFrom<UnreadCountsChannelTypeInternalDTO, UnreadCountsChannelType>
    {
        public int ChannelCount { get; private set; }

        public string ChannelType { get; private set; }

        public int UnreadCount { get; private set; }
        
        UnreadCountsChannelType IStateLoadableFrom<UnreadCountsChannelTypeInternalDTO, UnreadCountsChannelType>.LoadFromDto(UnreadCountsChannelTypeInternalDTO dto, ICache cache)
        {
            ChannelCount = dto.ChannelCount;
            ChannelType = dto.ChannelType;
            UnreadCount = dto.UnreadCount;

            return this;
        }
    }
}