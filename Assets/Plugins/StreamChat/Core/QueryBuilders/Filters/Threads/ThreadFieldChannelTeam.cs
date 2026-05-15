using System.Collections.Generic;

namespace StreamChat.Core.QueryBuilders.Filters.Threads
{
    /// <summary>
    /// Filter threads by channel <c>team</c>
    /// </summary>
    public sealed class ThreadFieldChannelTeam : BaseFieldToFilter
    {
        public override string FieldName => "channel.team";

        public FieldFilterRule EqualsTo(string team) => InternalEqualsTo(team);

        public FieldFilterRule In(IEnumerable<string> teams) => InternalIn(teams);

        public FieldFilterRule In(params string[] teams) => InternalIn(teams);
    }
}
