using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;
using StreamChat.Core.State.Models;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State.TrackedObjects
{
    public class StreamLocalUser : StreamTrackedObjectBase<StreamLocalUser>, IUpdateableFrom<OwnUserInternalDTO, StreamLocalUser>
    {
        #region OwnUser

        public IReadOnlyList<StreamChannelMute> ChannelMutes => _channelMutes;

        public IReadOnlyList<StreamDevice> Devices => _devices;

        public IReadOnlyList<string> LatestHiddenChannels => _latestHiddenChannels;

        public IReadOnlyList<StreamUserMute> Mutes => _mutes;

        public int? TotalUnreadCount { get; set; }

        public int? UnreadChannels { get; set; }

        public int? UnreadCount { get; set; }

        #endregion

        #region User

        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public System.DateTimeOffset? BanExpires { get; set; }

        /// <summary>
        /// Whether a user is banned or not
        /// </summary>
        public bool? Banned { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Date of deactivation
        /// </summary>
        public System.DateTimeOffset? DeactivatedAt { get; set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public System.DateTimeOffset? DeletedAt { get; set; }

        /// <summary>
        /// Unique user identifier
        /// </summary>
        public string Id { get; set; }

        public bool? Invisible { get; set; }

        /// <summary>
        /// Preferred language of a user
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Date of last activity
        /// </summary>
        public System.DateTimeOffset? LastActive { get; set; }

        /// <summary>
        /// Whether a user online or not
        /// </summary>
        public bool? Online { get; set; }

        public PushNotificationSettings PushNotifications { get; set; } //Todo custom type?

        /// <summary>
        /// Revocation date for tokens
        /// </summary>
        public System.DateTimeOffset? RevokeTokensIssuedBefore { get; set; }

        /// <summary>
        /// Determines the set of user permissions
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// List of teams user is a part of
        /// </summary>
        public List<string> Teams { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        //Not in API
        public string Name;
        public string Image;

        #endregion

        public void UpdateFrom(OwnUser ownUser)
        {
        }

        internal StreamLocalUser(string uniqueId, IRepository<StreamLocalUser> repository, ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        protected override string InternalUniqueId { get; set; }
        protected override StreamLocalUser Self => this;

        void IUpdateableFrom<OwnUserInternalDTO, StreamLocalUser>.UpdateFromDto(OwnUserInternalDTO dto, ICache cache)
        {
            #region OwnUser

            _channelMutes.TryReplaceRegularObjectsFromDto(dto.ChannelMutes, cache);
            _devices.TryReplaceRegularObjectsFromDto(dto.Devices, cache);
            //_latestHiddenChannels = dto.LatestHiddenChannels;
            _mutes.TryReplaceRegularObjectsFromDto(dto.Mutes, cache);

            TotalUnreadCount = dto.TotalUnreadCount;
            UnreadChannels = dto.UnreadChannels;
            UnreadCount = dto.UnreadCount;

            #endregion

            //BanExpires = dto.BanExpires;
            Banned = dto.Banned;
            CreatedAt = dto.CreatedAt;
            DeactivatedAt = dto.DeactivatedAt;
            DeletedAt = dto.DeletedAt;
            Id = dto.Id;
            Invisible = dto.Invisible;
            Language = dto.Language;
            LastActive = dto.LastActive;
            Online = dto.Online;
            //PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications);
            //RevokeTokensIssuedBefore = dto.RevokeTokensIssuedBefore;
            Role = dto.Role;
            Teams = dto.Teams;
            UpdatedAt = dto.UpdatedAt;

            //Not in API spec
            //Name = dto.Name;
            //Image = dto.Image;

            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        private readonly List<StreamChannelMute> _channelMutes = new List<StreamChannelMute>();
        private readonly List<StreamDevice> _devices = new List<StreamDevice>();
        private readonly List<string> _latestHiddenChannels = new List<string>();
        private readonly List<StreamUserMute> _mutes = new List<StreamUserMute>();
    }
}