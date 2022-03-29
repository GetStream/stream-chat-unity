using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class UpdateChannelPartialRequest : RequestObjectBase, ISavableTo<UpdateChannelPartialRequestDTO>
    {
        /// <summary>
        /// Sets new field values
        /// </summary>
        public System.Collections.Generic.IDictionary<string, object> Set { get; set; } = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Array of field names to unset
        /// </summary>
        public System.Collections.Generic.ICollection<string> Unset { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        UpdateChannelPartialRequestDTO ISavableTo<UpdateChannelPartialRequestDTO>.SaveToDto() =>
            new UpdateChannelPartialRequestDTO
            {
                Set = Set,
                Unset = Unset,
                AdditionalProperties = AdditionalProperties,
            };
    }
}