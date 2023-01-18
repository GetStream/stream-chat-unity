using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Team"/>
    /// </summary>
    public class ChannelFieldTeam : BaseFieldToFilter
    {
        public override string FieldName => "TEAM";
    }
}