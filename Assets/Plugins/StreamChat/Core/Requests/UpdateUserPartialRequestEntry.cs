using System.Collections.Generic;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public partial class UpdateUserPartialRequestEntry : RequestObjectBase, ISavableTo<UpdateUserPartialRequestEntryDTO>
    {
        /// <summary>
        /// User ID to update
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Sets new field values
        /// </summary>
        public Dictionary<string, object> Set { get; set; }

        /// <summary>
        /// Array of field names to unset
        /// </summary>
        public List<string> Unset { get; set; }

        UpdateUserPartialRequestEntryDTO ISavableTo<UpdateUserPartialRequestEntryDTO>.SaveToDto()
        {
            return new UpdateUserPartialRequestEntryDTO
            {
                Id = Id,
                Set = Set,
                Unset = Unset,
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}