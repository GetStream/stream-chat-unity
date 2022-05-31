using System.Threading.Tasks;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
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
        Task<MuteUserResponse> UnmuteUserAsync(MuteUserRequest muteUserRequest);

        /// <summary>
        /// <para>Bans a user.</para>
        /// Users can be banned from an app entirely or from a channel.
        /// When a user is banned, they will not be allowed to post messages until the
        /// ban is removed or expired but will be able to connect to Chat and to channels as before.
        /// To unban a user, use <see cref="UnbanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#ban</remarks>
        Task<ApiResponse> BanAsync(BanRequest banRequest);

        /// <summary>
        /// <para>Unbans a user.</para>
        /// Users can be banned from an app entirely or from a channel.
        /// When a user is banned, they will not be allowed to post messages until the
        /// ban is removed or expired but will be able to connect to Chat and to channels as before.
        /// To ban a user, use <see cref="BanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#ban</remarks>
        Task<ApiResponse> UnbanAsync(string userId, string channelId, string channelType);

        /// <summary>
        /// <para>Shadow bans a user.</para>
        /// When a user is shadow banned, they will still be allowed to post messages,
        /// but any message sent during the will only be visible to the messages author
        /// and invisible to other users of the App.
        /// To remove a shadow ban, use <see cref="RemoveShadowBanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban</remarks>
        Task<ApiResponse> ShadowBanAsync(ShadowBanRequest shadowBanRequest);

        /// <summary>
        /// <para>Removes a shadow ban from a user.</para>
        /// When a user is shadow banned, they will still be allowed to post messages,
        /// but any message sent during the will only be visible to the messages author
        /// and invisible to other users of the App.
        /// To shadow ban a user, use <see cref="ShadowBanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban</remarks>
        Task<ApiResponse> RemoveShadowBanAsync(string userId, string channelId, string channelType);

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
    }
}