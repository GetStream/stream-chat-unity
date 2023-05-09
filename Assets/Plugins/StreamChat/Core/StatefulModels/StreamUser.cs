using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.Models;

namespace StreamChat.Core.StatefulModels
{
    public delegate void StreamUserPresenceHandler(IStreamUser user, bool isOnline, DateTimeOffset? lastActive);

    /// <inheritdoc cref="IStreamUser"/>
    internal sealed class StreamUser : StreamStatefulModelBase<StreamUser>,
        IUpdateableFrom<UserObjectInternalDTO, StreamUser>,
        IUpdateableFrom<UserResponseInternalDTO, StreamUser>, IUpdateableFrom<OwnUserInternalDTO, StreamUser>,
        IStreamUser
    {
        public event StreamUserPresenceHandler PresenceChanged;

        public DateTimeOffset? BanExpires { get; private set; }

        public bool Banned { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? DeactivatedAt { get; private set; }

        public DateTimeOffset? DeletedAt { get; private set; }

        public string Id { get; private set; }

        public bool Invisible { get; private set; }

        public string Language { get; private set; }

        public DateTimeOffset? LastActive { get; private set; }

        public bool Online
        {
            get => _online;
            private set
            {
                var prev = _online;
                _online = value;

                if (prev != value)
                {
                    PresenceChanged?.Invoke(this, Online, LastActive);
                }
            }
        }

        public StreamPushNotificationSettings PushNotifications { get; private set; }

        public DateTimeOffset? RevokeTokensIssuedBefore { get; private set; }

        public string Role { get; private set; }

        public bool ShadowBanned { get; private set; }

        public IReadOnlyList<string> Teams => _teams;

        public DateTimeOffset? UpdatedAt { get; private set; }

        //Not in API
        public string Name { get; private set; }
        public string Image { get; private set; }

        public Task FlagAsync() => LowLevelClient.InternalModerationApi.FlagUserAsync(Id);

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

            Client.UpdateLocalUser(response.OwnUser);
        }

        public Task UnmuteAsync()
            => LowLevelClient.InternalModerationApi.UnmuteUserAsync(new UnmuteUserRequestInternalDTO
            {
                TargetId = Id,
            });

        public async Task MarkInvisibleAsync()
        {
            var response = await LowLevelClient.InternalUserApi.UpdateUserPartialAsync(
                new UpdateUserPartialRequestInternalDTO
                {
                    Users = new List<UpdateUserPartialRequestEntryInternalDTO>
                    {
                        new UpdateUserPartialRequestEntryInternalDTO
                        {
                            Id = Id,
                            Set = new Dictionary<string, object>
                            {
                                { "invisible", true }
                            }
                        }
                    }
                });
            //StreamTodo: probably better to fetch by id or throw exception
            Cache.TryCreateOrUpdate(response.Users.First().Value);
        }

        public async Task MarkVisibleAsync()
        {
            var response = await LowLevelClient.InternalUserApi.UpdateUserPartialAsync(
                new UpdateUserPartialRequestInternalDTO
                {
                    Users = new List<UpdateUserPartialRequestEntryInternalDTO>
                    {
                        new UpdateUserPartialRequestEntryInternalDTO
                        {
                            Id = Id,
                            Unset = new List<string>
                            {
                                "invisible"
                            }
                        }
                    }
                });
            //StreamTodo: probably better to fetch by id or throw exception
            Cache.TryCreateOrUpdate(response.Users.First().Value);
        }
        
        public override string ToString() => $"User - Id: {Id}, Name: {Name}";

        void IUpdateableFrom<UserObjectInternalDTO, StreamUser>.UpdateFromDto(UserObjectInternalDTO dto,
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

        internal StreamUser(string uniqueId, ICacheRepository<StreamUser> repository, IStatefulModelContext context)
            : base(uniqueId, repository, context)
        {
        }

        internal void InternalHandlePresenceChanged(UserPresenceChangedEventInternalDTO eventDto)
        {
            Cache.TryCreateOrUpdate(eventDto.User);
        }

        protected override StreamUser Self => this;

        protected override string InternalUniqueId
        {
            get => Id;
            set => Id = value;
        }

        private readonly List<string> _teams = new List<string>();
        private bool _online;
    }
}