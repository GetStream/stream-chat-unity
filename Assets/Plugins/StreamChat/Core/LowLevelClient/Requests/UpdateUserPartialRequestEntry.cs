using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UpdateUserPartialRequestEntry : RequestObjectBase, ISavableTo<UpdateUserPartialRequestEntryInternalDTO>
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

        UpdateUserPartialRequestEntryInternalDTO ISavableTo<UpdateUserPartialRequestEntryInternalDTO>.SaveToDto()
        {
            return new UpdateUserPartialRequestEntryInternalDTO
            {
                Id = Id,
                Set = Set,
                Unset = Unset,
            };
        }
    }
}