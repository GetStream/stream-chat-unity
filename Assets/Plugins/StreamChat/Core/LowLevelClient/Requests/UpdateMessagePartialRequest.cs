using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UpdateMessagePartialRequest : RequestObjectBase,
        ISavableTo<UpdateMessagePartialRequestInternalDTO>
    {
        /// <summary>
        /// Sets new field values
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> Set { get; set; }
            = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Do not try to enrich the links within message
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        /// <summary>
        /// Array of field names to unset
        /// </summary>
        public System.Collections.Generic.List<string> Unset { get; set; }
            = new System.Collections.Generic.List<string>();

        UpdateMessagePartialRequestInternalDTO ISavableTo<UpdateMessagePartialRequestInternalDTO>.SaveToDto()
            => new UpdateMessagePartialRequestInternalDTO
            {
                Set = Set,
                SkipEnrichUrl = SkipEnrichUrl,
                Unset = Unset,
                AdditionalProperties = AdditionalProperties
            };
    }
}