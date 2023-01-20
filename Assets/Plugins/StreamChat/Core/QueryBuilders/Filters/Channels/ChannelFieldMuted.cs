using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Muted"/>
    /// </summary>
    public sealed class ChannelFieldMuted : BaseFieldToFilter
    {
        public override string FieldName => "muted";
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Muted"/> state is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool isMuted) => InternalEqualsTo(isMuted);
    }
}