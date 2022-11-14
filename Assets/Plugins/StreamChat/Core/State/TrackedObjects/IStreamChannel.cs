using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.Models;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.Requests;
using StreamChat.Core.State.Responses;

namespace StreamChat.Core.State.TrackedObjects
{
    /// <summary>
    /// Channel is where group of <see cref="IStreamChannelMember"/>s can send messages.
    /// Default permissions and configuration is based on <see cref="StreamChannel.Type"/>
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/unity/permissions_reference/?language=unity&q=hidden#default-grants</remarks>
    public interface IStreamChannel : IStreamTrackedObject
    {
        /// <summary>
        /// Event fired when a new <see cref="IStreamMessage"/> was received on this channel
        /// </summary>
        event StreamChannelMessageHandler MessageReceived;
        
        /// <summary>
        /// Event fired when a <see cref="IStreamMessage"/> from this channel was updated
        /// </summary>
        event StreamChannelMessageHandler MessageUpdated;

        /// <summary>
        /// Event fired when a <see cref="IStreamMessage"/> from this channel was deleted
        /// </summary>
        event StreamMessageDeleteHandler MessageDeleted;

        /// <summary>
        /// Event fired when a new <see cref="IStreamChannelMember"/> joined this channel
        /// </summary>
        event StreamChannelMemberChangeHandler MemberAdded;

        /// <summary>
        /// Event fired when a <see cref="IStreamChannelMember"/> left this channel
        /// </summary>
        event StreamChannelMemberChangeHandler MemberRemoved; //StreamTodo: Unifiy Removed or Deleted

        /// <summary>
        /// Event fired when a <see cref="IStreamChannelMember"/> was updated
        /// </summary>
        event StreamChannelMemberChangeHandler MemberUpdated;

        /// <summary>
        /// Event fired when visibility of this channel changed. Check <see cref="IStreamChannel.Hidden"/> to know if channel is hidden
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity&q=hidden#hiding-a-channel</remarks>
        event StreamChannelVisibilityHandler VisibilityChanged;

        /// <summary>
        /// Event fired when channel got muted on unmuted. Check <see cref="IStreamChannel.Muted"/> and <see cref="IStreamChannel.MuteExpiresAt"/> 
        /// </summary>
        event StreamChannelMuteHandler MuteChanged;

        /// <summary>
        /// Event fired when this channel was truncated meaning that all or part of the messages where removed
        /// </summary>
        event StreamChannelChangeHandler Truncated;

        /// <summary>
        /// Event fired when this channel data was updated
        /// </summary>
        event StreamChannelChangeHandler Updated;

        /// <summary>
        /// Event fired when a <see cref="IStreamUser"/> started watching this channel
        /// See also <see cref="WatcherCount"/> and <see cref="Watchers"/>
        /// </summary>
        event StreamChannelUserChangeHandler WatcherAdded;

        /// <summary>
        /// Event fired when a <see cref="IStreamUser"/> stopped watching this channel
        /// See also <see cref="WatcherCount"/> and <see cref="Watchers"/>
        /// </summary>
        event StreamChannelUserChangeHandler WatcherRemoved;

        /// <summary>
        /// Event fired when a <see cref="IStreamUser"/> in this channel starts typing
        /// </summary>
        event StreamChannelUserChangeHandler UserStartedTyping;

        /// <summary>
        /// Event fired when a <see cref="IStreamUser"/> in this channel stops typing
        /// </summary>
        event StreamChannelUserChangeHandler UserStoppedTyping;

        /// <summary>
        /// Whether auto translation is enabled or not
        /// </summary>
        bool AutoTranslationEnabled { get; }

        /// <summary>
        /// Language to translate to when auto translation is active
        /// </summary>
        string AutoTranslationLanguage { get; }

        /// <summary>
        /// Channel CID (type:id)
        /// </summary>
        string Cid { get; }

        /// <summary>
        /// Channel configuration
        /// </summary>
        StreamChannelConfig Config { get; }

