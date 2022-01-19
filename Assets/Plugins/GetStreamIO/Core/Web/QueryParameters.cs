using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core.Web
{
    public class QueryParameters : Dictionary<string, string>
    {
        public static QueryParameters Create() => new QueryParameters();
    }
}