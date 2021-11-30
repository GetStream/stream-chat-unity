using System;
using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Events.DTO
{
    /// <summary>
    /// Health check event
    /// </summary>
    public class HealthCheckEvent : BaseEvent
    {
        [JsonProperty("connection_id")]
        public string ConnectionId;

        [JsonProperty("created_at")]
        public DateTime CreatedAt;
    }
}