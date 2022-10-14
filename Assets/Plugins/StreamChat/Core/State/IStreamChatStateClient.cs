using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        StreamLocalUser LocalUser { get; }

        /// <summary>
        /// Channels loaded via <see cref="GetOrCreateChannelAsync"/> and <see cref="QueryChannelsAsync"/>
        ///
        /// These channels are receiving automatic updates
        /// </summary>
        IEnumerable<StreamChannel> LoadedChannels { get; }

        void Update();

        Task<StreamLocalUser> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default);

        Task<StreamChannel> GetOrCreateChannelAsync(string channelType, string channelId);

        Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId);

        Task MuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels, int? milliseconds = default);

        Task UnmuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels);

        /// <summary>
        /// Delete multiple channels
        /// </summary>
        /// <param name="cids">Collection of <see cref="StreamChannel.Cid"/></param>
        /// <param name="isHardDelete">`Hard delete` removes channels entirely whereas `Soft Delete` deletes them from client but still allows to access them from the server-side SDK</param>
        Task DeleteMultipleChannelsAsync(IEnumerable<string> cids, bool isHardDelete = false);

        Task DisconnectUserAsync();
    }
}