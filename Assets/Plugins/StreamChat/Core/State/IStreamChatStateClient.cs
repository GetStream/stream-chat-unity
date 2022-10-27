using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamChat.Core.State.Responses;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs.Auth;

namespace StreamChat.Core.State
{
    public interface IStreamChatStateClient : IDisposable
    {
        /// <summary>
        /// Triggered when connection with Stream Chat server is established
        /// </summary>
        event ConnectionMadeHandler Connected;

        /// <summary>
        /// Triggered when connection with Stream Chat server is lost
        /// </summary>
        event Action Disconnected;

        /// <summary>
        /// Triggered when connection state with Stream Chat server has changed
        /// </summary>
        event ConnectionChangeHandler ConnectionStateChanged;

        /// <summary>
        /// Current connection state
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Local user that is connected to the Stream Chat. This fields gets set after the client connection is established.
        /// </summary>
        StreamLocalUser LocalUserData { get; }

        /// <summary>
        /// Channels loaded via <see cref="GetOrCreateChannelAsync"/> and <see cref="QueryChannelsAsync"/>
        ///
        /// These channels are receiving automatic updates
        /// </summary>
        IEnumerable<StreamChannel> WatchedChannels { get; }

        void Update();

        Task<StreamLocalUser> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create or return a channel with a given type and id
        ///
        /// Use this to create general purpose channel for unspecified group of users
        ///
        /// If you want to create a channel for a dedicated group of users e.g. private conversation use the other overload <see cref="GetOrCreateChannelAsync(StreamChat.Core.State.ChannelType,IEnumerable{string})"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/creating_channels/?language=unity#1.-creating-a-channel-using-a-channel-id</remarks>
        Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId,
            IChannelCustomData optionalCustomData = null);

        Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, IEnumerable<StreamUser> members,
            IChannelCustomData optionalCustomData = null);

        /// <summary>
        /// Mute channels with optional duration in milliseconds
        /// </summary>
        /// <param name="channels">Channels to mute</param>
        /// <param name="milliseconds">[Optional] Duration in milliseconds</param>
        Task MuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels, int? milliseconds = default);

        Task UnmuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels);

        /// <summary>
        /// Delete multiple channels. This in an asynchronous server operation meaning it may still be executing when this method Task is completed.
        /// Response contains <see cref="StreamDeleteChannelsResponse.TaskId"/> which can be used to check the status of the delete operation
        /// </summary>
        /// <param name="channels">Collection of <see cref="StreamChannel"/> to delete</param>
        /// <param name="isHardDelete">Hard delete removes channels entirely whereas Soft Delete deletes them from client but still allows to access them from the server-side SDK</param>
        /// <remarks>https://getstream.io/chat/docs/unity/channel_delete/?language=unity</remarks>
        Task<StreamDeleteChannelsResponse> DeleteMultipleChannelsAsync(IEnumerable<StreamChannel> channels,
            bool isHardDelete = false);

        Task DisconnectUserAsync();
    }
}