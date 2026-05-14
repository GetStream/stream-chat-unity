namespace StreamChat.Core.QueryBuilders.Filters.Threads
{
    /// <summary>
    /// Filter threads by channel <c>disabled</c> state
    /// </summary>
    public sealed class ThreadFieldChannelDisabled : BaseFieldToFilter
    {
        public override string FieldName => "channel.disabled";

        public FieldFilterRule EqualsTo(bool disabled) => InternalEqualsTo(disabled);
    }
}
