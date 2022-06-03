using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Responses;

namespace StreamChat.Core.Models
{
    public partial class MessageModerationResult : ModelBase, ILoadableFrom<MessageModerationResultDTO, MessageModerationResult>
    {
        public string Action { get; set; }

        public ModerationResponse AiModerationResponse { get; set; }

        public string BlockedWord { get; set; }

        public string BlocklistName { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public string MessageId { get; set; }

        public string ModeratedBy { get; set; }

        public Thresholds ModerationThresholds { get; set; }

        public System.DateTimeOffset? UpdatedAt { get; set; }

        public bool? UserBadKarma { get; set; }

        public float? UserKarma { get; set; }

        MessageModerationResult ILoadableFrom<MessageModerationResultDTO, MessageModerationResult>.LoadFromDto(MessageModerationResultDTO dto)
        {
            Action = dto.Action;
            AiModerationResponse = AiModerationResponse.TryLoadFromDto(dto.AiModerationResponse);
            BlockedWord = dto.BlockedWord;
            BlocklistName = dto.BlocklistName;
            CreatedAt = dto.CreatedAt;
            MessageId = dto.MessageId;
            ModeratedBy = dto.ModeratedBy;
            ModerationThresholds = ModerationThresholds.TryLoadFromDto(dto.ModerationThresholds);
            UpdatedAt = dto.UpdatedAt;
            UserBadKarma = dto.UserBadKarma;
            UserKarma = dto.UserKarma;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}