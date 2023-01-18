using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Muted"/>
    /// </summary>
    public class ChannelFieldMuted : BaseFieldToFilter
    {
        public override string FieldName => "MUTED";
        
        public FieldFilterRule EqualsTo(bool isMuted) => InternalEqualsTo(isMuted);
    }
}