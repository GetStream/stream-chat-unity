using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Disabled"/>
    /// </summary>
    public class ChannelFieldDisabled : BaseFieldToFilter
    {
        public override string FieldName => "DISABLED";
    }
}