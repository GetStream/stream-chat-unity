namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by whether current user joined the channel or not
    /// </summary>
    public class ChannelFieldJoined : BaseFieldToFilter
    {
        public override string FieldName => "JOINED";
    }
}