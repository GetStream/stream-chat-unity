using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Requests
{
    public partial class UnmuteChannelRequest : ResponseObjectBase, ISavableTo<UnmuteChannelRequestDTO>
    {
        /// <summary>
        /// Channel CIDs to mute (if multiple channels)
        /// </summary>
        public System.Collections.Generic.ICollection<string> ChannelCids { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>
        /// Duration of mute in milliseconds
        /// </summary>
        public double? Expiration { get; set; }

        UnmuteChannelRequestDTO ISavableTo<UnmuteChannelRequestDTO>.SaveToDto() =>
            new UnmuteChannelRequestDTO
            {
                ChannelCids = ChannelCids,
                Expiration = Expiration,
                AdditionalProperties = AdditionalProperties,
            };
    }
}