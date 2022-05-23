using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class MuteUserRequest : RequestObjectBase, ISavableTo<MuteUserRequestDTO>
    {
        /// <summary>
        /// User IDs to mute (if multiple users)
        /// </summary>
        public System.Collections.Generic.List<string> TargetIds { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>
        /// Duration of mute in minutes
        /// </summary>
        public double? Timeout { get; set; }

        MuteUserRequestDTO ISavableTo<MuteUserRequestDTO>.SaveToDto() =>
            new MuteUserRequestDTO
            {
                AdditionalProperties = AdditionalProperties,
                TargetIds = TargetIds,
                Timeout = Timeout,
            };
    }
}