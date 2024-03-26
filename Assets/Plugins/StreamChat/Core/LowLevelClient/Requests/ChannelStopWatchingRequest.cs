using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class ChannelStopWatchingRequest : RequestObjectBase, ISavableTo<ChannelStopWatchingRequestInternalDTO>
    {
        ChannelStopWatchingRequestInternalDTO ISavableTo<ChannelStopWatchingRequestInternalDTO>.SaveToDto() =>
            new ChannelStopWatchingRequestInternalDTO
            {
                AdditionalProperties = AdditionalProperties,
            };
    }
}