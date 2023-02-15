using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.Id"/> of a user who created the <see cref="IStreamChannel"/>
    /// </summary>
    public sealed class ChannelFieldCreatedById : BaseFieldToFilter
    {
        public override string FieldName => "created_by_id";
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedBy"/> is EQUAL to the provided user ID
        /// </summary>
        public FieldFilterRule EqualsTo(string userId) => InternalEqualsTo(userId);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedBy"/> is EQUAL to the provided user
        /// </summary>
        public FieldFilterRule EqualsTo(IStreamUser user) => InternalEqualsTo(user.Id);
        
        /// <summary>
        /// Return only channels where <see cref="IStreamChannel.CreatedBy"/> is EQUAL to the local user
        /// </summary>
        public FieldFilterRule EqualsTo(IStreamLocalUserData localUserData) => InternalEqualsTo(localUserData.UserId);
    }
}