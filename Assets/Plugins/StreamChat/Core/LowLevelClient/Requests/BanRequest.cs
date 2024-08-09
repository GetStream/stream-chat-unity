using System;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class BanRequest : RequestObjectBase, ISavableTo<BanRequestInternalDTO>
    {
        //StreamTodo: isn't this server-side only? 
        /// <summary>
        /// User who issued a ban
        /// </summary>
        public UserObjectRequest BannedBy { get; set; }

        //StreamTodo: isn't this server-side only? 
        /// <summary>
        /// User ID who issued a ban
        /// </summary>
        public string BannedById { get; set; }
        
        /// <summary>
        /// Channel CID to ban user in eg. messaging:123
        /// </summary>
        public string ChannelCid { get; set; }

        [Obsolete("Has no effect and will be removed in a future release")] //StreamTODO: remove this in a major release
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

        [Obsolete("Has no effect and will be removed in a future release")] //StreamTODO: remove this in a major release
        public string Type { get; set; }

        [Obsolete("Has no effect and will be removed in a future release")] //StreamTODO: remove this in a major release
        public UserObjectRequest User { get; set; }

        [Obsolete("Has no effect and will be removed in a future release")] //StreamTODO: remove this in a major release
        public string UserId { get; set; }

        BanRequestInternalDTO ISavableTo<BanRequestInternalDTO>.SaveToDto()
        {
            return new BanRequestInternalDTO
            {
                BannedBy = BannedBy.TrySaveToDto<UserRequestInternalDTO>(),
                BannedById = BannedById,
                ChannelCid = ChannelCid,
                IpBan = IpBan,
                Reason = Reason,
                Shadow = Shadow,
                TargetUserId = TargetUserId,
                Timeout = Timeout,
                AdditionalProperties = AdditionalProperties,
            };
        }
    }
}