using System;

namespace StreamChat.Core
{
    /// <summary>
    /// <see cref="IStreamChatClient"/> connection state
    /// </summary>
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
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
                case ConnectionState.Reconnecting:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}