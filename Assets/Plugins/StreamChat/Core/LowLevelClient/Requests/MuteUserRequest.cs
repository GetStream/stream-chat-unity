using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class MuteUserRequest : RequestObjectBase, ISavableTo<MuteUserRequestInternalDTO>
    {
        /// <summary>
        /// User IDs to mute (if multiple users)
        /// </summary>
        public System.Collections.Generic.List<string> TargetIds { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>
        /// Duration of mute in minutes
        /// </summary>
        public int? Timeout { get; set; }

        MuteUserRequestInternalDTO ISavableTo<MuteUserRequestInternalDTO>.SaveToDto() =>
            new MuteUserRequestInternalDTO
            {
                AdditionalProperties = AdditionalProperties,
                TargetIds = TargetIds,
                Timeout = Timeout,
            };
    }
}