using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// User model
    /// </summary>
    public class User
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("image")]
        public string Image;

        //Todo: why is there Image and ImageUrl?
        [JsonProperty("image_url")]
        public string ImageUrl;
    }
}