using StreamChat.Core;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public partial class ChannelStopWatchingRequest : RequestObjectBase, ISavableTo<ChannelStopWatchingRequestDTO>
    {
        /// <summary>
        /// Websocket connection ID to interact with. You can pass it as body or URL parameter
        /// </summary>
        public string ConnectionId { get; set; }

        public ChannelStopWatchingRequestDTO SaveToDto() =>
            new ChannelStopWatchingRequestDTO
            {
                ConnectionId = ConnectionId,
                AdditionalProperties = AdditionalProperties,
            };
    }
}