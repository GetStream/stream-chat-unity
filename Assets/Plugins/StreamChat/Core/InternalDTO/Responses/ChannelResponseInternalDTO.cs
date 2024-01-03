﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------


using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.InternalDTO.Responses
{
    using System = global::System;

    /// <summary>
    /// Represents channel in chat
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    internal partial class ChannelResponseInternalDTO
    {
        /// <summary>
        /// Whether auto translation is enabled or not
        /// </summary>
        [Newtonsoft.Json.JsonProperty("auto_translation_enabled", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? AutoTranslationEnabled { get; set; }

        /// <summary>
        /// Language to translate to when auto translation is active
        /// </summary>
        [Newtonsoft.Json.JsonProperty("auto_translation_language", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string AutoTranslationLanguage { get; set; }

        /// <summary>
        /// Channel CID (&lt;type&gt;:&lt;id&gt;)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("cid", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Cid { get; set; }

        /// <summary>
        /// Channel configuration
        /// </summary>
        [Newtonsoft.Json.JsonProperty("config", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ChannelConfigWithInfoInternalDTO Config { get; set; }

        /// <summary>
        /// Cooldown period after sending each message
        /// </summary>
        [Newtonsoft.Json.JsonProperty("cooldown", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Cooldown { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        [Newtonsoft.Json.JsonProperty("created_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Creator of the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("created_by", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public UserObjectInternalDTO CreatedBy { get; set; }

        /// <summary>
        /// Date/time of deletion
        /// </summary>
        [Newtonsoft.Json.JsonProperty("deleted_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? DeletedAt { get; set; }

        [Newtonsoft.Json.JsonProperty("disabled", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Whether channel is frozen or not
        /// </summary>
        [Newtonsoft.Json.JsonProperty("frozen", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Frozen { get; set; }

        /// <summary>
        /// Whether this channel is hidden by current user or not
        /// </summary>
        [Newtonsoft.Json.JsonProperty("hidden", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Hidden { get; set; }

        /// <summary>
        /// Date since when the message history is accessible
        /// </summary>
        [Newtonsoft.Json.JsonProperty("hide_messages_before", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? HideMessagesBefore { get; set; }

        /// <summary>
        /// Channel unique ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Date of the last message sent
        /// </summary>
        [Newtonsoft.Json.JsonProperty("last_message_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? LastMessageAt { get; set; }

        /// <summary>
        /// Number of members in the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("member_count", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? MemberCount { get; set; }

        /// <summary>
        /// List of channel members (max 100)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("members", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<ChannelMemberInternalDTO> Members { get; set; }

        /// <summary>
        /// Date of mute expiration
        /// </summary>
        [Newtonsoft.Json.JsonProperty("mute_expires_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? MuteExpiresAt { get; set; }

        /// <summary>
        /// Whether this channel is muted or not
        /// </summary>
        [Newtonsoft.Json.JsonProperty("muted", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Muted { get; set; }

        /// <summary>
        /// List of channel capabilities of authenticated user
        /// </summary>
        [Newtonsoft.Json.JsonProperty("own_capabilities", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<string> OwnCapabilities { get; set; }

        /// <summary>
        /// Team the channel belongs to (multi-tenant only)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("team", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Team { get; set; }

        /// <summary>
        /// Date of the latest truncation of the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("truncated_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? TruncatedAt { get; set; }

        [Newtonsoft.Json.JsonProperty("truncated_by", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public UserObjectInternalDTO TruncatedBy { get; set; }

        /// <summary>
        /// Type of the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        [Newtonsoft.Json.JsonProperty("updated_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? UpdatedAt { get; set; }

        private System.Collections.Generic.Dictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.Dictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

    }

}

