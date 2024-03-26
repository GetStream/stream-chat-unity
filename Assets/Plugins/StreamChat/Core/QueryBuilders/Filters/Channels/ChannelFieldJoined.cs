namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by whether current user joined the channel or not
    /// </summary>
    public sealed class ChannelFieldJoined : BaseFieldToFilter
    {
        public override string FieldName => "joined";
        
        public FieldFilterRule EqualsTo(bool hasJoined) => InternalEqualsTo(hasJoined);
    }
}