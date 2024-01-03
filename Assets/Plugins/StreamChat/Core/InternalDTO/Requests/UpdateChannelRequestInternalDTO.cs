﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------


using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.InternalDTO.Requests
{
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    internal partial class UpdateChannelRequestInternalDTO
    {
        /// <summary>
        /// Set to `true` to accept the invite
        /// </summary>
        [Newtonsoft.Json.JsonProperty("accept_invite", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? AcceptInvite { get; set; }

        /// <summary>
        /// List of user IDs to add to the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("add_members", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<ChannelMemberRequestInternalDTO> AddMembers { get; set; }

        /// <summary>
        /// List of user IDs to make channel moderators
        /// </summary>
        [Newtonsoft.Json.JsonProperty("add_moderators", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<string> AddModerators { get; set; }

        /// <summary>
        /// List of channel member role assignments. If any specified user is not part of the channel, the request will fail
        /// </summary>
        [Newtonsoft.Json.JsonProperty("assign_roles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<ChannelMemberRequestInternalDTO> AssignRoles { get; set; }

        /// <summary>
        /// Sets cool down period for the channel in seconds
        /// </summary>
        [Newtonsoft.Json.JsonProperty("cooldown", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Cooldown { get; set; }

        [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ChannelRequestInternalDTO Data { get; set; }

        /// <summary>
        /// List of user IDs to take away moderators status from
        /// </summary>
        [Newtonsoft.Json.JsonProperty("demote_moderators", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<string> DemoteModerators { get; set; }

        /// <summary>
        /// Set to `true` to hide channel's history when adding new members
        /// </summary>
        [Newtonsoft.Json.JsonProperty("hide_history", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? HideHistory { get; set; }

        /// <summary>
        /// List of user IDs to invite to the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("invites", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<ChannelMemberRequestInternalDTO> Invites { get; set; }

        /// <summary>
        /// Message to send to the chat when channel is successfully updated
        /// </summary>
        [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public MessageRequestInternalDTO Message { get; set; }

        /// <summary>
        /// Set to `true` to reject the invite
        /// </summary>
        [Newtonsoft.Json.JsonProperty("reject_invite", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? RejectInvite { get; set; }

        /// <summary>
        /// List of user IDs to remove from the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("remove_members", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<string> RemoveMembers { get; set; }

        /// <summary>
        /// When `message` is set disables all push notifications for it
        /// </summary>
        [Newtonsoft.Json.JsonProperty("skip_push", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? SkipPush { get; set; }

        [Newtonsoft.Json.JsonProperty("user", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public UserObjectRequestInternalDTO User { get; set; }

        [Newtonsoft.Json.JsonProperty("user_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string UserId { get; set; }

        private System.Collections.Generic.Dictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.Dictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

    }

}

