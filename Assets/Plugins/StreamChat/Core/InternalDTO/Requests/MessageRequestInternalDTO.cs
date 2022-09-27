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

    /// <summary>
    /// Represents any chat message
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    internal partial class MessageRequestInternalDTO
    {
        /// <summary>
        /// Array of message attachments
        /// </summary>
        [Newtonsoft.Json.JsonProperty("attachments", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<AttachmentRequestInternalDTO> Attachments { get; set; } = new System.Collections.Generic.List<AttachmentRequestInternalDTO>();

        /// <summary>
        /// Channel unique identifier in &lt;type&gt;:&lt;id&gt; format
        /// </summary>
        [Newtonsoft.Json.JsonProperty("cid", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<int> Cid { get; set; }

        /// <summary>
        /// Contains HTML markup of the message. Can only be set when using server-side API
        /// </summary>
        [Newtonsoft.Json.JsonProperty("html", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Html { get; set; }

        /// <summary>
        /// Message ID is unique string identifier of the message
        /// </summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// List of mentioned users
        /// </summary>
        [Newtonsoft.Json.JsonProperty("mentioned_users", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<string> MentionedUsers { get; set; }

        /// <summary>
        /// Should be empty if `text` is provided. Can only be set when using server-side API
        /// </summary>
        [Newtonsoft.Json.JsonProperty("mml", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Mml { get; set; }

        /// <summary>
        /// ID of parent message (thread)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("parent_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ParentId { get; set; }

        /// <summary>
        /// Date when pinned message expires
        /// </summary>
        [Newtonsoft.Json.JsonProperty("pin_expires", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? PinExpires { get; set; }

        /// <summary>
        /// Whether message is pinned or not
        /// </summary>
        [Newtonsoft.Json.JsonProperty("pinned", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Pinned { get; set; }

        /// <summary>
        /// Date when message got pinned
        /// </summary>
        [Newtonsoft.Json.JsonProperty("pinned_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? PinnedAt { get; set; }

        /// <summary>
        /// Contains user who pinned the message
        /// </summary>
        [Newtonsoft.Json.JsonProperty("pinned_by", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<int> PinnedBy { get; set; }

        [Newtonsoft.Json.JsonProperty("quoted_message_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string QuotedMessageId { get; set; }

        /// <summary>
        /// An object containing scores of reactions of each type. Key: reaction type (string), value: total score of reactions (int)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("reaction_scores", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.Dictionary<string, int> ReactionScores { get; set; }

        /// <summary>
        /// Whether thread reply should be shown in the channel as well
        /// </summary>
        [Newtonsoft.Json.JsonProperty("show_in_channel", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? ShowInChannel { get; set; }

        /// <summary>
        /// Whether message is silent or not
        /// </summary>
        [Newtonsoft.Json.JsonProperty("silent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Silent { get; set; }

        /// <summary>
        /// Text of the message. Should be empty if `mml` is provided
        /// </summary>
        [Newtonsoft.Json.JsonProperty("text", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Text { get; set; }

        /// <summary>
        /// Sender of the message. Required when using server-side API
        /// </summary>
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

        public MessageRequestInternalDTO()
        {
        }


    }

}

