using System.Threading.Tasks;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.API.Internal
{
    internal interface IInternalChannelApi
    {
        Task<ShowChannelResponseDTO> ShowChannelAsync(string channelType, string channelId,
            ShowChannelRequestDTO showChannelRequest);

        Task<ChannelsResponseDTO> QueryChannelsAsync(QueryChannelsRequestDTO queryChannelsRequest);

        Task<ChannelStateResponseDTO> GetOrCreateChannelAsync(string channelType,
            ChannelGetOrCreateRequestDTO getOrCreateRequest);

        Task<ChannelStateResponseDTO> GetOrCreateChannelAsync(string channelType, string channelId,
            ChannelGetOrCreateRequestDTO getOrCreateRequest);

        Task<UpdateChannelResponseDTO> UpdateChannelAsync(string channelType, string channelId,
            UpdateChannelRequestDTO updateChannelRequest);

        Task<UpdateChannelPartialResponseDTO> UpdateChannelPartialAsync(string channelType, string channelId,
            UpdateChannelPartialRequestDTO updateChannelPartialRequest);

        Task<DeleteChannelsResponseDTO> DeleteChannelsAsync(DeleteChannelsRequestDTO deleteChannelsRequest);

        Task<DeleteChannelResponseDTO> DeleteChannelAsync(string channelType, string channelId);

        Task<TruncateChannelResponseDTO> TruncateChannelAsync(string channelType, string channelId,
            TruncateChannelRequestDTO truncateChannelRequest);

        Task<MuteChannelResponseDTO> MuteChannelAsync(MuteChannelRequestDTO muteChannelRequest);

        Task<UnmuteResponseDTO> UnmuteChannelAsync(UnmuteChannelRequestDTO unmuteChannelRequest);

        Task<HideChannelResponseDTO> HideChannelAsync(string channelType, string channelId,
            HideChannelRequestDTO hideChannelRequest);

        Task<MembersResponseDTO> QueryMembersAsync(QueryMembersRequestDTO queryMembersRequest);

        Task<StopWatchingResponseDTO> StopWatchingChannelAsync(string channelType, string channelId,
            ChannelStopWatchingRequestDTO channelStopWatchingRequest);

        Task<MarkReadResponseDTO> MarkReadAsync(string channelType, string channelId,
            MarkReadRequestDTO markReadRequest);

        Task<MarkReadResponseDTO> MarkManyReadAsync(MarkChannelsReadRequestDTO markChannelsReadRequest);

        Task SendTypingStartEventAsync(string channelType, string channelId);

        Task SendTypingStopEventAsync(string channelType, string channelId);
    }
}