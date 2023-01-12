using System.Collections.Generic;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public abstract class RequestObjectBase
    {
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}