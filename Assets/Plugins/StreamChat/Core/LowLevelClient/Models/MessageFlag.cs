using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public partial class MessageFlag : ModelBase, ILoadableFrom<MessageFlagInternalDTO, MessageFlag>
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

        MessageFlag ILoadableFrom<MessageFlagInternalDTO, MessageFlag>.LoadFromDto(MessageFlagInternalDTO dto)
        {
            ApprovedAt = dto.ApprovedAt;
            CreatedAt = dto.CreatedAt;
            CreatedByAutomod = dto.CreatedByAutomod;
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            ModerationResult = ModerationResult.TryLoadFromDto(dto.ModerationResult);
            RejectedAt = dto.RejectedAt;
            ReviewedAt = dto.ReviewedAt;
            ReviewedBy = ReviewedBy.TryLoadFromDto<UserObjectInternalDTO, User>(dto.ReviewedBy);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectInternalDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}