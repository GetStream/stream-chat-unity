using System;
using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter channels of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity</remarks>
    public interface IChannelApi
    {
        /// <summary>
        /// <para>Queries channels.</para>
        /// You can query channels based on built-in fields as well as any custom field you add to channels.
        /// Multiple filters can be combined using AND, OR logical operators, each filter can use its
        /// comparison (equality, inequality, greater than, greater or equal, etc.).
        /// You can find the complete list of supported operators in the query syntax section of the docs.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/query_channels/?language=unity</remarks>
        Task<ChannelsResponse> QueryChannelsAsync(QueryChannelsRequest queryChannelsRequest);

        /// <summary>
        /// Create or return a channel with a given type for a list of members.
        /// This endpoint requires providing a list of members for which a single channel is maintained.
        /// Please refer to link below for more information
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity#2.-creating-a-channel-for-a-list-of-members</remarks>
        Task<ChannelState> GetOrCreateChannelAsync(string channelType, ChannelGetOrCreateRequest getOrCreateRequest);

        /// <summary>
        /// Create or return a channel with a given type and id
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity#1.-creating-a-channel-using-a-channel-id</remarks>
        Task<ChannelState> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequest getOrCreateRequest);

        /// <summary>
        /// <para>Updates a channel.</para>
        /// The update function updates all of the channel data. Any data that is present on the channel
        /// and not included in a full update will be deleted. If you don't want that, use
        /// the <see cref="UpdateChannelPartialAsync"/> method instead.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_update/?language=unity</remarks>
        Task<UpdateChannelResponse> UpdateChannelAsync(string channelType, string channelId,
            UpdateChannelRequest updateChannelRequest);

        /// <summary>
        /// Can be used to set and unset specific fields when it is necessary to retain additional custom data fields on the object.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_update/?language=unity</remarks>
        Task<UpdateChannelPartialResponse> UpdateChannelPartialAsync(string channelType, string channelId,
            UpdateChannelPartialRequest updateChannelPartialRequest);

        /// <summary>
        /// <para>Deletes multiple channels.</para>
        /// This is an asynchronous operation and the returned value is a task Id.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        Task<DeleteChannelsResponse> DeleteChannelsAsync(DeleteChannelsRequest deleteChannelsRequest);

        //StreamTodo: deprecate isHardDelete since it's no longer available in client-side SDK
        /// <summary>
        /// Deletes a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        Task<DeleteChannelResponse> DeleteChannelAsync(string channelType, string channelId, bool isHardDelete);

        /// <summary>
        /// Deletes a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        [Obsolete("Please use the other overload. This method is deprecated and will be removed in a future release")]
        Task<DeleteChannelResponse> DeleteChannelAsync(string channelType, string channelId);

        /// <summary>
        /// <para>Removes all of the messages but not affect the channel data or channel members.</para>
        /// If you want to delete both channel and message data then use <see cref="DeleteAsync"/> method instead.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/truncate_channel/?language=unity</remarks>
        Task<TruncateChannelResponse> TruncateChannelAsync(string channelType, string channelId,
            TruncateChannelRequest truncateChannelRequest);

        /// <summary>
        /// <para>Mutes a channel.</para>
        /// Messages added to a muted channel will not trigger push notifications, nor change the
        /// unread count for the users that muted it. By default, mutes stay in place indefinitely
        /// until the user removes it; however, you can optionally set an expiration time. The list
        /// of muted channels and their expiration time is returned when the user connects.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        Task<MuteChannelResponse> MuteChannelAsync(MuteChannelRequest muteChannelRequest);

        /// <summary>
        /// <para>Unmutes a channel.</para>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        Task<UnmuteResponse> UnmuteChannelAsync(UnmuteChannelRequest unmuteChannelRequest);

        /// <summary>
        /// <para>Shows a previously hidden channel.</para>
        /// Use <see cref="HideChannelAsync"/> to hide a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        Task<ShowChannelResponse> ShowChannelAsync(string channelType, string channelId,
            ShowChannelRequest showChannelRequest);

        /// <summary>
        /// <para>Removes a channel from query channel requests for that user until a new message is added.</para>
        /// Use <see cref="ShowChannelAsync"/> to cancel this operation.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/muting_channels/?language=unity</remarks>
        Task<HideChannelResponse> HideChannelAsync(string channelType, string channelId,
            HideChannelRequest hideChannelRequest);

        /// <summary>
        /// <para>Queries members of a channel.</para>
        /// The queryMembers endpoint allows you to list and paginate members for a channel. The
        /// endpoint supports filtering on numerous criteria to efficiently return member information.
        /// This endpoint is useful for channels that have large lists of members and
        /// you want to search members or if you want to display the full list of members for a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/query_members/?language=unity</remarks>
        Task<MembersResponse> QueryMembersAsync(QueryMembersRequest queryMembersRequest);

        /// <summary>
        /// Stop receiving channel events
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/watch_channel/?language=unity#stop-watching-a-channel</remarks>
        Task<StopWatchingResponse> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequest channelStopWatchingRequest);

        /// <summary>
        /// Marks channel as read up to the specific message
        /// If message ID is empty, the whole channel will be considered as read
        /// </summary>
        /// <returns></returns>
        Task<MarkReadResponse> MarkReadAsync(string channelType, string channelId,
            MarkReadRequest markReadRequest);

        /// <summary>
        /// Mark multiple channels as read. Pass a map of CID to a message ID that is considered last read by client.
        /// If message ID is empty, the whole channel will be considered as read
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/unread_channel/?language=unity</remarks>
        Task<MarkReadResponse> MarkManyReadAsync(MarkChannelsReadRequest markChannelsReadRequest);

        Task SendTypingStartEventAsync(string channelType, string channelId);

        Task SendTypingStopEventAsync(string channelType, string channelId);
    }
}