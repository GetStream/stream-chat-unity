using System.Collections.Generic;

namespace StreamChat.Core.Requests
{
    public abstract class RequestObjectBase
    {
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}