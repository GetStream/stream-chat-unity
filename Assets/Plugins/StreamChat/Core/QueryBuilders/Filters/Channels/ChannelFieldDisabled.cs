using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Disabled"/>
    /// </summary>
    public sealed class ChannelFieldDisabled : BaseFieldToFilter
    {
        public override string FieldName => "disabled";
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Disabled"/> state is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool isDisabled) => InternalEqualsTo(isDisabled);
    }
}