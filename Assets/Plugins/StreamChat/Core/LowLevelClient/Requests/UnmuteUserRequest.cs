using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UnmuteUserRequest : RequestObjectBase, ISavableTo<UnmuteUserRequestInternalDTO>
    {
        /// <summary>
        /// User IDs to mute (if multiple users)
        /// </summary>
        public System.Collections.Generic.List<string> TargetIds { get; set; } = new System.Collections.Generic.List<string>();

        UnmuteUserRequestInternalDTO ISavableTo<UnmuteUserRequestInternalDTO>.SaveToDto() =>
            new UnmuteUserRequestInternalDTO
            {
                AdditionalProperties = AdditionalProperties,
                TargetIds = TargetIds,
            };
    }
}