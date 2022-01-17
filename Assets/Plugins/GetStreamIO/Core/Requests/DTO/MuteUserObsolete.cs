using Newtonsoft.Json;

namespace Plugins.GetStreamIO.Core.Requests.DTO
{
    public class MuteUserObsolete
    {
        //This field works but it's not in Open API spec, is it left as a backward compatibility?
        [JsonProperty("target_id")]
        public string Id;
    }
}