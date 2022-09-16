using System.Threading.Tasks;
using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.API.Internal
{
    internal class InternalChannelApi : InternalApiClientBase, IInternalChannelApi
    {
        internal InternalChannelApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<ChannelsResponseDTO> QueryChannelsAsync(QueryChannelsRequestDTO queryChannelsRequest)
        {
            var endpoint = ChannelEndpoints.QueryChannels();
            return Post<QueryChannelsRequestDTO, ChannelsResponseDTO>(endpoint, queryChannelsRequest);
        }

        public Task<ChannelStateResponseDTO> GetOrCreateChannelAsync(string channelType,
            ChannelGetOrCreateRequestDTO getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreate(channelType);
            return Post<ChannelGetOrCreateRequestDTO, ChannelStateResponseDTO>(endpoint, getOrCreateRequest);
        }

        public Task<ChannelStateResponseDTO> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequestDTO getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreate(channelType, channelId);
            return Post<ChannelGetOrCreateRequestDTO, ChannelStateResponseDTO>(endpoint, getOrCreateRequest);
        }

        public Task<UpdateChannelResponseDTO> UpdateChannelAsync(string channelType, string channelId,
            UpdateChannelRequestDTO updateChannelRequest)
        {
            var endpoint = ChannelEndpoints.Update(channelType, channelId);
            return Post<UpdateChannelRequestDTO, UpdateChannelResponseDTO>(endpoint, updateChannelRequest);
        }

        public Task<UpdateChannelPartialResponseDTO> UpdateChannelPartialAsync(string channelType, string channelId,
            UpdateChannelPartialRequestDTO updateChannelPartialRequest)
        {
            var endpoint = ChannelEndpoints.UpdatePartial(channelType, channelId);
            return Patch<UpdateChannelPartialRequestDTO, UpdateChannelPartialResponseDTO>(endpoint,
                updateChannelPartialRequest);
        }

        public Task<DeleteChannelsResponseDTO> DeleteChannelsAsync(DeleteChannelsRequestDTO deleteChannelsRequest)
        {
            var endpoint = ChannelEndpoints.DeleteChannels();
            return Post<DeleteChannelsRequestDTO, DeleteChannelsResponseDTO>(endpoint, deleteChannelsRequest);
        }

        public Task<DeleteChannelResponseDTO> DeleteChannelAsync(string channelType, string channelId)
        {
            var endpoint = ChannelEndpoints.DeleteChannel(channelType, channelId);
            return Delete<DeleteChannelResponseDTO>(endpoint);
        }

        public Task<TruncateChannelResponseDTO> TruncateChannelAsync(string channelType, string channelId,
            TruncateChannelRequestDTO truncateChannelRequest)
        {
            var endpoint = ChannelEndpoints.TruncateChannel(channelType, channelId);
            return Post<TruncateChannelRequestDTO, TruncateChannelResponseDTO>(endpoint, truncateChannelRequest);
        }

        public Task<MuteChannelResponseDTO> MuteChannelAsync(MuteChannelRequestDTO muteChannelRequest)
        {
            var endpoint = ChannelEndpoints.MuteChannel();
            return Post<MuteChannelRequestDTO, MuteChannelResponseDTO>(endpoint, muteChannelRequest);
        }

        public Task<UnmuteResponseDTO> UnmuteChannelAsync(UnmuteChannelRequestDTO unmuteChannelRequest)
        {
            var endpoint = ChannelEndpoints.UnmuteChannel();
            return Post<UnmuteChannelRequestDTO, UnmuteResponseDTO>(endpoint, unmuteChannelRequest);
        }

        public Task<ShowChannelResponseDTO> ShowChannelAsync(string channelType, string channelId,
            ShowChannelRequestDTO showChannelRequest)
        {
            var endpoint = ChannelEndpoints.ShowChannel(channelType, channelId);
            return Post<ShowChannelRequestDTO, ShowChannelResponseDTO>(endpoint, showChannelRequest);
        }

        public Task<HideChannelResponseDTO> HideChannelAsync(string channelType, string channelId,
            HideChannelRequestDTO hideChannelRequest)
        {
            var endpoint = ChannelEndpoints.HideChannel(channelType, channelId);
            return Post<HideChannelRequestDTO, HideChannelResponseDTO>(endpoint, hideChannelRequest);
        }

        public Task<MembersResponseDTO> QueryMembersAsync(QueryMembersRequestDTO queryMembersRequest)
            => Get<QueryMembersRequestDTO, MembersResponseDTO>("/members", queryMembersRequest);

        public Task<StopWatchingResponseDTO> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequestDTO channelStopWatchingRequest)
            => Post<ChannelStopWatchingRequestDTO, StopWatchingResponseDTO>(
                $"/channels/{channelType}/{channelId}/stop-watching", channelStopWatchingRequest);

        public Task<MarkReadResponseDTO> MarkReadAsync(string channelType, string channelId,
            MarkReadRequestDTO markReadRequest)
            => Post<MarkReadRequestDTO, MarkReadResponseDTO>($"/channels/{channelType}/{channelId}/read",
                markReadRequest);

        public Task<MarkReadResponseDTO> MarkManyReadAsync(MarkChannelsReadRequestDTO markChannelsReadRequest)
            => Post<MarkChannelsReadRequestDTO, MarkReadResponseDTO>($"/channels/read", markChannelsReadRequest);

        public Task SendTypingStartEventAsync(string channelType, string channelId)
            => PostEventAsync(channelType, channelId, new EventTypingStartDTO());

        public Task SendTypingStopEventAsync(string channelType, string channelId)
            => PostEventAsync(channelType, channelId, new EventTypingStopDTO());
    }
}