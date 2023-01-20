using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Team"/>
    /// </summary>
    public sealed class ChannelFieldTeam : BaseFieldToFilter
    {
        public override string FieldName => "team";
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Team"/> state is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(string team) => InternalEqualsTo(team);
    }
}