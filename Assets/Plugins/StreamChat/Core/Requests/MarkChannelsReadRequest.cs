using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public partial class MarkChannelsReadRequest : RequestObjectBase, ISavableTo<MarkChannelsReadRequestDTO>
    {
        /// <summary>
        /// Map which binds a CID to a message ID that is considered last read by client. If message ID is empty, the whole channel will be considered as read
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> ReadByChannel { get; set; }

        MarkChannelsReadRequestDTO ISavableTo<MarkChannelsReadRequestDTO>.SaveToDto() =>
            new MarkChannelsReadRequestDTO
            {
                ReadByChannel = ReadByChannel,
                AdditionalProperties = AdditionalProperties
            };
    }
}