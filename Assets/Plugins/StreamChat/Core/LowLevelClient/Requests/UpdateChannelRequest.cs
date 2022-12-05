using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class UpdateChannelRequest : RequestObjectBase, ISavableTo<UpdateChannelRequestInternalDTO>
    {
        /// <summary>
        /// Set to `true` to accept the invite
        /// </summary>
        public bool? AcceptInvite { get; set; }

        /// <summary>
        /// List of user IDs to add to the channel
        /// </summary>
        public System.Collections.Generic.List<ChannelMemberRequest> AddMembers { get; set; }

        /// <summary>
        /// List of user IDs to make channel moderators
        /// </summary>
        public System.Collections.Generic.List<string> AddModerators { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>
        /// List of channel member role assignments. If any specified user is not part of the channel, the request will fail
        /// </summary>
        public System.Collections.Generic.List<ChannelMemberRequest> AssignRoles { get; set; }

        /// <summary>
        /// Sets cool down period for the channel in seconds
        /// </summary>
        public int? Cooldown { get; set; }

        /// <summary>
        /// Channel data to update
        /// </summary>
        public ChannelRequest Data { get; set; }

        /// <summary>
        /// List of user IDs to take away moderators status from
        /// </summary>
        public System.Collections.Generic.List<string> DemoteModerators { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>
        /// Set to `true` to hide channel's history when adding new members
        /// </summary>
        public bool? HideHistory { get; set; }

        /// <summary>
        /// List of user IDs to invite to the channel
        /// </summary>
        public System.Collections.Generic.List<ChannelMemberRequest> Invites { get; set; }

        /// <summary>
        /// Message to send to the chat when channel is successfully updated
        /// </summary>
        public MessageRequest Message { get; set; }

        /// <summary>
        /// Set to `true` to reject the invite
        /// </summary>
        public bool? RejectInvite { get; set; }

        /// <summary>
        /// List of user IDs to remove from the channel
        /// </summary>
        public System.Collections.Generic.List<string> RemoveMembers { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>
        /// When `message` is set disables all push notifications for it
        /// </summary>
        public bool? SkipPush { get; set; }

        UpdateChannelRequestInternalDTO ISavableTo<UpdateChannelRequestInternalDTO>.SaveToDto() =>
            new UpdateChannelRequestInternalDTO
            {
                AcceptInvite = AcceptInvite,
                AddMembers = AddMembers.TrySaveToDtoCollection<ChannelMemberRequest, ChannelMemberRequestInternalDTO>(),
                AddModerators = AddModerators,
                AssignRoles = AssignRoles.TrySaveToDtoCollection<ChannelMemberRequest, ChannelMemberRequestInternalDTO>(),
                Cooldown = Cooldown,
                Data = Data.TrySaveToDto(),
                DemoteModerators = DemoteModerators,
                HideHistory = HideHistory,
                Invites = Invites.TrySaveToDtoCollection<ChannelMemberRequest, ChannelMemberRequestInternalDTO>(),
                Message = Message.TrySaveToDto(),
                RejectInvite = RejectInvite,
                RemoveMembers = RemoveMembers,
                SkipPush = SkipPush,
                AdditionalProperties = AdditionalProperties,
            };
    }
}