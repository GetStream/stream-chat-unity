using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class ChannelGetOrCreateRequest : RequestObjectBase, ISavableTo<ChannelGetOrCreateRequestInternalDTO>
    {
        public ChannelRequest Data { get; set; }
        
        /// <summary>
        /// Whether this channel will be hidden for the user who created the channel or not
        /// </summary>
        public bool? HideForCreator { get; set; }

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

        ChannelGetOrCreateRequestInternalDTO ISavableTo<ChannelGetOrCreateRequestInternalDTO>.SaveToDto()
        {
            return new ChannelGetOrCreateRequestInternalDTO
            {
                AdditionalProperties = AdditionalProperties,
                Data = Data.TrySaveToDto(),
                HideForCreator = HideForCreator,
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