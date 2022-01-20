using System.Threading.Tasks;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.API
{
    public interface IChannelApi
    {
        Task<ChannelsResponse> QueryChannelsAsync(QueryChannelsRequest queryChannelsRequest);

        Task<ChannelState> GetOrCreateChannelAsync(string channelType, ChannelGetOrCreateRequest getOrCreateRequest);

        Task<ChannelState> GetOrCreateChannelAsync(string channelType, string channelId, ChannelGetOrCreateRequest getOrCreateRequest);

        Task<UpdateChannelResponse> UpdateChannelAsync(string channelType, string channelId, UpdateChannelRequest updateChannelRequest);

        Task<UpdateChannelPartialResponse> UpdateChannelPartialAsync(string channelType, string channelId, UpdateChannelPartialRequest updateChannelPartialRequest);
    }
}