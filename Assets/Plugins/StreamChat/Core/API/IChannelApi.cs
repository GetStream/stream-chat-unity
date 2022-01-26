using System.Threading.Tasks;
using Plugins.StreamChat.Core.Requests;
using Plugins.StreamChat.Core.Responses;
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

        Task<DeleteChannelsResponse> DeleteChannelsAsync(DeleteChannelsRequest deleteChannelsRequest);

        Task<DeleteChannelResponse> DeleteChannelAsync(string channelType, string channelId);

        Task<TruncateChannelResponse> TruncateChannelAsync(string channelType, string channelId, TruncateChannelRequest truncateChannelRequest);

        Task<MuteChannelResponse> MuteChannelAsync(MuteChannelRequest muteChannelRequest);

        Task<UnmuteResponse> UnmuteChannelAsync(UnmuteChannelRequest unmuteChannelRequest);

        Task<ShowChannelResponse> ShowChannelAsync(string channelType, string channelId, ShowChannelRequest showChannelRequest);

        Task<HideChannelResponse> HideChannelAsync(string channelType, string channelId, HideChannelRequest hideChannelRequest);

        Task<MembersResponse> QueryMembersAsync(QueryMembersRequest queryMembersRequest);

        Task<StopWatchingResponse> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequest channelStopWatchingRequest);
    }
}