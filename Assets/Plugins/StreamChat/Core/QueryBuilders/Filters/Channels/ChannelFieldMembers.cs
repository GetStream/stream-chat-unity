using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Members"/>
    /// </summary>
    public sealed class ChannelFieldMembers : BaseFieldToFilter
    {
        public override string FieldName => "members";

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> contains a user with provided user ID
        /// </summary>
        public FieldFilterRule EqualsTo(string userId) => InternalEqualsTo(userId);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> contain any of the users with provided user IDs
        /// </summary>
        public FieldFilterRule In(IEnumerable<string> userIds) => InternalIn(userIds);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> contain any of the users with provided user IDs
        /// </summary>
        public FieldFilterRule In(params string[] userIds) => InternalIn(userIds);

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> contain any of the users with provided user IDs
        /// </summary>
        public FieldFilterRule In(IEnumerable<IStreamUser> userIds)
            => InternalIn(userIds.Select(_ => _.Id));
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> contain any of the users with provided user IDs
        /// </summary>
        public FieldFilterRule In(params IStreamUser[] userIds)
            => InternalIn(userIds.Select(_ => _.Id));

        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> contain any of the users with provided user IDs
        /// </summary>
        public FieldFilterRule In(IEnumerable<IStreamChannelMember> userIds)
            => InternalIn(userIds.Select(_ => _.User.Id));
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.Members"/> contain any of the users with provided user IDs
        /// </summary>
        public FieldFilterRule In(params IStreamChannelMember[] userIds)
            => InternalIn(userIds.Select(_ => _.User.Id));
    }
}