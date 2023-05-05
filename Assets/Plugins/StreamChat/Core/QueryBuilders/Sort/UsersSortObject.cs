namespace StreamChat.Core.QueryBuilders.Sort
{
    /// <summary>
    /// Sort object for <see cref="IStreamUser"/> query: <see cref="IStreamChatClient.QueryUsersAsync"/>
    /// </summary>
    public sealed class UsersSortObject : QuerySort<UsersSortObject, UserSortField>
    {
        protected override UsersSortObject Instance => this;

        protected override string ToUnderlyingFieldName(UserSortField field) => field.FieldName;
    }
}