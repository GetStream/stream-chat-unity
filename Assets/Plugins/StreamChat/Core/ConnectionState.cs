using System;

namespace StreamChat.Core
{
    /// <summary>
    /// <see cref="IStreamChatClient"/> connection state
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// StreamChatClient got Disconnected from the server.
        /// If allowed by <see cref="IStreamChatClient.ReconnectStrategy"/> it will switch to WaitToReconnect and attempt to connect again after a timeout
        /// </summary>
        Disconnected,

        /// <summary>
        /// Trying to connect to server, waiting for connection handshake to complete
        /// </summary>
        Connecting,

        /// <summary>
        /// Waiting for the next connect attempt
        /// </summary>
        WaitToReconnect,

        /// <summary>
        /// Connection with server is established. StreamChatClient is ready to send and receive data
        /// </summary>
        Connected,

        /// <summary>
        /// Connection is permanently closing. There will be no reconnects made. StreamChatClient is probably being disposed
        /// </summary>
        Closing,
    }

    /// <summary>
    /// Extensions for <see cref="ConnectionState"/>
    /// </summary>
    internal static class ConnectionStateExt
    {
        public static bool IsValidToConnect(this ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.Connecting:
                case ConnectionState.Connected:
                    return false;
                case ConnectionState.Disconnected:
                case ConnectionState.WaitToReconnect:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}