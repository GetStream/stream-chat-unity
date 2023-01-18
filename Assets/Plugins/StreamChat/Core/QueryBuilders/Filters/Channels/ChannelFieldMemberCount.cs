using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel"/> members count
    /// </summary>
    public class ChannelFieldMemberCount : BaseFieldToFilter
    {
        public override string FieldName => "MEMBER_COUNT";
    }
}