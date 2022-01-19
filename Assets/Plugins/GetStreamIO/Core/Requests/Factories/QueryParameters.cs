using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core.Requests.Factories
{
    public class QueryParameters : Dictionary<string, string>
    {
        public static QueryParameters Create() => new QueryParameters();
    }
}