using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class HideChannelRequest : RequestObjectBase, ISavableTo<HideChannelRequestDTO>
    {
        /// <summary>
        /// Whether to clear message history of the channel or not
        /// </summary>
        public bool? ClearHistory { get; set; }

        HideChannelRequestDTO ISavableTo<HideChannelRequestDTO>.SaveToDto() =>
            new HideChannelRequestDTO
            {
                ClearHistory = ClearHistory,
                AdditionalProperties = AdditionalProperties,
            };
    }
}