using System.Collections.Generic;

namespace StreamChat.Core.Web
{
    internal class QueryParameters : Dictionary<string, string>
    {
        public static QueryParameters Default => new QueryParameters();
    }
}