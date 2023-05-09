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

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    internal partial class SearchResponseInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("duration", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Duration { get; set; }

        /// <summary>
        /// Value to pass to the next search query in order to paginate
        /// </summary>
        [Newtonsoft.Json.JsonProperty("next", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Next { get; set; }

        /// <summary>
        /// Value that points to the previous page. Pass as the next value in a search query to paginate backwards
        /// </summary>
        [Newtonsoft.Json.JsonProperty("previous", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Previous { get; set; }

        /// <summary>
        /// Search results
        /// </summary>
        [Newtonsoft.Json.JsonProperty("results", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<SearchResultInternalDTO> Results { get; set; } = new System.Collections.Generic.List<SearchResultInternalDTO>();

        /// <summary>
        /// Warning about the search results
        /// </summary>
        [Newtonsoft.Json.JsonProperty("results_warning", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SearchWarningInternalDTO ResultsWarning { get; set; }

        private System.Collections.Generic.Dictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.Dictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

    }

}

