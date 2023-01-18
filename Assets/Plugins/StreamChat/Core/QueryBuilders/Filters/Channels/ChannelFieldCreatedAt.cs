using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.CreatedAt"/>
    /// </summary>
    public class ChannelFieldCreatedAt : BaseFieldToFilter
    {
        public override string FieldName => "CREATED_AT";
    }
}