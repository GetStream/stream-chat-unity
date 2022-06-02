using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public partial class MessageFlag : ModelBase, ILoadableFrom<MessageFlagDTO, MessageFlag>
    {
        public System.DateTimeOffset? ApprovedAt { get; set; }

        public System.DateTimeOffset? CreatedAt { get; set; }

        public bool? CreatedByAutomod { get; set; }

        public Message Message { get; set; }

        public MessageModerationResult ModerationResult { get; set; }

        public System.DateTimeOffset? RejectedAt { get; set; }

        public System.DateTimeOffset? ReviewedAt { get; set; }

        public User ReviewedBy { get; set; }

        public System.DateTimeOffset? UpdatedAt { get; set; }

        public User User { get; set; }

        MessageFlag ILoadableFrom<MessageFlagDTO, MessageFlag>.LoadFromDto(MessageFlagDTO dto)
        {
            ApprovedAt = dto.ApprovedAt;
            CreatedAt = dto.CreatedAt;
            CreatedByAutomod = dto.CreatedByAutomod;
            Message = Message.TryLoadFromDto(dto.Message);
            ModerationResult = ModerationResult.TryLoadFromDto(dto.ModerationResult);
            RejectedAt = dto.RejectedAt;
            ReviewedAt = dto.ReviewedAt;
            ReviewedBy = ReviewedBy.TryLoadFromDto<UserObjectDTO, User>(dto.ReviewedBy);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}