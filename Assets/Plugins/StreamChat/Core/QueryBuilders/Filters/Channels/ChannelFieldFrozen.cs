using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Frozen"/>
    /// </summary>
    public sealed class ChannelFieldFrozen : BaseFieldToFilter
    {
        public override string FieldName => "frozen";

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Frozen"/> state is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool isFrozen) => InternalEqualsTo(isFrozen);
    }
}