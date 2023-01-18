using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.UpdatedAt"/>
    /// </summary>
    public class ChannelFieldUpdatedAt : BaseFieldToFilter
    {
        public override string FieldName => "UPDATED_AT";
    }
}