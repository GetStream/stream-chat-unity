using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Users
{
    /// <summary>
    /// Filters for <see cref="IStreamUser"/> query filters in <see cref="IStreamChatClient.QueryUsersAsync"/>
    /// </summary>
    public static class UserFilter
    {
        /// <inheritdoc cref="UserFieldId"/>
        public static UserFieldId Id { get; } = new UserFieldId();

        /// <inheritdoc cref="UserFieldRole"/>
        public static UserFieldRole Role { get; } = new UserFieldRole();

        /// <inheritdoc cref="UserFieldBanned"/>
        public static UserFieldBanned Banned { get; } = new UserFieldBanned();

        /// <inheritdoc cref="UserFieldShadowBanned"/>
        public static UserFieldShadowBanned ShadowBanned { get; } = new UserFieldShadowBanned();

        /// <inheritdoc cref="UserFieldCreatedAt"/>
        public static UserFieldCreatedAt CreatedAt { get; } = new UserFieldCreatedAt();

        /// <inheritdoc cref="UserFieldUpdatedAt"/>
        public static UserFieldUpdatedAt UpdatedAt { get; } = new UserFieldUpdatedAt();

        /// <inheritdoc cref="UserFieldLastActive"/>
        public static UserFieldLastActive LastActive { get; } = new UserFieldLastActive();

        /// <inheritdoc cref="UserFieldTeams"/>
        public static UserFieldTeams Teams { get; } = new UserFieldTeams();

        /// <inheritdoc cref="UserFieldName"/>
        public static UserFieldName Name { get; } = new UserFieldName();
    }
}