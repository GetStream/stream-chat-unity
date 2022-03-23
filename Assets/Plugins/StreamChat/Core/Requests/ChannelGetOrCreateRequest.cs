using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public partial class ChannelGetOrCreateRequest : RequestObjectBase, ISavableTo<ChannelGetOrCreateRequestDTO>
    {
        public ChannelRequest Data { get; set; }

        public PaginationParamsRequest Members { get; set; }

        public MessagePaginationParamsRequest Messages { get; set; }

        /// <summary>
        /// Fetch user presence info
        /// </summary>
        public bool? Presence { get; set; }

        /// <summary>
        /// Refresh channel state
        /// </summary>
        public bool? State { get; set; }

        /// <summary>
        /// Start watching the channel
        /// </summary>
        public bool? Watch { get; set; }

        public PaginationParamsRequest Watchers { get; set; }

        ChannelGetOrCreateRequestDTO ISavableTo<ChannelGetOrCreateRequestDTO>.SaveToDto()
        {
            return new ChannelGetOrCreateRequestDTO
            {
                AdditionalProperties = AdditionalProperties,
                Data = Data.TrySaveToDto(),
                Members = Members.TrySaveToDto(),
                Messages = Messages.TrySaveToDto(),
                Presence = Presence,
                State = State,
                Watch = Watch,
                Watchers = Watchers.TrySaveToDto()
            };
        }
    }
}