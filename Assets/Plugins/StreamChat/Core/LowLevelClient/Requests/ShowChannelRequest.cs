using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class ShowChannelRequest : RequestObjectBase, ISavableTo<ShowChannelRequestInternalDTO>
    {
        ShowChannelRequestInternalDTO ISavableTo<ShowChannelRequestInternalDTO>.SaveToDto() =>
            new ShowChannelRequestInternalDTO
            {
                AdditionalProperties = AdditionalProperties,
            };
    }
}