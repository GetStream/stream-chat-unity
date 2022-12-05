using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class BanRequest : RequestObjectBase, ISavableTo<BanRequestInternalDTO>
    {
        /// <summary>
        /// User who issued a ban
        /// </summary>
        public UserObjectRequest BannedBy { get; set; }

        /// <summary>
        /// User ID who issued a ban
        /// </summary>
        public string BannedById { get; set; }

        /// <summary>
        /// Channel ID to ban user in
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Whether to perform IP ban or not
        /// </summary>
        public bool? IpBan { get; set; }

        /// <summary>
        /// Ban reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Whether to perform shadow ban or not
        /// </summary>
        public bool? Shadow { get; set; }

        /// <summary>
        /// ID of user to ban
        /// </summary>
        public string TargetUserId { get; set; }

        /// <summary>
        /// Timeout of ban in minutes. User will be unbanned after this period of time
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// Channel type to ban user in
        /// </summary>
        public string Type { get; set; }

        public UserObjectRequest User { get; set; }

        public string UserId { get; set; }

        BanRequestInternalDTO ISavableTo<BanRequestInternalDTO>.SaveToDto()
        {
            return new BanRequestInternalDTO
            {
                BannedBy = BannedBy.TrySaveToDto(),
                BannedById = BannedById,
                Id = Id,
                IpBan = IpBan,
                Reason = Reason,
                Shadow = Shadow,
                TargetUserId = TargetUserId,
                Timeout = Timeout,
                Type = Type,
                User = User.TrySaveToDto(),
                UserId = UserId,
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}