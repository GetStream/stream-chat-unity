using System;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    internal class ChannelApi : IChannelApi
    {
        internal ChannelApi(IInternalChannelApi internalChannelApi)
        {
            _internalChannelApi = internalChannelApi ?? throw new ArgumentNullException(nameof(internalChannelApi));
        }

        public async Task<ChannelsResponse> QueryChannelsAsync(QueryChannelsRequest queryChannelsRequest)
        {
            var dto = await _internalChannelApi.QueryChannelsAsync(queryChannelsRequest.TrySaveToDto());
            return dto.ToDomain<ChannelsResponseInternalDTO, ChannelsResponse>();
        }

        public async Task<ChannelState> GetOrCreateChannelAsync(string channelType,
            ChannelGetOrCreateRequest getOrCreateRequest)
        {
            var dto = await _internalChannelApi.GetOrCreateChannelAsync(channelType, getOrCreateRequest.TrySaveToDto());
            return dto.ToDomain<ChannelStateResponseInternalDTO, ChannelState>();
        }

        public async Task<ChannelState> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequest getOrCreateRequest)
        {
            var dto = await _internalChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                getOrCreateRequest.TrySaveToDto());
            return dto.ToDomain<ChannelStateResponseInternalDTO, ChannelState>();
        }

        public async Task<UpdateChannelResponse> UpdateChannelAsync(string channelType, string channelId,
            UpdateChannelRequest updateChannelRequest)
        {
            var dto = await _internalChannelApi.UpdateChannelAsync(channelType, channelId,
                updateChannelRequest.TrySaveToDto());
            return dto.ToDomain<UpdateChannelResponseInternalDTO, UpdateChannelResponse>();
        }

        public async Task<UpdateChannelPartialResponse> UpdateChannelPartialAsync(string channelType, string channelId,
            UpdateChannelPartialRequest updateChannelPartialRequest)
        {
            var dto = await _internalChannelApi.UpdateChannelPartialAsync(channelType, channelId,
                updateChannelPartialRequest.TrySaveToDto());
            return dto.ToDomain<UpdateChannelPartialResponseInternalDTO, UpdateChannelPartialResponse>();
        }

        public async Task<DeleteChannelsResponse> DeleteChannelsAsync(DeleteChannelsRequest deleteChannelsRequest)
        {
            var dto = await _internalChannelApi.DeleteChannelsAsync(deleteChannelsRequest.TrySaveToDto());
            return dto.ToDomain<DeleteChannelsResponseInternalDTO, DeleteChannelsResponse>();
        }

        public async Task<DeleteChannelResponse> DeleteChannelAsync(string channelType, string channelId, bool isHardDelete)
        {
            var dto = await _internalChannelApi.DeleteChannelAsync(channelType, channelId, isHardDelete);
            return dto.ToDomain<DeleteChannelResponseInternalDTO, DeleteChannelResponse>();
        }

        [Obsolete("Please use the other overload. This method is deprecated and will be removed in a future release")]
        public async Task<DeleteChannelResponse> DeleteChannelAsync(string channelType, string channelId)
        {
            var dto = await _internalChannelApi.DeleteChannelAsync(channelType, channelId);
            return dto.ToDomain<DeleteChannelResponseInternalDTO, DeleteChannelResponse>();
        }

        public async Task<TruncateChannelResponse> TruncateChannelAsync(string channelType, string channelId,
            TruncateChannelRequest truncateChannelRequest)
        {
            var dto = await _internalChannelApi.TruncateChannelAsync(channelType, channelId,
                truncateChannelRequest.TrySaveToDto());
            return dto.ToDomain<TruncateChannelResponseInternalDTO, TruncateChannelResponse>();
        }

        public async Task<MuteChannelResponse> MuteChannelAsync(MuteChannelRequest muteChannelRequest)
        {
            var dto = await _internalChannelApi.MuteChannelAsync(muteChannelRequest.TrySaveToDto());
            return dto.ToDomain<MuteChannelResponseInternalDTO, MuteChannelResponse>();
        }

        public async Task<UnmuteResponse> UnmuteChannelAsync(UnmuteChannelRequest unmuteChannelRequest)
        {
            var dto = await _internalChannelApi.UnmuteChannelAsync(unmuteChannelRequest.TrySaveToDto());
            return dto.ToDomain<UnmuteResponseInternalDTO, UnmuteResponse>();
        }

        public async Task<ShowChannelResponse> ShowChannelAsync(string channelType, string channelId,
            ShowChannelRequest showChannelRequest)
        {
            var dto = await _internalChannelApi.ShowChannelAsync(channelType, channelId,
                showChannelRequest.TrySaveToDto());
            return dto.ToDomain<ShowChannelResponseInternalDTO, ShowChannelResponse>();
        }

        public async Task<HideChannelResponse> HideChannelAsync(string channelType, string channelId,
            HideChannelRequest hideChannelRequest)
        {
            var dto = await _internalChannelApi.HideChannelAsync(channelType, channelId,
                hideChannelRequest.TrySaveToDto());
            return dto.ToDomain<HideChannelResponseInternalDTO, HideChannelResponse>();
        }

        public async Task<MembersResponse> QueryMembersAsync(QueryMembersRequest queryMembersRequest)
        {
            var dto = await _internalChannelApi.QueryMembersAsync(queryMembersRequest.TrySaveToDto());
            return dto.ToDomain<MembersResponseInternalDTO, MembersResponse>();
        }

        public async Task<StopWatchingResponse> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequest channelStopWatchingRequest)
        {
            var dto = await _internalChannelApi.StopWatchingChannelAsync(channelType, channelId,
                channelStopWatchingRequest.TrySaveToDto());
            return dto.ToDomain<StopWatchingResponseInternalDTO, StopWatchingResponse>();
        }

        public async Task<MarkReadResponse> MarkReadAsync(string channelType, string channelId,
            MarkReadRequest markReadRequest)
        {
            var dto = await _internalChannelApi.MarkReadAsync(channelType, channelId, markReadRequest.TrySaveToDto());
            return dto.ToDomain<MarkReadResponseInternalDTO, MarkReadResponse>();
        }

        public async Task<MarkReadResponse> MarkManyReadAsync(MarkChannelsReadRequest markChannelsReadRequest)
        {
            var dto = await _internalChannelApi.MarkManyReadAsync(markChannelsReadRequest.TrySaveToDto());
            return dto.ToDomain<MarkReadResponseInternalDTO, MarkReadResponse>();
        }

        public Task SendTypingStartEventAsync(string channelType, string channelId)
            => _internalChannelApi.SendTypingStartEventAsync(channelType, channelId);

        public Task SendTypingStopEventAsync(string channelType, string channelId)
            => _internalChannelApi.SendTypingStopEventAsync(channelType, channelId);

        private readonly IInternalChannelApi _internalChannelApi;
    }
}