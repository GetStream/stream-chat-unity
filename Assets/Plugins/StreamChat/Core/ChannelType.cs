using System;
using StreamChat.Libs.Utils;

namespace StreamChat.Core
{
    /// <summary>
    /// Channel Type determines default permission settings
    ///
    /// You can use predefined channel types:
    /// - <see cref="ChannelType.Messaging"/>
    /// - <see cref="ChannelType.Livestream"/>
    /// - <see cref="ChannelType.Team"/>
    /// - <see cref="ChannelType.Commerce"/>
    /// - <see cref="ChannelType.Gaming"/>
    /// - or create a custom channel type in Stream Dashboard and use <see cref="ChannelType.Custom"/> with your own channel type key
    ///
    /// Read more:
    /// <see href="https://getstream.io/chat/docs/unity/permissions_reference/?language=unity">Permissions tables</see>
    /// <see href="https://getstream.io/chat/docs/unity/channel_features/?language=unity">Channel Types</see>
    /// </summary>
    public readonly struct ChannelType
    {
        public bool IsValid => !_channelTypeKey.IsNullOrEmpty();

        public static readonly ChannelType Messaging = new ChannelType("messaging");
        public static ChannelType Livestream => new ChannelType("livestream");
        public static ChannelType Team => new ChannelType("team");
        public static ChannelType Commerce => new ChannelType("commerce");
        public static ChannelType Gaming => new ChannelType("gaming");

        public static ChannelType Custom(string channelTypeKey) => new ChannelType(channelTypeKey);

        public ChannelType(string channelTypeKey)
        {
            if (channelTypeKey.IsNullOrEmpty())
            {
                throw new ArgumentException(
                    $"{channelTypeKey} cannot be null or empty. Use predefined channel types: {nameof(Messaging)}, " +
                    $"{nameof(Livestream)}, {nameof(Team)}, {nameof(Commerce)}, {nameof(Gaming)}, or create custom one in your Dashboard and use {nameof(Custom)}");
            }

            _channelTypeKey = channelTypeKey;
        }

        public static implicit operator string(ChannelType channelType) => channelType._channelTypeKey;

        public override string ToString() => _channelTypeKey;

        private readonly string _channelTypeKey;
    }
}