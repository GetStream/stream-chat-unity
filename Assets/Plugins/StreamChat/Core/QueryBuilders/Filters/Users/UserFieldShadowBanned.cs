using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Users
{
    /// <summary>
    /// Filter by <see cref="IStreamUser.ShadowBanned"/>
    /// </summary>
    public sealed class UserFieldShadowBanned : BaseFieldToFilter
    {
        public override string FieldName => "shadow_banned";
        
        /// <summary>
        /// Return only users where <see cref="IStreamUser.ShadowBanned"/> state is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(bool isShadowBanned) => InternalEqualsTo(isShadowBanned);
    }
}