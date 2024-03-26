using System;
using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal class InternalChannelApi : InternalApiClientBase, IInternalChannelApi
    {
        internal InternalChannelApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory, IStreamChatLowLevelClient lowLevelClient)
            : base(httpClient, serializer, logs, requestUriFactory, lowLevelClient)
        {
        }

        public Task<ChannelsResponseInternalDTO> QueryChannelsAsync(QueryChannelsRequestInternalDTO queryChannelsRequest)
        {
            var endpoint = ChannelEndpoints.QueryChannels();
            return Post<QueryChannelsRequestInternalDTO, ChannelsResponseInternalDTO>(endpoint, queryChannelsRequest);
        }

        public Task<ChannelStateResponseInternalDTO> GetOrCreateChannelAsync(string channelType,
            ChannelGetOrCreateRequestInternalDTO getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreate(channelType);
            return Post<ChannelGetOrCreateRequestInternalDTO, ChannelStateResponseInternalDTO>(endpoint, getOrCreateRequest);
        }

        public Task<ChannelStateResponseInternalDTO> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequestInternalDTO getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreate(channelType, channelId);
            return Post<ChannelGetOrCreateRequestInternalDTO, ChannelStateResponseInternalDTO>(endpoint, getOrCreateRequest);
        }

        public Task<UpdateChannelResponseInternalDTO> UpdateChannelAsync(string channelType, string channelId,
            UpdateChannelRequestInternalDTO updateChannelRequest)
        {
            var endpoint = ChannelEndpoints.Update(channelType, channelId);
            return Post<UpdateChannelRequestInternalDTO, UpdateChannelResponseInternalDTO>(endpoint, updateChannelRequest);
        }

        public Task<UpdateChannelPartialResponseInternalDTO> UpdateChannelPartialAsync(string channelType, string channelId,
            UpdateChannelPartialRequestInternalDTO updateChannelPartialRequest)
        {
            var endpoint = ChannelEndpoints.UpdatePartial(channelType, channelId);
            return Patch<UpdateChannelPartialRequestInternalDTO, UpdateChannelPartialResponseInternalDTO>(endpoint,
                updateChannelPartialRequest);
        }

        public Task<DeleteChannelsResponseInternalDTO> DeleteChannelsAsync(DeleteChannelsRequestInternalDTO deleteChannelsRequest)
        {
            var endpoint = ChannelEndpoints.DeleteChannels();
            return Post<DeleteChannelsRequestInternalDTO, DeleteChannelsResponseInternalDTO>(endpoint, deleteChannelsRequest);
        }

        public Task<DeleteChannelResponseInternalDTO> DeleteChannelAsync(string channelType, string channelId, bool isHardDelete = false)
        {
            var endpoint = ChannelEndpoints.DeleteChannel(channelType, channelId);
            var parameters = QueryParameters.Default.Append("hard_delete", isHardDelete);
            return Delete<DeleteChannelResponseInternalDTO>(endpoint, parameters);
        }

        [Obsolete("Please use the other overload. This method is deprecated and will be removed in a future release")]
        public Task<DeleteChannelResponseInternalDTO> DeleteChannelAsync(string channelType, string channelId)
        {
            var endpoint = ChannelEndpoints.DeleteChannel(channelType, channelId);
            return Delete<DeleteChannelResponseInternalDTO>(endpoint);
        }

        public Task<TruncateChannelResponseInternalDTO> TruncateChannelAsync(string channelType, string channelId,
            TruncateChannelRequestInternalDTO truncateChannelRequest)
        {
            var endpoint = ChannelEndpoints.TruncateChannel(channelType, channelId);
            return Post<TruncateChannelRequestInternalDTO, TruncateChannelResponseInternalDTO>(endpoint, truncateChannelRequest);
        }

        public Task<MuteChannelResponseInternalDTO> MuteChannelAsync(MuteChannelRequestInternalDTO muteChannelRequest)
        {
            var endpoint = ChannelEndpoints.MuteChannel();
            return Post<MuteChannelRequestInternalDTO, MuteChannelResponseInternalDTO>(endpoint, muteChannelRequest);
        }

        public Task<UnmuteResponseInternalDTO> UnmuteChannelAsync(UnmuteChannelRequestInternalDTO unmuteChannelRequest)
        {
            var endpoint = ChannelEndpoints.UnmuteChannel();
            return Post<UnmuteChannelRequestInternalDTO, UnmuteResponseInternalDTO>(endpoint, unmuteChannelRequest);
        }

        public Task<ShowChannelResponseInternalDTO> ShowChannelAsync(string channelType, string channelId,
            ShowChannelRequestInternalDTO showChannelRequest)
        {
            var endpoint = ChannelEndpoints.ShowChannel(channelType, channelId);
            return Post<ShowChannelRequestInternalDTO, ShowChannelResponseInternalDTO>(endpoint, showChannelRequest);
        }

        public Task<HideChannelResponseInternalDTO> HideChannelAsync(string channelType, string channelId,
            HideChannelRequestInternalDTO hideChannelRequest)
        {
            var endpoint = ChannelEndpoints.HideChannel(channelType, channelId);
            return Post<HideChannelRequestInternalDTO, HideChannelResponseInternalDTO>(endpoint, hideChannelRequest);
        }

        public Task<MembersResponseInternalDTO> QueryMembersAsync(QueryMembersRequestInternalDTO queryMembersRequest)
            => Get<QueryMembersRequestInternalDTO, MembersResponseInternalDTO>("/members", queryMembersRequest);

        public Task<StopWatchingResponseInternalDTO> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequestInternalDTO channelStopWatchingRequest)
            => Post<ChannelStopWatchingRequestInternalDTO, StopWatchingResponseInternalDTO>(
                $"/channels/{channelType}/{channelId}/stop-watching", channelStopWatchingRequest);

        public Task<MarkReadResponseInternalDTO> MarkReadAsync(string channelType, string channelId,
            MarkReadRequestInternalDTO markReadRequest)
            => Post<MarkReadRequestInternalDTO, MarkReadResponseInternalDTO>($"/channels/{channelType}/{channelId}/read",
                markReadRequest);

        public Task<MarkReadResponseInternalDTO> MarkManyReadAsync(MarkChannelsReadRequestInternalDTO markChannelsReadRequest)
            => Post<MarkChannelsReadRequestInternalDTO, MarkReadResponseInternalDTO>($"/channels/read", markChannelsReadRequest);

        public Task SendTypingStartEventAsync(string channelType, string channelId)
            => PostEventAsync(channelType, channelId, new TypingStartEventInternalDTO
            {
                Type = WSEventType.TypingStart
            });

        public Task SendTypingStopEventAsync(string channelType, string channelId)
            => PostEventAsync(channelType, channelId, new TypingStopEventInternalDTO
            {
                Type = WSEventType.TypingStop
            });
    }
}