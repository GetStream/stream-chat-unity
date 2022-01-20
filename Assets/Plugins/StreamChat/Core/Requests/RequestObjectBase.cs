using System.Collections.Generic;

namespace StreamChat.Core.Requests
{
    public abstract class RequestObjectBase
    {
        public IDictionary<string, object> AdditionalProperties { get; set; }
    }
}