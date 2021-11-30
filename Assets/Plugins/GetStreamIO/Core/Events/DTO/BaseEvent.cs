using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Events.DTO
{
    /// <summary>
    /// Base class for received events
    /// </summary>
    public abstract class BaseEvent
    {
        [JsonProperty("type")]
        public string Type;
    }
}