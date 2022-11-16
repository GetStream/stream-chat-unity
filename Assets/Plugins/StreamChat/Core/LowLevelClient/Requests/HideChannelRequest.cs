using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class HideChannelRequest : RequestObjectBase, ISavableTo<HideChannelRequestInternalDTO>
    {
        /// <summary>
        /// Whether to clear message history of the channel or not
        /// </summary>
        public bool? ClearHistory { get; set; }

        HideChannelRequestInternalDTO ISavableTo<HideChannelRequestInternalDTO>.SaveToDto() =>
            new HideChannelRequestInternalDTO
            {
                ClearHistory = ClearHistory,
                AdditionalProperties = AdditionalProperties,
            };
    }
}