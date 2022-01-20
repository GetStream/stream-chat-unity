using GetStreamIO.Core.DTO.Requests;
using Plugins.GetStreamIO.Core.Helpers;

namespace Plugins.GetStreamIO.Core.Requests
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

        /// <summary>
        /// **Server-side only**. User object which server acts upon
        /// </summary>
        public UserObjectRequest User { get; set; }

        /// <summary>
        /// **Server-side only**. User ID which server acts upon
        /// </summary>
        public string UserId { get; set; }

        public UpdateChannelPartialRequestDTO SaveToDto() =>
            new UpdateChannelPartialRequestDTO
            {
                Set = Set,
                Unset = Unset,
                User = User.TrySaveToDto(),
                UserId = UserId,
                AdditionalProperties = AdditionalProperties,
            };
    }
}