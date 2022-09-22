using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
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