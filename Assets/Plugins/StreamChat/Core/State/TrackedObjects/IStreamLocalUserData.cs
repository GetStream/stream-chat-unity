using System.Collections.Generic;
using StreamChat.Core.State;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State.TrackedObjects
{
    /// <summary>
    /// Data of the local <see cref="IStreamUser"/> connected with <see cref="IStreamChatStateClient"/> to the Stream Chat Server
    /// Use <see cref="IStreamLocalUserData.User"/> to get local <see cref="IStreamUser"/> reference
    /// </summary>
    public interface IStreamLocalUserData : IStreamTrackedObject
    {
        /// <summary>
        /// Muted channels
        /// </summary>
        IReadOnlyList<StreamChannelMute> ChannelMutes { get; }

        IReadOnlyList<StreamDevice> Devices { get; }
        IReadOnlyList<string> LatestHiddenChannels { get; }

        /// <summary>
        /// Muted users
        /// </summary>
        IReadOnlyList<StreamUserMute> Mutes { get; }

        int? TotalUnreadCount { get; }
        int? UnreadChannels { get; }
        IStreamUser User { get; }
    }
}