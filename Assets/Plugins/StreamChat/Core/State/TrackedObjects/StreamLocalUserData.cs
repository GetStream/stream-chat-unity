using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State.Models;
using StreamChat.Core.State.Caches;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State.TrackedObjects
{
    internal sealed class StreamLocalUserData : StreamTrackedObjectBase<StreamLocalUserData>,
        IUpdateableFrom<OwnUserInternalDTO, StreamLocalUserData>, IStreamLocalUserData
    {
        #region OwnUser
        
        public IReadOnlyList<StreamChannelMute> ChannelMutes => _channelMutes;

        public IReadOnlyList<StreamDevice> Devices => _devices;

        public IReadOnlyList<string> LatestHiddenChannels => _latestHiddenChannels;
        
        public IReadOnlyList<StreamUserMute> Mutes => _mutes;

        public int? TotalUnreadCount { get; private set; }

        public int? UnreadChannels { get; private set; }
        
        #endregion
        
        public IStreamUser User { get; private set; }

        internal StreamLocalUserData(string uniqueId, ICacheRepository<StreamLocalUserData> repository,
            ITrackedObjectContext context)
            : base(uniqueId, repository, context)
        {
        }

        protected override string InternalUniqueId { get; set; }
        protected override StreamLocalUserData Self => this;

        void IUpdateableFrom<OwnUserInternalDTO, StreamLocalUserData>.UpdateFromDto(OwnUserInternalDTO dto,
            ICache cache)
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