        /// <summary>
        /// Cooldown period after sending each message
        /// </summary>
        int? Cooldown { get; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Creator of the channel
        /// </summary>
        IStreamUser CreatedBy { get; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        DateTimeOffset? DeletedAt { get; }

        bool Disabled { get; }

        /// <summary>
        /// Whether channel is frozen or not
        /// </summary>
        bool Frozen { get; }

        /// <summary>
        /// Whether this channel is hidden by current user or not. Subscribe to <see cref="VisibilityChanged"/> to get notified when this property changes
        /// </summary>
        bool Hidden { get; }

        /// <summary>
        /// Date since when the message history is accessible
        /// </summary>
        DateTimeOffset? HideMessagesBefore { get; }

        /// <summary>
        /// Channel unique ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Date of the last message sent
        /// </summary>
        DateTimeOffset? LastMessageAt { get; }

        /// <summary>
        /// Number of members in the channel
        /// </summary>
        int MemberCount { get; }

        /// <summary>
        /// List of channel members (max 100)
        /// </summary>
        IReadOnlyList<IStreamChannelMember> Members { get; }

        /// <summary>
        /// Date of mute expiration
        /// </summary>
        DateTimeOffset? MuteExpiresAt { get; }

        /// <summary>
        /// Whether this channel is muted or not. Subscribe to <see cref="MuteChanged"/> to get notified when this property changes
        /// </summary>
        bool Muted { get; }

        /// <summary>
        /// List of channel capabilities of authenticated user
        /// </summary>
        IReadOnlyList<string> OwnCapabilities { get; }

        /// <summary>
        /// Team the channel belongs to (multi-tenant only)
        /// </summary>
        string Team { get; }

        DateTimeOffset? TruncatedAt { get; }
        IStreamUser TruncatedBy { get; }

        /// <summary>
        /// Type of the channel
        /// </summary>
        ChannelType Type { get; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        DateTimeOffset? UpdatedAt { get; }

        string Name { get; }

        /// <summary>
        /// Current user membership object
        /// </summary>
        IStreamChannelMember Membership { get; }

        /// <summary>
        /// List of channel messages. By default only latest messages are loaded. If you wish to load older messages user the <see cref="LoadOlderMessagesAsync"/>
        /// </summary>
        IReadOnlyList<IStreamMessage> Messages { get; }

        /// <summary>
        /// Pending messages that this user has sent
        /// </summary>
        IReadOnlyList<StreamPendingMessage> PendingMessages { get; }

        /// <summary>
        /// List of pinned messages in the channel
        /// </summary>
        IReadOnlyList<IStreamMessage> PinnedMessages { get; }

        /// <summary>
        /// List of read states
        /// </summary>
        IReadOnlyList<StreamRead> Read { get; }

        /// <summary>
        /// Number of channel watchers
        /// </summary>
        int WatcherCount { get; }

        /// <summary>
        /// List of user who is watching the channel
        /// Subscribe to <see cref="WatcherAdded"/> and <see cref="WatcherRemoved"/> events to know when this list changes.
        /// </summary>
        IReadOnlyList<IStreamUser> Watchers { get; }

        /// <summary>
        /// List of currently typing users.
        /// Subscribe to <see cref="UserStartedTyping"/> and <see cref="UserStoppedTyping"/> events to know when this list changes.
        /// </summary>
        IReadOnlyList<IStreamUser> TypingUsers { get; }

        /// <summary>
        /// Is this a direct message channel between the local and some other user
        /// </summary>
        bool IsDirectMessage { get; }

        /// <summary>
        /// Basic send message method. If you want to set additional parameters like use the other <see cref="IStreamChannel.SendNewMessageAsync(StreamChat.Core.State.Requests.StreamSendMessageRequest)"/> overload
        /// </summary>
        Task<IStreamMessage> SendNewMessageAsync(string message);

        /// <summary>
        /// Advanced send message method. Check out the <see cref="StreamSendMessageRequest"/> to see all of the parameters
        /// </summary>
        Task<IStreamMessage> SendNewMessageAsync(StreamSendMessageRequest sendMessageRequest);

        Task LoadOlderMessagesAsync();

        /// <summary>
        /// Update channel in a complete overwrite mode.
        /// Important! Any data that is present on the channel and not included in a full update will be deleted.
        ///
        /// If you want to update only some fields of the channel use the <see cref="IStreamChannel.UpdatePartialAsync"/>
        /// </summary>
        Task UpdateOverwriteAsync(); //StreamTodo: NOT IMPLEMENTED

        /// <summary>
        /// Update channel in a partial mode. You can selectively set and unset fields of the channel
        ///
        /// If you want to completely overwrite the channel use the <see cref="IStreamChannel.UpdateOverwriteAsync"/>
        /// </summary>
        /// StreamTodo: this should be more high level, maybe use enum with predefined field names?
        Task UpdatePartialAsync(IDictionary<string, object> setFields,
            IEnumerable<string> unsetFields = null);

        /// <summary>
        /// Upload file to stream CDN. Returned file URL can be used as a message attachment.
        /// For image files use <see cref="IStreamChannel.UploadImageAsync"/> as it will generate the thumbnail and allow for image resize and crop operations
        /// </summary>
        /// <param name="fileContent">File bytes content (e.g. returned from <see cref="System.IO.File.ReadAllBytes"/></param>
        /// <param name="fileName">Name of the file</param>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity</remarks>
        Task<StreamFileUploadResponse> UploadFileAsync(byte[] fileContent, string fileName);

        /// <summary>
        /// Delete file of any type that was send to Stream CDN.
        /// This handles both files sent via <see cref="IStreamChannel.UploadFileAsync"/> and images sent via <see cref="IStreamChannel.UploadImageAsync"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity#deleting-files-and-images</remarks>
        Task DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Upload image file to stream CDN. The returned image URL can be injected into <see cref="StreamAttachmentRequest"/> when sending new message.
        /// </summary>
        /// <param name="imageContent"></param>
        /// <param name="imageName"></param>
        /// <remarks>https://getstream.io/chat/docs/unity/file_uploads/?language=unity#how-to-upload-a-file-or-image</remarks>
        Task<StreamImageUploadResponse> UploadImageAsync(byte[] imageContent, string imageName);

        void QueryMembers() //StreamTodo: IMPLEMENT
            ;

        void QueryWatchers() //StreamTodo: IMPLEMENT
            ;

        /// <summary>
        /// Ban user from this channel.
        /// If you wish to ban user completely from all of the channels, this can be done only by server-side SDKs.
        /// </summary>
        /// <param name="user">User to ban from channel</param>
        /// <param name="isShadowBan">Shadow banned user is not notified about the ban. Read more: <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#shadow-ban</remarks></param>
        /// <param name="reason">[Optional] reason description why user got banned</param>
        /// <param name="timeoutMinutes">[Optional] timeout in minutes after which ban is automatically expired</param>
        /// <param name="isIpBan">[Optional] Should ban apply to user's IP address</param>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#ban</remarks>
        Task BanUserFromChannelAsync(IStreamUser user, bool isShadowBan = false, string reason = "",
            int? timeoutMinutes = default, bool isIpBan = false);

        /// <summary>
        /// Remove ban from the user on this channel
        /// </summary>
        Task UnbanUserInChannelAsync(IStreamUser user);

        /// <summary>
        /// Mark this message as the last that was read by this user in this channel
        /// If you want to mark whole channel as read use the <see cref="IStreamChannel.MarkChannelReadAsync"/>
        ///
        /// This feature allows to track to which message users have read the channel
        /// </summary>
        Task MarkMessageReadAsync(IStreamMessage message);

        /// <summary>
        /// Mark this channel completely as read
        /// If you want to mark specific message as read use the <see cref="IStreamChannel.MarkMessageReadAsync"/>
        ///
        /// This feature allows to track to which message users have read the channel
        /// </summary>
        Task MarkChannelReadAsync();

        /// <summary>
        /// <para>Shows a previously hidden channel.</para>
        /// Use <see cref="IStreamChannel.HideAsync"/> to hide a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        Task ShowAsync();

        /// <summary>
        /// <para>Removes a channel from query channel requests for that user until a new message is added.</para>
        /// Use <see cref="IStreamChannel.ShowAsync"/> to cancel this operation.
        /// </summary>
        /// <param name="clearHistory">Whether to clear message history of the channel or not</param>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        Task HideAsync(bool? clearHistory = false);

        /// <summary>
        /// Add users as members to this channel
        /// </summary>
        /// <param name="users">Users to become members of this channel</param>
        Task AddMembersAsync(IEnumerable<IStreamUser> users);

        /// <summary>
        /// Remove members from this channel
        /// </summary>
        /// <param name="members">Members to remove</param>
        Task RemoveMembersAsync(IEnumerable<ChannelMember> members);

        /// <summary>
        /// Mute channel with optional duration in milliseconds
        /// </summary>
        /// <param name="milliseconds">[Optional] Duration in milliseconds</param>
        Task MuteChannelAsync(int? milliseconds = default);

        /// <summary>
        /// Unmute channel
        /// </summary>
        Task UnmuteChannelAsync();

        /// <summary>
        /// Truncate removes all of the messages but does not affect the channel data or channel members. If you want to delete both messages and channel data then use the <see cref="IStreamChannel.DeleteAsync"/> method instead.
        /// </summary>
        /// <param name="truncatedAt">[Optional]truncate channel up to given time. If not set then all messages are truncated</param>
        /// <param name="systemMessage">A system message to be added via truncation.</param>
        /// <param name="skipPushNotifications">Don't send a push notification for <param name="systemMessage"/>.</param>
        /// <param name="isHardDelete">if truncation should delete messages instead of hiding</param>
        Task TruncateAsync(DateTimeOffset? truncatedAt = default, string systemMessage = "",
            bool skipPushNotifications = false, bool isHardDelete = false);

        /// <summary>
        /// Delete this channel. By default channel is soft deleted. You may hard delete it by setting the <param name="isHardDelete"> argument to true
        /// </summary>
        /// <param name="isHardDelete">Hard delete completely removes channel with all its resources</param>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        Task DeleteAsync(bool isHardDelete);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task SendTypingStartedEventAsync();

        Task SendTypingStoppedEventAsync();
    }
}