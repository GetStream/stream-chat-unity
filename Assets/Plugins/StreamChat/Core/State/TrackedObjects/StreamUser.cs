using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Models;

namespace StreamChat.Core.State.TrackedObjects
{
    /// <summary>
    /// Stream user represents a single chat user that can be a member of multiple channels
    ///
    /// This object is tracked by <see cref="StreamChatStateClient"/> meaning its state will be automatically updated
    /// </summary>
    public class StreamUser : StreamTrackedObjectBase<StreamUser>, IUpdateableFrom<UserObjectInternalInternalDTO, StreamUser>
        , IUpdateableFrom<UserResponseInternalDTO, StreamUser>
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

        public PushNotificationSettings PushNotifications { get; set; } //StreamTodo custom type?

        /// <summary>
        /// Revocation date for tokens
        /// </summary>
        public System.DateTimeOffset? RevokeTokensIssuedBefore { get; set; }

        /// <summary>
        /// Determines the set of user permissions
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Whether user is shadow banned or not
        /// </summary>
        public bool? ShadowBanned { get; set; }

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

        public Task FlagAsync() => LowLevelClient.InternalModerationApi.FlagUserAsync(Id);



        internal StreamUser(string uniqueId, IRepository<StreamUser> repository, ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        void IUpdateableFrom<UserObjectInternalInternalDTO, StreamUser>.UpdateFromDto(UserObjectInternalInternalDTO dto, ICache cache)
        {
            BanExpires = dto.BanExpires;
            Banned = dto.Banned;
            CreatedAt = dto.CreatedAt;
            DeactivatedAt = dto.DeactivatedAt;
            DeletedAt = dto.DeletedAt;
            Id = dto.Id;
            Invisible = dto.Invisible;
            Language = dto.Language;
            LastActive = dto.LastActive;
            Online = dto.Online;
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications);
            RevokeTokensIssuedBefore = dto.RevokeTokensIssuedBefore;
            Role = dto.Role;
            //ShadowBanned = dto.ShadowBanned; Missing in DTO
            Teams = dto.Teams;
            UpdatedAt = dto.UpdatedAt;

            //Not in API spec
            Name = dto.Name;
            Image = dto.Image;

            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        void IUpdateableFrom<UserResponseInternalDTO, StreamUser>.UpdateFromDto(UserResponseInternalDTO dto, ICache cache)
        {
            BanExpires = dto.BanExpires;
            Banned = dto.Banned;
            CreatedAt = dto.CreatedAt;
            DeactivatedAt = dto.DeactivatedAt;
            DeletedAt = dto.DeletedAt;
            Id = dto.Id;
            Invisible = dto.Invisible;
            Language = dto.Language;
            LastActive = dto.LastActive;
            Online = dto.Online;
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications);
            RevokeTokensIssuedBefore = dto.RevokeTokensIssuedBefore;
            Role = dto.Role;
            ShadowBanned = dto.ShadowBanned;
            Teams = dto.Teams;
            UpdatedAt = dto.UpdatedAt;

            //Not in API spec
            Name = dto.Name;
            Image = dto.Image;

            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        protected override StreamUser Self => this;

        protected override string InternalUniqueId
        {
            get => Id;
            set => Id = value;
        }
    }
}