using StreamChat.Core;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public partial class HideChannelRequest : RequestObjectBase, ISavableTo<HideChannelRequestDTO>
    {
        /// <summary>
        /// Whether to clear message history of the channel or not
        /// </summary>
        public bool? ClearHistory { get; set; }

        public HideChannelRequestDTO SaveToDto() =>
            new HideChannelRequestDTO
            {
                ClearHistory = ClearHistory,
                AdditionalProperties = AdditionalProperties,
            };
    }
}