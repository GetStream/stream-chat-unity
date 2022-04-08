using System;
using System.Threading.Tasks;
using StreamChat.Core.Requests.DTO;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter users of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/unity/init_and_users/?language=unity</remarks>
    public interface IUserApi
    {
        /// <summary>
        /// <para>Allows you to search for users and see if they are online/offline.</para>
        /// You can filter and sort on the custom fields you've set for your user, the user id, and when the user was last active.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/query_users/?language=unity/</remarks>
        Task<UsersResponse> QueryUsersAsync(QueryUsersRequest queryUsersRequest);

        /// <summary>
        /// <para>Creates a guest user.</para>
        /// Guest sessions can be created client-side and do not require any server-side authentication.
        /// Support and livestreams are common use cases for guests users because
        /// often you want a visitor to be able to use chat on your application without (or before)
        /// they have a regular user account.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/authless_users/?language=unity</remarks>
        Task<GuestResponse> CreateGuestAsync(GuestRequest createGuestRequest);

        /// <summary>
        /// <para>Creates or updates users.</para>
        /// Any user present in the payload will have its data replaced with the new version.
        /// For partial updates, use <see cref="UpdateUserPartialAsync"/> method.
        /// You can send up to 100 users per API request in both upsert and partial update API.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/update_users/?language=unity#server-side-user-updates-(batch)</remarks>
        [Obsolete("Method was renamed. Please use the new " + nameof(UpsertManyUsersAsync) + ". This one will be removed in the future.")]
        Task<UpdateUsersResponse> UpsertUsersAsync(UpdateUsersRequest updateUsersRequest);

        /// <summary>
        /// <para>Creates or updates users.</para>
        /// Any user present in the payload will have its data replaced with the new version.
        /// For partial updates, use <see cref="UpdateUserPartialAsync"/> method.
        /// You can send up to 100 users per API request in both upsert and partial update API.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/update_users/?language=unity#server-side-user-updates-(batch)</remarks>
        Task<UpdateUsersResponse> UpsertManyUsersAsync(UpdateUsersRequest updateUsersRequest);

        /// <summary>
        /// <para>Partial updates a user.</para>
        /// If you need to update a subset of properties for a user(s), you can use
        /// a partial update method. Both <see cref="UpdateUserPartialRequest.Set"/> and <see cref="UpdateUserPartialRequest.Unset"/> parameters can be provided to add, modify, or
        /// remove attributes to or from the target user(s). The set and unset parameters can be used separately or combined.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/update_users/?language=unity#server-side-partial-update-(batch)</remarks>
        Task<UpdateUsersResponse> UpdateUserPartialAsync(UpdateUserPartialRequest updateUserPartialRequest);

        /// <summary>
        /// <para>Deletes a user.</para>
        /// <para>
        /// Once a user has been deleted, it cannot be un-deleted and the user_id cannot be used again.
        /// This method can only be called server-side due to security concerns, so please keep this in mind when attempting
        /// to make the call.
        /// After deleting a user, the user will no longer be able to Connect to Stream Chat, send or receive messages
        /// be displayed when querying users or have messages stored in Stream Chat
        /// (depending on whether or not <see cref="DeleteUserRequestParameters.MarkMessagesDeleted"/> is set to true or false).
        /// </para>
        /// <para>
        /// The <see cref="DeleteUserRequestParameters.MarkMessagesDeleted"/> parameter is optional. This parameter will delete all messages
        /// associated with the user.
        /// If you would like to keep message history, ensure that <see cref="DeleteUserRequestParameters.MarkMessagesDeleted"/> is set to false.
        /// </para>
        /// <para>
        /// To perform a "hard delete" on the user, you must set <see cref="DeleteUserRequestParameters.HardDelete"/> to true
        /// This will delete all messages, reactions, and any other associated data with the user.
        /// </para>
        /// <para>
        /// if <see cref="DeleteUserRequestParameters.DeleteConversationChannels"/>  set true the deleted user is removed from all one-to-one channels.
        /// </para>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/update_users/#delete-a-user</remarks>
        Task<DeleteUserResponse> DeleteUserAsync(string userId, DeleteUserRequestParameters deleteUserRequestParameters);

        /// <summary>
        /// <para>Deletes multiple users.</para>
        /// <para>
        /// It is an asynchronous operation. The returned task id can be retrieved to check operation status
        /// </para>
        /// Once a user has been deleted, it cannot be un-deleted and the user_id cannot be used again.
        /// This method can only be called server-side due to security concerns, so please keep this in mind when attempting
        /// to make the call. The <see cref="DeleteUsersRequest.MessageDeletionStrategy"/> parameter is optional.
        /// This parameter will delete all messages associated with the user.
        /// Another option is <see cref="DeleteUsersRequest.ConversationDeletionStrategy"/>, if set to Hard, the deleted user is removed from all one-to-one channels.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/update_users/#delete-a-user</remarks>
        [Obsolete("Method was renamed. Please use the new " + nameof(DeleteManyUsersAsync) + ". This one will be removed in the future.")]
        Task<DeleteUsersResponse> DeleteUsersAsync(DeleteUsersRequest deleteUsersRequest);

        /// <summary>
        /// <para>Deletes multiple users.</para>
        /// <para>
        /// It is an asynchronous operation. The returned task id can be retrieved to check operation status
        /// </para>
        /// Once a user has been deleted, it cannot be un-deleted and the user_id cannot be used again.
        /// This method can only be called server-side due to security concerns, so please keep this in mind when attempting
        /// to make the call. The <see cref="DeleteUsersRequest.MessageDeletionStrategy"/> parameter is optional.
        /// This parameter will delete all messages associated with the user.
        /// Another option is <see cref="DeleteUsersRequest.ConversationDeletionStrategy"/>, if set to Hard, the deleted user is removed from all one-to-one channels.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/update_users/#delete-a-user</remarks>
        Task<DeleteUsersResponse> DeleteManyUsersAsync(DeleteUsersRequest deleteUsersRequest);
    }
}