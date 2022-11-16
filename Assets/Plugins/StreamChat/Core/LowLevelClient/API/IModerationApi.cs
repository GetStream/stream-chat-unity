using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    public interface IModerationApi
    {
        /// <summary>
        /// <para>Mutes a user.</para>
        /// Any user is allowed to mute another user. Mutes are stored at user level and returned with the
        /// rest of the user information when connectUser is called. A user will be be muted until the
        /// user is unmuted or the mute is expired.
        /// Use <see cref="UnmuteAsync"/> method to unmute a user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#mutes</remarks>
        Task<MuteUserResponse> MuteUserAsync(MuteUserRequest muteUserRequest);

        /// <summary>
        /// <para>Unmutes a user.</para>
        /// Any user is allowed to mute another user. Mutes are stored at user level and returned with the
        /// rest of the user information when connectUser is called. A user will be be muted until the
        /// user is unmuted or the mute is expired.
        /// Use <see cref="MuteAsync"/> method to mute a user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#mutes</remarks>
        Task<UnmuteResponse> UnmuteUserAsync(UnmuteUserRequest unmuteUserRequest);

        /// <summary>
        /// <para>Bans a user.</para>
        /// Users can be banned from an app entirely or from a channel.
        /// When a user is banned, they will not be allowed to post messages until the
        /// ban is removed or expired but will be able to connect to Chat and to channels as before.
        /// To unban a user, use <see cref="UnbanUserAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#ban</remarks>
        Task<ApiResponse> BanUserAsync(BanRequest banRequest);

        /// <summary>
        /// <para>Unbans a user.</para>
        /// Users can be banned from an app entirely or from a channel.
        /// When a user is banned, they will not be allowed to post messages until the
        /// ban is removed or expired but will be able to connect to Chat and to channels as before.
        /// To ban a user, use <see cref="BanUserAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#ban</remarks>
        Task<ApiResponse> UnbanUserAsync(UnbanRequest unbanRequest);

        /// <summary>
        /// <para>Shadow bans a user.</para>
        /// When a user is shadow banned, they will still be allowed to post messages,
        /// but any message sent during the will only be visible to the messages author
        /// and invisible to other users of the App.
        /// To remove a shadow ban, use <see cref="RemoveUserShadowBanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban</remarks>
        Task<ApiResponse> ShadowBanUserAsync(ShadowBanRequest shadowBanRequest);

        /// <summary>
        /// <para>Removes a shadow ban from a user.</para>
        /// When a user is shadow banned, they will still be allowed to post messages,
        /// but any message sent during the will only be visible to the messages author
        /// and invisible to other users of the App.
        /// To shadow ban a user, use <see cref="ShadowBanUserAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban</remarks>
        Task<ApiResponse> RemoveUserShadowBanAsync(UnbanRequest unbanRequest);

        /// <summary>
        /// <para>Queries banned users.</para>
        /// Banned users can be retrieved in different ways:
        /// 1) Using the dedicated query bans endpoint
        /// 2) User Search: you can add the banned:true condition to your search. Please note that
        /// this will only return users that were banned at the app-level and not the ones
        /// that were banned only on channels.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#query-banned-users</remarks>
        Task<QueryBannedUsersResponse> QueryBannedUsersAsync(QueryBannedUsersRequest queryBannedUsersRequest);

        /// <summary>
        /// <para>Flags a user.</para>
        /// To unflag a user, use <see cref="UnflagUserAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity</remarks>
        Task<FlagResponse> FlagUserAsync(string targetUserId);

        /// <summary>
        /// <para>Flags a message.</para>
        /// Any user is allowed to flag a message. This triggers the
        /// message.flagged webhook event and adds the message to the inbox of your Stream Dashboard Chat Moderation view.
        /// To unflag a message, use <see cref="UnflagMessageAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity</remarks>
        Task<FlagResponse> FlagMessageAsync(string targetMessageId);

        /// <summary>
        /// <para>Queries message flags.</para>
        /// If you prefer to build your own in app moderation dashboard, rather than use the Stream
        /// dashboard, then the query message flags endpoint lets you get flagged messages. Similar
        /// to other queries in Stream Chat, you can filter the flags using query operators.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#query-message-flags</remarks>
        Task<QueryMessageFlagsResponse> QueryMessageFlagsAsync(QueryMessageFlagsRequest queryMessageFlagsRequest);
    }
}