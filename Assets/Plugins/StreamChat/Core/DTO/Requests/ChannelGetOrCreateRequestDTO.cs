//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------


using StreamChat.Core.DTO.Responses;
using StreamChat.Core.DTO.Events;
using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.DTO.Requests
{
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    internal partial class ChannelGetOrCreateRequestDTO
    {
        /// <summary>
        /// Websocket connection ID to interact with. You can pass it as body or URL parameter
        /// </summary>
        [Newtonsoft.Json.JsonProperty("connection_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ConnectionId { get; set; }

        [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ChannelRequestDTO Data { get; set; }

        [Newtonsoft.Json.JsonProperty("members", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PaginationParamsRequestDTO Members { get; set; }

        [Newtonsoft.Json.JsonProperty("messages", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public MessagePaginationParamsRequestDTO Messages { get; set; }

        /// <summary>
        /// Fetch user presence info
        /// </summary>
        [Newtonsoft.Json.JsonProperty("presence", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Presence { get; set; }

        /// <summary>
        /// Refresh channel state
        /// </summary>
        [Newtonsoft.Json.JsonProperty("state", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? State { get; set; }

        /// <summary>
        /// Start watching the channel
        /// </summary>
        [Newtonsoft.Json.JsonProperty("watch", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Watch { get; set; }

        [Newtonsoft.Json.JsonProperty("watchers", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PaginationParamsRequestDTO Watchers { get; set; }

        private System.Collections.Generic.Dictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.Dictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

    }

}

