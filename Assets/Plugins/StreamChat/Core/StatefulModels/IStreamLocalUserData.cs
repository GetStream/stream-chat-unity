using System.Collections.Generic;
using StreamChat.Core.Models;

namespace StreamChat.Core.StatefulModels
{
    /// <summary>
    /// Data of the local <see cref="IStreamUser"/> connected with <see cref="IStreamChatClient"/> to the Stream Chat Server
    /// Use <see cref="IStreamLocalUserData.User"/> to get local <see cref="IStreamUser"/> reference
    /// </summary>
    public interface IStreamLocalUserData : IStreamStatefulModel
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
        string UserId { get; }
    }
}