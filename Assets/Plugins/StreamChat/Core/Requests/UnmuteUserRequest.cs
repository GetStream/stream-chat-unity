using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class UnmuteUserRequest : RequestObjectBase, ISavableTo<UnmuteUserRequestDTO>
    {
        /// <summary>
        /// User IDs to mute (if multiple users)
        /// </summary>
        public System.Collections.Generic.List<string> TargetIds { get; set; } = new System.Collections.Generic.List<string>();

        UnmuteUserRequestDTO ISavableTo<UnmuteUserRequestDTO>.SaveToDto() =>
            new UnmuteUserRequestDTO
            {
                AdditionalProperties = AdditionalProperties,
                TargetIds = TargetIds,
            };
    }
}