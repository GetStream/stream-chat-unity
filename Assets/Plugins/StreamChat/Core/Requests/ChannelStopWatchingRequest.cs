using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class ChannelStopWatchingRequest : RequestObjectBase, ISavableTo<ChannelStopWatchingRequestDTO>
    {
        ChannelStopWatchingRequestDTO ISavableTo<ChannelStopWatchingRequestDTO>.SaveToDto() =>
            new ChannelStopWatchingRequestDTO
            {
                AdditionalProperties = AdditionalProperties,
            };
    }
}