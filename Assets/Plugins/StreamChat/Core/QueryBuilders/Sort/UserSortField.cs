using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Fields that you can use to sort <see cref="IStreamUser"/> query results when using <see cref="IStreamChatClient.QueryUsersAsync"/>
    /// </summary>
    public readonly struct UserSortField
    {
        public static UserSortField Name => new UserSortField("name");
        public static UserSortField Role => new UserSortField("role");
        public static UserSortField Banned => new UserSortField("banned");
        public static UserSortField ShadowBanned => new UserSortField("shadow_banned");
        public static UserSortField CreatedAt => new UserSortField("created_at");
        public static UserSortField UpdatedAt => new UserSortField("updated_at");
        public static UserSortField LastActive => new UserSortField("last_active");
        public static UserSortField Teams => new UserSortField("teams");

        /// <summary>
        /// Sort by your custom field of <see cref="IStreamUser"/>
        /// </summary>
        public static UserSortField CustomField(string fieldName) => new UserSortField(fieldName);

        internal readonly string FieldName;

        public UserSortField(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}