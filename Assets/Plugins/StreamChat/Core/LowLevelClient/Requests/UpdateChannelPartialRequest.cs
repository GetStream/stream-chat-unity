using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UpdateChannelPartialRequest : RequestObjectBase, ISavableTo<UpdateChannelPartialRequestInternalDTO>
    {
        /// <summary>
        /// Sets new field values
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> Set { get; set; } = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Array of field names to unset
        /// </summary>
        public System.Collections.Generic.List<string> Unset { get; set; } = new System.Collections.Generic.List<string>();

        UpdateChannelPartialRequestInternalDTO ISavableTo<UpdateChannelPartialRequestInternalDTO>.SaveToDto() =>
            new UpdateChannelPartialRequestInternalDTO
            {
                Set = Set,
                Unset = Unset,
                AdditionalProperties = AdditionalProperties,
            };
    }
}