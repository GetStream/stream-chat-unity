using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Users
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.Banned"/>
    /// </summary>
    public sealed class UserFieldBanned : BaseFieldToFilter
    {
        public override string FieldName => "banned";
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.Banned"/> state is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool isBanned) => InternalEqualsTo(isBanned);
    }
}