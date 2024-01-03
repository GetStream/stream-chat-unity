using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Requests
{
    public sealed class StreamUpdateOverwriteChannelRequest: ISavableTo<UpdateChannelRequestInternalDTO>
    {
        /// <summary>
        /// Set to `true` to accept the invite
        /// </summary>
        public bool? AcceptInvite { get; set; }

        /// <summary>
        /// List of user IDs to add to the channel
        /// </summary>
        public List<StreamChannelMemberRequest> AddMembers { get; set; }

        /// <summary>
        /// List of user IDs to make channel moderators
        /// </summary>
        public List<string> AddModerators { get; set; }

        /// <summary>
        /// List of channel member role assignments. If any specified user is not part of the channel, the request will fail
        /// </summary>
        public List<StreamChannelMemberRequest> AssignRoles { get; set; }

        /// <summary>
        /// Sets cool down period for the channel in seconds
        /// </summary>
        public int? Cooldown { get; set; }

        #region ChannelRequest
        
        /// <summary>
        /// Enable or disable auto translation
        /// </summary>
        public bool? AutoTranslationEnabled { get; set; }

        /// <summary>
        /// Switch auto translation language
        /// </summary>
        public string AutoTranslationLanguage { get; set; }

        public bool? Disabled { get; set; }

        /// <summary>
        /// Freeze or unfreeze the channel
        /// </summary>
        public bool? Frozen { get; set; }

        public List<string> OwnCapabilities { get; set; }

        /// <summary>
        /// Team the channel belongs to (if multi-tenant mode is enabled)
        /// </summary>
        public string Team { get; set; }
        
        public string Name { get; set; }
        
        #endregion

        /// <summary>
        /// List of user IDs to take away moderators status from
        /// </summary>
        public List<string> DemoteModerators { get; set; }

        /// <summary>
        /// Set to `true` to hide channel's history when adding new members
        /// </summary>
        public bool? HideHistory { get; set; }

        /// <summary>
        /// List of user IDs to invite to the channel
        /// </summary>
        public List<StreamChannelMemberRequest> Invites { get; set; }

        /// <summary>
        /// Message to send to the chat when channel is successfully updated
        /// </summary>
        public StreamMessageRequest Message { get; set; }

        /// <summary>
        /// Set to `true` to reject the invite
        /// </summary>
        public bool? RejectInvite { get; set; }

        /// <summary>
        /// List of user IDs to remove from the channel
        /// </summary>
        public List<string> RemoveMembers { get; set; }

        /// <summary>
        /// When `message` is set disables all push notifications for it
        /// </summary>
        public bool? SkipPush { get; set; }
        
        /// <summary>
        /// Any custom data to associate with this channel. This will be accessible through <see cref="IStreamStatefulModel.CustomData"/>
        /// </summary>
        public StreamCustomDataRequest CustomData { get; set; }

        /// <summary>
        /// Create update request from an instance of <see cref="IStreamChannel"/>. This way you copy channel's current state and can only 
        /// </summary>
        /// <param name="channel">Channel to copy data from into the request body</param>
        public StreamUpdateOverwriteChannelRequest(IStreamChannel channel)
        {
            Cooldown = channel.Cooldown;
            AutoTranslationEnabled = channel.AutoTranslationEnabled;
            AutoTranslationLanguage = channel.AutoTranslationLanguage;
            Disabled = channel.Disabled;
            Frozen = channel.Frozen;
            OwnCapabilities = channel.OwnCapabilities?.ToList();
            Team = channel.Team;
            Name = channel.Name;

            if (channel.CustomData.Count > 0)
            {
                CustomData = new StreamCustomDataRequest(channel.CustomData);
            }
        }

        /// <summary>
        /// Create empty update request.
        /// Warning! Any channel data that is present on the server and is not present in this request will be removed.
        /// </summary>
        public StreamUpdateOverwriteChannelRequest()
        {
            
        }

        UpdateChannelRequestInternalDTO ISavableTo<UpdateChannelRequestInternalDTO>.SaveToDto()
        {
            return new UpdateChannelRequestInternalDTO
            {
                AcceptInvite = AcceptInvite,
                AddMembers = AddMembers?.TrySaveToDtoCollection<StreamChannelMemberRequest, ChannelMemberRequestInternalDTO>(),
                AddModerators = AddModerators,
                AssignRoles = AssignRoles?.TrySaveToDtoCollection<StreamChannelMemberRequest, ChannelMemberRequestInternalDTO>(),
                Cooldown = Cooldown,
                Data = new ChannelRequestInternalDTO
                {
                    AutoTranslationEnabled = AutoTranslationEnabled,
                    AutoTranslationLanguage = AutoTranslationLanguage,
                    Disabled = Disabled,
                    Frozen = Frozen,
                    OwnCapabilities = OwnCapabilities,
                    Team = Team,
                    AdditionalProperties = CustomData?.ToDictionary(),
                    Name = Name
                },
                DemoteModerators = DemoteModerators,
                HideHistory = HideHistory,
                Invites = Invites?.TrySaveToDtoCollection<StreamChannelMemberRequest, ChannelMemberRequestInternalDTO>(),
                Message = Message?.TrySaveToDto(),
                RejectInvite = RejectInvite,
                RemoveMembers = RemoveMembers,
                SkipPush = SkipPush,
            };
        }
    }
}