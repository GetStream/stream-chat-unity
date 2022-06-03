using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class FlagRequest : RequestObjectBase, ISavableTo<FlagRequestDTO>
    {
        /// <summary>
        /// ID of the message when reporting a message
        /// </summary>
        public string TargetMessageId { get; set; }

        /// <summary>
        /// ID of the user when reporting a user
        /// </summary>
        public string TargetUserId { get; set; }

        FlagRequestDTO ISavableTo<FlagRequestDTO>.SaveToDto() =>
            new FlagRequestDTO
            {
                TargetMessageId = TargetMessageId,
                TargetUserId = TargetUserId,
                AdditionalProperties = AdditionalProperties,
            };
    }
}