using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel"/> members count
    /// </summary>
    public sealed class ChannelFieldMemberCount : BaseFieldToFilter
    {
        public override string FieldName => "member_count";

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> count is EQUAL to the provided one
        /// </summary>
        public FieldFilterRule EqualsTo(int memberCount) => InternalEqualsTo(memberCount);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> count is GREATER THAN the provided one
        /// </summary>
        public FieldFilterRule GreaterThan(int memberCount) => InternalGreaterThan(memberCount);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> count is GREATER THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule GreaterThanOrEquals(int memberCount) => InternalGreaterThanOrEquals(memberCount);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> count is LESS THAN the provided one
        /// </summary>
        public FieldFilterRule LessThan(int memberCount) => InternalLessThan(memberCount);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> count is LESS THAN OR EQUAL to the provided one
        /// </summary>
        public FieldFilterRule LessThanOrEquals(int memberCount) => InternalLessThanOrEquals(memberCount);
    }
}