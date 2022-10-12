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
        /// Local user that is connected to the Stream Chat. This fields gets set after the client connection is established.
        /// </summary>
        StreamLocalUser LocalUser { get; }

        void Update();

        Task<StreamLocalUser> ConnectUserAsync(AuthCredentials userAuthCredentials,
            CancellationToken cancellationToken = default);

        Task<StreamChannel> GetOrCreateChannelAsync(string channelType, string channelId);

        Task<StreamChannel> GetOrCreateChannelAsync(ChannelType channelType, string channelId);

        Task MuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels, int? milliseconds = default);

        Task UnmuteMultipleChannelsAsync(IEnumerable<StreamChannel> channels);
    }
}