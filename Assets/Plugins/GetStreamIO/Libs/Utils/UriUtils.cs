using System.Collections.Generic;

namespace Plugins.GetStreamIO.Libs.Utils
{
    /// <summary>
    /// Uri utility methods
    /// </summary>
    public static class UriUtils
    {
        public static string ToQueryParams(this IDictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach(var item in dict)
            {
                list.Add(item.Key + "=" + item.Value);
            }
            return string.Join("&", list);
        }
    }
}