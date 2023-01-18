using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Frozen"/>
    /// </summary>
    public class ChannelFieldFrozen : BaseFieldToFilter
    {
        public override string FieldName => "frozen";

        public FieldFilterRule EqualsTo(bool isFrozen) => InternalEqualsTo(isFrozen);
    }
}