using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Requests
{
    public partial class DeleteChannelsRequest : ResponseObjectBase, ISavableTo<DeleteChannelsRequestDTO>
    {
        /// <summary>
        /// All channels that should be deleted
        /// </summary>
        public System.Collections.Generic.ICollection<string> Cids { get; set; }

        /// <summary>
        /// Specify if channels and all ressources should be hard deleted
        /// </summary>
        public bool? HardDelete { get; set; }

        DeleteChannelsRequestDTO ISavableTo<DeleteChannelsRequestDTO>.SaveToDto() =>
            new DeleteChannelsRequestDTO
            {
                Cids = Cids,
                HardDelete = HardDelete,
                AdditionalProperties = AdditionalProperties,
            };

    }
}