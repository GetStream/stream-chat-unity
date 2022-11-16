using System.Threading.Tasks;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    internal interface IInternalChannelApi
    {
        Task<ShowChannelResponseInternalDTO> ShowChannelAsync(string channelType, string channelId,
            ShowChannelRequestInternalDTO showChannelRequest);

        Task<ChannelsResponseInternalDTO> QueryChannelsAsync(QueryChannelsRequestInternalDTO queryChannelsRequest);

        Task<ChannelStateResponseInternalDTO> GetOrCreateChannelAsync(string channelType,
            ChannelGetOrCreateRequestInternalDTO getOrCreateRequest);

        Task<ChannelStateResponseInternalDTO> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequestInternalDTO getOrCreateRequest);

        Task<UpdateChannelResponseInternalDTO> UpdateChannelAsync(string channelType, string channelId,
            UpdateChannelRequestInternalDTO updateChannelRequest);

        Task<UpdateChannelPartialResponseInternalDTO> UpdateChannelPartialAsync(string channelType, string channelId,
            UpdateChannelPartialRequestInternalDTO updateChannelPartialRequest);

        Task<DeleteChannelsResponseInternalDTO> DeleteChannelsAsync(DeleteChannelsRequestInternalDTO deleteChannelsRequest);

        Task<DeleteChannelResponseInternalDTO> DeleteChannelAsync(string channelType, string channelId, bool isHardDelete);

        Task<DeleteChannelResponseInternalDTO> DeleteChannelAsync(string channelType, string channelId);

        Task<TruncateChannelResponseInternalDTO> TruncateChannelAsync(string channelType, string channelId,
            TruncateChannelRequestInternalDTO truncateChannelRequest);

        Task<MuteChannelResponseInternalDTO> MuteChannelAsync(MuteChannelRequestInternalDTO muteChannelRequest);

        Task<UnmuteResponseInternalDTO> UnmuteChannelAsync(UnmuteChannelRequestInternalDTO unmuteChannelRequest);

        Task<HideChannelResponseInternalDTO> HideChannelAsync(string channelType, string channelId,
            HideChannelRequestInternalDTO hideChannelRequest);

        Task<MembersResponseInternalDTO> QueryMembersAsync(QueryMembersRequestInternalDTO queryMembersRequest);

        Task<StopWatchingResponseInternalDTO> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequestInternalDTO channelStopWatchingRequest);

        Task<MarkReadResponseInternalDTO> MarkReadAsync(string channelType, string channelId,
            MarkReadRequestInternalDTO markReadRequest);

        Task<MarkReadResponseInternalDTO> MarkManyReadAsync(MarkChannelsReadRequestInternalDTO markChannelsReadRequest);

        Task SendTypingStartEventAsync(string channelType, string channelId);

        Task SendTypingStopEventAsync(string channelType, string channelId);
    }
}