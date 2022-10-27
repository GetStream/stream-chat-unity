using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.Models;

namespace StreamChat.Core.State.TrackedObjects
{
    /// <summary>
    /// Stream user represents a single chat user that can be a member of multiple channels
    ///
    /// This object is tracked by <see cref="StreamChatStateClient"/> meaning its state will be automatically updated
    /// </summary>
    public class StreamUser : StreamTrackedObjectBase<StreamUser>, IUpdateableFrom<UserObjectInternalInternalDTO, StreamUser>
        , IUpdateableFrom<UserResponseInternalDTO, StreamUser>, IUpdateableFrom<OwnUserInternalDTO, StreamUser>
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public System.DateTimeOffset? BanExpires { get; private set; }

        /// <summary>
        /// Whether a user is banned or not
        /// </summary>
        public bool? Banned { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Date of deactivation
        /// </summary>
        public System.DateTimeOffset? DeactivatedAt { get; private set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        public System.DateTimeOffset? DeletedAt { get; private set; }

        /// <summary>
        /// Unique user identifier
        /// </summary>
        public string Id { get; private set; }

        public bool? Invisible { get; private set; }

        /// <summary>
        /// Preferred language of a user
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// Date of last activity
        /// </summary>
        public System.DateTimeOffset? LastActive { get; private set; }

        /// <summary>
        /// Whether a user online or not
        /// </summary>
        public bool? Online { get; private set; }

        public StreamPushNotificationSettings PushNotifications { get; private set; }

        /// <summary>
        /// Revocation date for tokens
        /// </summary>
        public System.DateTimeOffset? RevokeTokensIssuedBefore { get; private set; }

        /// <summary>
        /// Determines the set of user permissions
        /// </summary>
        public string Role { get; private set; }

        /// <summary>
        /// Whether user is shadow banned or not
        /// </summary>
        public bool? ShadowBanned { get; private set; }

        /// <summary>
        /// List of teams user is a part of
        /// </summary>
        public System.Collections.Generic.List<string> Teams { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; private set; }

        //Not in API
        public string Name;
        public string Image;

        /// <summary>
        /// Flag this user
        /// </summary>
        public Task FlagAsync() => LowLevelClient.InternalModerationApi.FlagUserAsync(Id);

        /// <summary>
        /// Mark user as muted. Any user is allowed to mute another user. Mute will last until the <see cref="UnmuteAsync"/> is called or until mute expires.
        /// Muted user messages will still be received by the <see cref="IStreamChatStateClient"/> so if you wish to hide muted users messages you need implement by yourself
        ///
        /// You can access mutes via <see cref="StreamLocalUser.Mutes"/> in <see cref="IStreamChatStateClient.LocalUserData"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#mutes</remarks>
        public async Task MuteAsync()
        {
            var response = await LowLevelClient.InternalModerationApi.MuteUserAsync(new MuteUserRequestInternalDTO
            {
                TargetIds = new List<string>
                {
                    Id
                },
                Timeout = null,
            });

            StreamChatStateClient.UpdateLocalUser(response.OwnUser);
        }

        /// <summary>
        /// Remove user mute. Any user is allowed to mute another user. Mute will last until the <see cref="UnmuteAsync"/> is called or until mute expires.
        /// Muted user messages will still be received by the <see cref="IStreamChatStateClient"/> so if you wish to hide muted users messages you need implement by yourself
        ///
        /// You can access mutes via <see cref="StreamLocalUser.Mutes"/> in <see cref="IStreamChatStateClient.LocalUserData"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#mutes</remarks>
        public Task UnmuteAsync() => LowLevelClient.InternalModerationApi.UnmuteUserAsync(new UnmuteUserRequestInternalDTO
        {
            TargetId = Id,
        });

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
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications, cache);
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
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications, cache);
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

        void IUpdateableFrom<OwnUserInternalDTO, StreamUser>.UpdateFromDto(OwnUserInternalDTO dto, ICache cache)
        {
            #region OwnUser

            //_channelMutes.TryReplaceRegularObjectsFromDto(dto.ChannelMutes, cache);
            //_devices.TryReplaceRegularObjectsFromDto(dto.Devices, cache);
            //_latestHiddenChannels.TryReplaceValuesFromDto(dto.LatestHiddenChannels);
            //_mutes.TryReplaceRegularObjectsFromDto(dto.Mutes, cache);

            //TotalUnreadCount = dto.TotalUnreadCount;
            //UnreadChannels = dto.UnreadChannels;
            //UnreadCount = dto.UnreadCount; Deprecated

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
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications, cache);
            //RevokeTokensIssuedBefore = dto.RevokeTokensIssuedBefore; //Not present in this DTO
            Role = dto.Role;
            _teams.TryReplaceValuesFromDto(dto.Teams);
            UpdatedAt = dto.UpdatedAt;

            //Not in API spec
            //Name = dto.Name;
            //Image = dto.Image;

            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        protected override StreamUser Self => this;

        protected override string InternalUniqueId
        {
            get => Id;
            set => Id = value;
        }

        private readonly List<string> _teams = new List<string>();
    }
}