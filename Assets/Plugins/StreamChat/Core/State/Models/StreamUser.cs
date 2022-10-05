using StreamChat.Core.Models;
using StreamChat.Core.State;
using StreamChat.Core.StreamChat.Core.State;

namespace StreamChat.Core.State.Models
{
    public class StreamUser : StreamTrackedObjectBase<StreamUser>
    {
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

        public PushNotificationSettings PushNotifications { get; set; }

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

        internal static StreamUser Create(string uniqueId, IRepository<StreamUser> repository)
            => new StreamUser(uniqueId, repository);

        internal StreamUser(string uniqueId, IRepository<StreamUser> repository)
            : base(uniqueId, repository)
        {
        }

        protected override StreamUser Self => this;

        protected override string InternalUniqueId
        {
            get => Id;
            set => Id = value;
        }
    }
}