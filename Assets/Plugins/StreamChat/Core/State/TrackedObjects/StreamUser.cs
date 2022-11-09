using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State.TrackedObjects
{
    public interface IStreamUser
    {
    }

    /// <summary>
    /// Stream user represents a single chat user that can be a member of multiple channels
    ///
    /// This object is tracked by <see cref="StreamChatStateClient"/> meaning its state will be automatically updated
    /// </summary>
    public sealed class StreamUser : StreamTrackedObjectBase<StreamUser>,
        IUpdateableFrom<UserObjectInternalInternalDTO, StreamUser>,
        IUpdateableFrom<UserResponseInternalDTO, StreamUser>, IUpdateableFrom<OwnUserInternalDTO, StreamUser>
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public System.DateTimeOffset? BanExpires { get; private set; }

        /// <summary>
        /// Whether a user is banned or not
        /// </summary>
        public bool Banned { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset CreatedAt { get; private set; }

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

        /// <summary>
        /// Invisible user will appear as offline to other users
        ///
        /// You can change user visibility with <see cref="ChangeVisibilityAsync"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/presence_format/?language=unity</remarks>
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
        public IReadOnlyList<string> Teams => _teams;

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; private set; }

        //Not in API
        public string Name { get; private set; }
        public string Image { get; private set; }

        /// <summary>
        /// Flag this user
        /// </summary>
        public Task FlagAsync() => LowLevelClient.InternalModerationApi.FlagUserAsync(Id);

        /// <summary>
        /// Mark user as muted. Any user is allowed to mute another user. Mute will last until the <see cref="UnmuteAsync"/> is called or until mute expires.
        /// Muted user messages will still be received by the <see cref="IStreamChatStateClient"/> so if you wish to hide muted users messages you need implement by yourself
        ///
        /// You can access mutes via <see crefStreamLocalUserDatata.Mutes"/> in <see cref="IStreamChatStateClient.LocalUserData"/>
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
        /// Mark user as invisible. Invisible user will appear as offline to other users.
        /// User will remain invisible even if you disconnect and reconnect again. You must explicitly call <see cref="MarkVisible"/> in order to become visible again.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/presence_format/?language=unity#invisible</remarks>
        public async Task MarkInvisible()
        {
            var response = await LowLevelClient.InternalUserApi.UpdateUserPartialAsync(
                new UpdateUserPartialRequestInternalDTO
                {
                    Set = new Dictionary<string, object>
                    {
                        {"invisible", true}
                    }
                });
            Cache.TryCreateOrUpdate(response.Users.First().Value);
        }

        /// <summary>
        /// Mark user visible again if he was previously marked as invisible with <see cref="MarkInvisible"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/presence_format/?language=unity#invisible</remarks>
        public async Task MarkVisible()
        {
            var response = await LowLevelClient.InternalUserApi.UpdateUserPartialAsync(
                new UpdateUserPartialRequestInternalDTO
                {
                    Unset = new List<string>
                    {
                        "invisible"
                    }
                });
            Cache.TryCreateOrUpdate(response.Users.First().Value);
        }

        /// <summary>
        /// Remove user mute. Any user is allowed to mute another user. Mute will last until the <see cref="UnmuteAsync"/> is called or until mute expires.
        /// Muted user messages will still be received by the <see cref="IStreamChatStateClient"/> so if you wish to hide muted users messages you need implement by yourself
        ///
        /// You can access mutes via <see crefStreamLocalUserDatata.Mutes"/> in <see cref="IStreamChatStateClient.LocalUserData"/>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/moderation/?language=unity#mutes</remarks>
        public Task UnmuteAsync()
            => LowLevelClient.InternalModerationApi.UnmuteUserAsync(new UnmuteUserRequestInternalDTO
            {
                TargetId = Id,
            });

        void IUpdateableFrom<UserObjectInternalInternalDTO, StreamUser>.UpdateFromDto(UserObjectInternalInternalDTO dto,
            ICache cache)
        {
            BanExpires = GetOrDefault(dto.BanExpires, BanExpires);
            Banned = GetOrDefault(dto.Banned, Banned);
            CreatedAt = GetOrDefault(dto.CreatedAt, CreatedAt);
            DeactivatedAt = GetOrDefault(dto.DeactivatedAt, DeactivatedAt);
            DeletedAt = GetOrDefault(dto.DeletedAt, DeletedAt);
            Id = GetOrDefault(dto.Id, Id);
            Invisible = GetOrDefault(dto.Invisible, Invisible);
            Language = GetOrDefault(dto.Language, Language);
            LastActive = GetOrDefault(dto.LastActive, LastActive);
            Online = GetOrDefault(dto.Online, Online);
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications, cache);
            RevokeTokensIssuedBefore = GetOrDefault(dto.RevokeTokensIssuedBefore, RevokeTokensIssuedBefore);
            Role = GetOrDefault(dto.Role, Role);
            //ShadowBanned = dto.ShadowBanned; Missing in DTO
            _teams.TryReplaceValuesFromDto(dto.Teams);
            UpdatedAt = GetOrDefault(dto.UpdatedAt, UpdatedAt);

            //Not in API spec
            Name = GetOrDefault(dto.Name, Name);
            Image = GetOrDefault(dto.Image, Image);

            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        void IUpdateableFrom<UserResponseInternalDTO, StreamUser>.UpdateFromDto(UserResponseInternalDTO dto,
            ICache cache)
        {
            BanExpires = GetOrDefault(dto.BanExpires, BanExpires);
            Banned = GetOrDefault(dto.Banned, Banned);
            CreatedAt = GetOrDefault(dto.CreatedAt, CreatedAt);
            DeactivatedAt = GetOrDefault(dto.DeactivatedAt, DeactivatedAt);
            DeletedAt = GetOrDefault(dto.DeletedAt, DeletedAt);
            Id = GetOrDefault(dto.Id, Id);
            Invisible = GetOrDefault(dto.Invisible, Invisible);
            Language = GetOrDefault(dto.Language, Language);
            LastActive = GetOrDefault(dto.LastActive, LastActive);
            Online = GetOrDefault(dto.Online, Online);
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications, cache);
            RevokeTokensIssuedBefore = GetOrDefault(dto.RevokeTokensIssuedBefore, RevokeTokensIssuedBefore);
            Role = GetOrDefault(dto.Role, Role);
            ShadowBanned = GetOrDefault(dto.ShadowBanned, ShadowBanned);
            _teams.TryReplaceValuesFromDto(dto.Teams);
            UpdatedAt = GetOrDefault(dto.UpdatedAt, UpdatedAt);

            //Not in API spec
            Name = GetOrDefault(dto.Name, Name);
            Image = GetOrDefault(dto.Image, Image);

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

            //BanExpires = GetOrDefault(dto.BanExpires, BanExpires);
            Banned = GetOrDefault(dto.Banned, Banned);
            CreatedAt = GetOrDefault(dto.CreatedAt, CreatedAt);
            DeactivatedAt = GetOrDefault(dto.DeactivatedAt, DeactivatedAt);
            DeletedAt = GetOrDefault(dto.DeletedAt, DeletedAt);
            Id = GetOrDefault(dto.Id, Id);
            Invisible = GetOrDefault(dto.Invisible, Invisible);
            Language = GetOrDefault(dto.Language, Language);
            LastActive = GetOrDefault(dto.LastActive, LastActive);
            Online = GetOrDefault(dto.Online, Online);
            PushNotifications = PushNotifications.TryLoadFromDto(dto.PushNotifications, cache);
            //RevokeTokensIssuedBefore = dto.RevokeTokensIssuedBefore; //Not present in this DTO
            Role = GetOrDefault(dto.Role, Role);
            _teams.TryReplaceValuesFromDto(dto.Teams);
            UpdatedAt = GetOrDefault(dto.UpdatedAt, UpdatedAt);

            //Not in API spec //StreamTodo: Add to DTO?
            //Name = GetOrDefault(dto.Name, Name);
            //Image = GetOrDefault(dto.Image, Image);

            LoadAdditionalProperties(dto.AdditionalProperties);
        }

        internal StreamUser(string uniqueId, ICacheRepository<StreamUser> repository, ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        internal void InternalHandlePresenceChanged(EventUserPresenceChangedInternalDTO eventDto)
        {
            var prevOnline = Online;
            var prevLastActive = LastActive;
            Cache.TryCreateOrUpdate(eventDto.User);
            //StreamTodo: verify with test how presence notifications work
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