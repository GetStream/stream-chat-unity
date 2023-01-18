using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Hidden"/>
    /// </summary>
    public class ChannelFieldHidden : BaseFieldToFilter
    {
        public override string FieldName => "hidden";

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Hidden"/> state is equal to provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool isHidden) => InternalEqualsTo(isHidden);
    }
}