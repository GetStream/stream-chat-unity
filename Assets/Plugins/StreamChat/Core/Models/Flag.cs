using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public partial class Flag : ModelBase, ILoadableFrom<FlagDTO, Flag>
    {
        /// <summary>
        /// Date of the approval
        /// </summary>
        public System.DateTimeOffset? ApprovedAt { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        public bool? CreatedByAutomod { get; set; }

        public FlagDetails Details { get; set; }

        /// <summary>
        /// Date of the rejection
        /// </summary>
        public System.DateTimeOffset? RejectedAt { get; set; }

        /// <summary>
        /// Date of the review
        /// </summary>
        public System.DateTimeOffset? ReviewedAt { get; set; }

        public Message TargetMessage { get; set; }

        /// <summary>
        /// ID of flagged message
        /// </summary>
        public string TargetMessageId { get; set; }

        /// <summary>
        /// Flagged user
        /// </summary>
        public User TargetUser { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// User who flagged a message or a user
        /// </summary>
        public User User { get; set; }

        Flag ILoadableFrom<FlagDTO, Flag>.LoadFromDto(FlagDTO dto)
        {
            ApprovedAt = dto.ApprovedAt;
            CreatedAt = dto.CreatedAt;
            CreatedByAutomod = dto.CreatedByAutomod;
            Details = Details.TryLoadFromDto(dto.Details);
            RejectedAt = dto.RejectedAt;
            ReviewedAt = dto.ReviewedAt;
            ReviewedAt = dto.ReviewedAt;
            TargetMessage = TargetMessage.TryLoadFromDto<MessageDTO, Message>(dto.TargetMessage);
            TargetMessageId = dto.TargetMessageId;
            TargetUser = TargetUser.TryLoadFromDto<UserObjectDTO, User>(dto.TargetUser);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}