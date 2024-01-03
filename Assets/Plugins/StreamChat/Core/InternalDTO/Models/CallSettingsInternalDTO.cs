﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------


using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Events;

namespace StreamChat.Core.InternalDTO.Models
{
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    internal partial class CallSettingsInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("audio", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public AudioSettingsInternalDTO Audio { get; set; }

        [Newtonsoft.Json.JsonProperty("backstage", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public BackstageSettingsInternalDTO Backstage { get; set; }

        [Newtonsoft.Json.JsonProperty("broadcasting", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public BroadcastSettingsInternalDTO Broadcasting { get; set; }

        [Newtonsoft.Json.JsonProperty("geofencing", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public GeofenceSettingsInternalDTO Geofencing { get; set; }

        [Newtonsoft.Json.JsonProperty("recording", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RecordSettingsInternalDTO Recording { get; set; }

        [Newtonsoft.Json.JsonProperty("ring", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RingSettingsInternalDTO Ring { get; set; }

        [Newtonsoft.Json.JsonProperty("screensharing", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ScreensharingSettingsInternalDTO Screensharing { get; set; }

        [Newtonsoft.Json.JsonProperty("transcription", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public TranscriptionSettingsInternalDTO Transcription { get; set; }

        [Newtonsoft.Json.JsonProperty("video", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public VideoSettingsInternalDTO Video { get; set; }

        private System.Collections.Generic.Dictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.Dictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }

    }

}

