using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.Web;

namespace StreamChat.Core.API
{
    internal class ChannelApi : ApiClientBase, IChannelApi
    {
        public ChannelApi(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
            : base(httpClient, serializer, logs, requestUriFactory)
        {
        }

        public Task<ChannelsResponse> QueryChannelsAsync(QueryChannelsRequest queryChannelsRequest)
        {
            var endpoint = ChannelEndpoints.QueryChannels();

            return Post<QueryChannelsRequest, QueryChannelsRequestDTO, ChannelsResponse, ChannelsResponseDTO>(endpoint,
                queryChannelsRequest);
        }

        public Task<ChannelState> GetOrCreateChannelAsync(string channelType,
            ChannelGetOrCreateRequest getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreate(channelType);

            return Post<ChannelGetOrCreateRequest, ChannelGetOrCreateRequestDTO, ChannelState, ChannelStateResponseDTO>(
                endpoint,
                getOrCreateRequest);
        }

        public Task<ChannelState> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequest getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreate(channelType, channelId);

            return Post<ChannelGetOrCreateRequest, ChannelGetOrCreateRequestDTO, ChannelState, ChannelStateResponseDTO>(
                endpoint,
                getOrCreateRequest);
        }

        public Task<UpdateChannelResponse> UpdateChannelAsync(string channelType, string channelId,
            UpdateChannelRequest updateChannelRequest)
        {
            var endpoint = ChannelEndpoints.Update(channelType, channelId);

            return Post<UpdateChannelRequest, UpdateChannelRequestDTO, UpdateChannelResponse, UpdateChannelResponseDTO>(
                endpoint,
                updateChannelRequest);
        }

        public Task<UpdateChannelPartialResponse> UpdateChannelPartialAsync(string channelType, string channelId,
            UpdateChannelPartialRequest updateChannelPartialRequest)
        {
            var endpoint = ChannelEndpoints.UpdatePartial(channelType, channelId);

            return Patch<UpdateChannelPartialRequest, UpdateChannelPartialRequestDTO, UpdateChannelPartialResponse,
                UpdateChannelPartialResponseDTO>(endpoint,
                updateChannelPartialRequest);
        }

        public Task<DeleteChannelsResponse> DeleteChannelsAsync(DeleteChannelsRequest deleteChannelsRequest)
        {
            var endpoint = ChannelEndpoints.DeleteChannels();

            return Post<DeleteChannelsRequest, DeleteChannelsRequestDTO, DeleteChannelsResponse,
                DeleteChannelsResponseDTO>(endpoint,
                deleteChannelsRequest);
        }

        public Task<DeleteChannelResponse> DeleteChannelAsync(string channelType, string channelId)
        {
            var endpoint = ChannelEndpoints.DeleteChannel(channelType, channelId);

            return Delete<DeleteChannelResponse, DeleteChannelResponseDTO>(endpoint);
        }

        public Task<TruncateChannelResponse> TruncateChannelAsync(string channelType, string channelId,
            TruncateChannelRequest truncateChannelRequest)
        {
            var endpoint = ChannelEndpoints.TruncateChannel(channelType, channelId);

            return Post<TruncateChannelRequest, TruncateChannelRequestDTO, TruncateChannelResponse,
                TruncateChannelResponseDTO>(endpoint,
                truncateChannelRequest);
        }

        public Task<MuteChannelResponse> MuteChannelAsync(MuteChannelRequest muteChannelRequest)
        {
            var endpoint = ChannelEndpoints.MuteChannel();

            return Post<MuteChannelRequest, MuteChannelRequestDTO, MuteChannelResponse, MuteChannelResponseDTO>(
                endpoint,
                muteChannelRequest);
        }

        public Task<UnmuteResponse> UnmuteChannelAsync(UnmuteChannelRequest unmuteChannelRequest)
        {
            var endpoint = ChannelEndpoints.UnmuteChannel();

            return Post<UnmuteChannelRequest, UnmuteChannelRequestDTO, UnmuteResponse, UnmuteResponseDTO>(
                endpoint,
                unmuteChannelRequest);
        }

        public Task<ShowChannelResponse> ShowChannelAsync(string channelType, string channelId,
            ShowChannelRequest showChannelRequest)
        {
            var endpoint = ChannelEndpoints.ShowChannel(channelType, channelId);

            return Post<ShowChannelRequest, ShowChannelRequestDTO, ShowChannelResponse, ShowChannelResponseDTO>(
                endpoint,
                showChannelRequest);
        }

        public Task<HideChannelResponse> HideChannelAsync(string channelType, string channelId,
            HideChannelRequest hideChannelRequest)
        {
            var endpoint = ChannelEndpoints.HideChannel(channelType, channelId);

            return Post<HideChannelRequest, HideChannelRequestDTO, HideChannelResponse, HideChannelResponseDTO>(
                endpoint,
                hideChannelRequest);
        }

        public Task<MembersResponse> QueryMembersAsync(QueryMembersRequest queryMembersRequest) =>
            Get<QueryMembersRequest, QueryMembersRequestDTO, MembersResponse, MembersResponseDTO>(
                "/members",
                queryMembersRequest);

        public Task<StopWatchingResponse> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequest channelStopWatchingRequest) =>
            Post<ChannelStopWatchingRequest, ChannelStopWatchingRequestDTO, StopWatchingResponse,
                StopWatchingResponseDTO>($"/channels/{channelType}/{channelId}/stop-watching",
                channelStopWatchingRequest);
    }
}