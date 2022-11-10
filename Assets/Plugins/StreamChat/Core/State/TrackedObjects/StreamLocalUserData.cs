using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State.TrackedObjects
{
    public sealed class StreamLocalUserData : StreamTrackedObjectBase<StreamLocalUserData>, IUpdateableFrom<OwnUserInternalDTO, StreamLocalUserData>
    {
        #region OwnUser

        /// <summary>
        /// Muted channels
        /// </summary>
        public IReadOnlyList<StreamChannelMute> ChannelMutes => _channelMutes;

        public IReadOnlyList<StreamDevice> Devices => _devices;

        public IReadOnlyList<string> LatestHiddenChannels => _latestHiddenChannels;

        /// <summary>
        /// Muted users
        /// </summary>
        public IReadOnlyList<StreamUserMute> Mutes => _mutes;

        public int? TotalUnreadCount { get; private set; }

        public int? UnreadChannels { get; private set; }

        public StreamUser User { get; private set; }

        #endregion

        #region User

        // /// <summary>
        // /// Expiration date of the ban
        // /// </summary>
        // public System.DateTimeOffset? BanExpires { get; private set; }
        //
        // /// <summary>
        // /// Whether a user is banned or not
        // /// </summary>
        // public bool? Banned { get; private set; }
        //
        // /// <summary>
        // /// Date/time of creation
        // /// </summary>
        // public System.DateTimeOffset? CreatedAt { get; private set; }
        //
        // /// <summary>
        // /// Date of deactivation
        // /// </summary>
        // public System.DateTimeOffset? DeactivatedAt { get; private set; }
        //
        // /// <summary>
        // /// Date/time of deletion
        // /// </summary>
        // public System.DateTimeOffset? DeletedAt { get; private set; }
        //
        // /// <summary>
        // /// Unique user identifier
        // /// </summary>
        // public string Id { get; private set; }
        //
        // public bool? Invisible { get; private set; }
        //
        // /// <summary>
        // /// Preferred language of a user
        // /// </summary>
        // public string Language { get; private set; }
        //
        // /// <summary>
        // /// Date of last activity
        // /// </summary>
        // public System.DateTimeOffset? LastActive { get; private set; }
        //
        // /// <summary>
        // /// Whether a user online or not
        // /// </summary>
        // public bool? Online { get; private set; }
        //
        // public StreamPushNotificationSettings PushNotifications { get; private set; }
        //
        // /// <summary>
        // /// Revocation date for tokens
        // /// </summary>
        // public System.DateTimeOffset? RevokeTokensIssuedBefore { get; private set; }
        //
        // /// <summary>
        // /// Determines the set of user permissions
        // /// </summary>
        // public string Role { get; private set; }
        //
        // /// <summary>
        // /// List of teams user is a part of
        // /// </summary>
        // public IReadOnlyList<string> Teams => _teams;
        //
        // /// <summary>
        // /// Date/time of the last update
        // /// </summary>
        // public System.DateTimeOffset? UpdatedAt { get; private set; }
        //
        // //Not in API
        // public string Name;
        // public string Image;

        #endregion

        internal StreamLocalUserData(string uniqueId, ICacheRepository<StreamLocalUserData> repository, ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        protected override string InternalUniqueId { get; set; }
        protected override StreamLocalUserData Self => this;

        void IUpdateableFrom<OwnUserInternalDTO, StreamLocalUserData>.UpdateFromDto(OwnUserInternalDTO dto, ICache cache)
        {
            #region OwnUser

            _channelMutes.TryReplaceRegularObjectsFromDto(dto.ChannelMutes, cache);
            _devices.TryReplaceRegularObjectsFromDto(dto.Devices, cache);
            _latestHiddenChannels.TryReplaceValuesFromDto(dto.LatestHiddenChannels);
            _mutes.TryReplaceRegularObjectsFromDto(dto.Mutes, cache);

            TotalUnreadCount = GetOrDefault(dto.TotalUnreadCount, TotalUnreadCount);
            UnreadChannels = GetOrDefault(dto.UnreadChannels, UnreadChannels);
            //UnreadCount = dto.UnreadCount; Deprecated

            #endregion

            User = cache.Users.CreateOrUpdate<StreamUser, OwnUserInternalDTO>(dto, out _);

            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        private readonly List<StreamChannelMute> _channelMutes = new List<StreamChannelMute>();
        private readonly List<StreamDevice> _devices = new List<StreamDevice>();
        private readonly List<string> _latestHiddenChannels = new List<string>();
        private readonly List<StreamUserMute> _mutes = new List<StreamUserMute>();
    }
}