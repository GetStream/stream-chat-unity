using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public partial class Flag  : ModelBase, ILoadableFrom<FlagDTO, Flag>
    {
        /// <summary>
        /// Date of the approval
        /// </summary>
        public System.DateTimeOffset? ApprovedAt { get; set; }

        public string ChannelCid { get; set; }

        public string ChannelTeam { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        public bool? CreatedByAutomod { get; set; }

        public string MessageUserId { get; set; }

        /// <summary>
        /// Date of the rejection
        /// </summary>
        public System.DateTimeOffset? RejectedAt { get; set; }

        /// <summary>
        /// Date of the review
        /// </summary>
        public System.DateTimeOffset? ReviewedAt { get; set; }

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
            ChannelCid = dto.ChannelCid;
            ChannelTeam = dto.ChannelTeam;
            CreatedAt = dto.CreatedAt;
            CreatedByAutomod = dto.CreatedByAutomod;
            MessageUserId = dto.MessageUserId;
            RejectedAt = dto.RejectedAt;
            ReviewedAt = dto.ReviewedAt;
            TargetMessageId = dto.TargetMessageId;
            TargetUser = TargetUser.TryLoadFromDto<UserObjectDTO, User>(dto.TargetUser);
            UpdatedAt = dto.UpdatedAt;
            User = User.TryLoadFromDto<UserObjectDTO, User>(dto.User);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}