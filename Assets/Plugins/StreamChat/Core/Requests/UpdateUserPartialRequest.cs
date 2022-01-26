using StreamChat.Core;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Core.Requests
{
    public partial class UpdateUserPartialRequest : RequestObjectBase, ISavableTo<UpdateUserPartialRequestDTO>
    {
        /// <summary>
        /// User ID to update
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Sets new field values
        /// </summary>
        public System.Collections.Generic.IDictionary<string, object> Set { get; set; } = new System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Array of field names to unset
        /// </summary>
        public System.Collections.Generic.ICollection<string> Unset { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        public UpdateUserPartialRequestDTO SaveToDto()
        {
            return new UpdateUserPartialRequestDTO
            {
                Id = Id,
                Set = Set,
                Unset = Unset,
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}