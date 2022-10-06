using StreamChat.Core.Models;

namespace StreamChat.Core.State.Models
{
    //Todo: we should depend on DTO and not OwnUser

    public class StreamLocalUser : StreamTrackedObjectBase<StreamLocalUser>, IUpdateableFrom<OwnUser, StreamLocalUser>
    {
        #region OwnUser

        public System.Collections.Generic.List<ChannelMute> ChannelMutes { get; set; }

        public System.Collections.Generic.List<Device> Devices { get; set; }

        public System.Collections.Generic.List<string> LatestHiddenChannels { get; set; }

        public System.Collections.Generic.List<UserMute> Mutes { get; set; }

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
        public System.Collections.Generic.List<string> Teams { get; set; }

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

        internal static StreamLocalUser Create(string uniqueId, IRepository<StreamLocalUser> repository)
            => new StreamLocalUser(uniqueId, repository);

        internal StreamLocalUser(string uniqueId, IRepository<StreamLocalUser> repository)
            : base(uniqueId, repository)
        {
        }

        protected override string InternalUniqueId { get; set; }
        protected override StreamLocalUser Self => this;

        void IUpdateableFrom<OwnUser, StreamLocalUser>.UpdateFromDto(OwnUser dto, ICache cache)
        {
            //AdditionalProperties = dto.AdditionalProperties; //Todo: Add additional properties
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
        }
    }
}