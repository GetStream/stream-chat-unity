using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.Models;

namespace StreamChat.Core.StatefulModels
{
    internal sealed class StreamLocalUserData : StreamStatefulModelBase<StreamLocalUserData>,
        IUpdateableFrom<OwnUserInternalDTO, StreamLocalUserData>, IUpdateableFrom2<WrappedUnreadCountsResponseInternalDTO, StreamLocalUserData>, IStreamLocalUserData
    {
        #region OwnUser
        
        public IReadOnlyList<StreamChannelMute> ChannelMutes => _channelMutes;

        public IReadOnlyList<StreamDevice> Devices => _devices;

        public IReadOnlyList<string> LatestHiddenChannels => _latestHiddenChannels;
        
        public IReadOnlyList<StreamUserMute> Mutes => _mutes;

        public int? TotalUnreadCount { get; private set; }

        public int? UnreadChannels { get; private set; }

        public int? UnreadThreads { get; private set; }

        #endregion
        
        public IStreamUser User { get; private set; }
        public string UserId => User?.Id;

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
            UnreadThreads = GetOrDefault(dto.UnreadThreads, UnreadThreads);
            //UnreadCount = dto.UnreadCount; Deprecated

            #endregion

            User = cache.Users.CreateOrUpdate3<StreamUser, OwnUserInternalDTO>(dto, out _);

            LoadAdditionalProperties(dto.AdditionalProperties);
            
#if STREAM_DEBUG_ENABLED
            Logs.Info($"Local User Data Loaded. {nameof(TotalUnreadCount)}: {TotalUnreadCount}, UnreadChannels: {UnreadChannels}");
#endif
        }
        
        void IUpdateableFrom2<WrappedUnreadCountsResponseInternalDTO, StreamLocalUserData>.UpdateFromDto(WrappedUnreadCountsResponseInternalDTO dto,
            ICache cache)
        {
            TotalUnreadCount = GetOrDefault(dto.TotalUnreadCount, TotalUnreadCount);
            UnreadChannels = dto.Channels?.Count ?? 0;
            UnreadThreads = GetOrDefault(dto.TotalUnreadThreadsCount, UnreadThreads);
        }
        
        internal StreamLocalUserData(string uniqueId, ICacheRepository<StreamLocalUserData> repository,
            IStatefulModelContext context)
            : base(uniqueId, repository, context)
        {
        }
        
        internal void InternalHandleMarkReadNotification(NotificationMarkReadEventInternalDTO eventDto)
        {
            TotalUnreadCount = GetOrDefault(eventDto.TotalUnreadCount, TotalUnreadCount);
            UnreadChannels = GetOrDefault(eventDto.UnreadChannels, UnreadChannels);
            UnreadThreads = GetOrDefault(eventDto.UnreadThreads, UnreadThreads);
            //UnreadCount = dto.UnreadCount; Deprecated
        }

        internal void InternalHandleMarkUnreadNotification(NotificationMarkUnreadEventInternalDTO eventDto)
        {
            TotalUnreadCount = GetOrDefault(eventDto.TotalUnreadCount, TotalUnreadCount);
            UnreadChannels = GetOrDefault(eventDto.UnreadChannels, UnreadChannels);
            UnreadThreads = GetOrDefault(eventDto.UnreadThreads, UnreadThreads);
        }

        internal void InternalHandleThreadMessageNewNotification(NotificationThreadMessageNewEventInternalDTO eventDto)
        {
            if (eventDto.UnreadThreads.HasValue)
            {
                UnreadThreads = eventDto.UnreadThreads.Value;
            }
        }
        
        protected override string InternalUniqueId { get; set; }
        protected override StreamLocalUserData Self => this;

        private readonly List<StreamChannelMute> _channelMutes = new List<StreamChannelMute>();
        private readonly List<StreamDevice> _devices = new List<StreamDevice>();
        private readonly List<string> _latestHiddenChannels = new List<string>();
        private readonly List<StreamUserMute> _mutes = new List<StreamUserMute>();
    }
}