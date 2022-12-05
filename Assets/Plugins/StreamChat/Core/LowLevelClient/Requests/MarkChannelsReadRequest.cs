using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class MarkChannelsReadRequest : RequestObjectBase, ISavableTo<MarkChannelsReadRequestInternalDTO>
    {
        /// <summary>
        /// Map which binds a CID to a message ID that is considered last read by client. If message ID is empty, the whole channel will be considered as read
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> ReadByChannel { get; set; }

        MarkChannelsReadRequestInternalDTO ISavableTo<MarkChannelsReadRequestInternalDTO>.SaveToDto() =>
            new MarkChannelsReadRequestInternalDTO
            {
                ReadByChannel = ReadByChannel,
                AdditionalProperties = AdditionalProperties
            };
    }
}