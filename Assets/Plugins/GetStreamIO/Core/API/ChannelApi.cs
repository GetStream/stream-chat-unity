using System.Threading.Tasks;
using GetStreamIO.Core.DTO.Requests;
using GetStreamIO.Core.DTO.Responses;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Requests;
using Plugins.GetStreamIO.Core.Responses;
using Plugins.GetStreamIO.Core.Web;
using Plugins.GetStreamIO.Libs.Http;
using Plugins.GetStreamIO.Libs.Logs;
using Plugins.GetStreamIO.Libs.Serialization;

namespace Plugins.GetStreamIO.Core.API
{
    public class ChannelApi : ApiClientBase, IChannelApi
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

        public Task<ChannelState> GetOrCreateChannelAsync(string channelType, ChannelGetOrCreateRequest getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreateAsync(channelType);

            return Post<ChannelGetOrCreateRequest, ChannelGetOrCreateRequestDTO, ChannelState, ChannelStateResponseDTO>(endpoint,
                getOrCreateRequest);
        }

        public Task<ChannelState> GetOrCreateChannelAsync(string channelType, string channelId, ChannelGetOrCreateRequest getOrCreateRequest)
        {
            var endpoint = ChannelEndpoints.GetOrCreateAsync(channelType, channelId);

            return Post<ChannelGetOrCreateRequest, ChannelGetOrCreateRequestDTO, ChannelState, ChannelStateResponseDTO>(endpoint,
                getOrCreateRequest);
        }

        public Task<UpdateChannelResponse> UpdateChannelAsync(string channelType, string channelId, UpdateChannelRequest updateChannelRequest)
        {
            var endpoint = ChannelEndpoints.Update(channelType, channelId);

            return Post<UpdateChannelRequest, UpdateChannelRequestDTO, UpdateChannelResponse, UpdateChannelResponseDTO>(endpoint,
                updateChannelRequest);
        }

        public Task<UpdateChannelPartialResponse> UpdateChannelPartialAsync(string channelType, string channelId, UpdateChannelPartialRequest updateChannelPartialRequest)
        {
            var endpoint = ChannelEndpoints.UpdatePartial(channelType, channelId);

            return Patch<UpdateChannelPartialRequest, UpdateChannelPartialRequestDTO, UpdateChannelPartialResponse, UpdateChannelPartialResponseDTO>(endpoint,
                updateChannelPartialRequest);
        }
    }
}