using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core.Requests
{
    public abstract class RequestObjectBase
    {
        public IDictionary<string, object> AdditionalProperties { get; set; }
    }
}