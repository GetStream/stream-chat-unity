using System.Collections.Generic;
using System.Text;

namespace StreamChat.Libs.Utils
{
    /// <summary>
    /// Uri utility methods
    /// </summary>
    public static class UriUtils
    {
        public static string ToQueryParameters(this IDictionary<string, string> dict)
        {
            var sb = new StringBuilder();

            foreach(var item in dict)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }

                sb.Append(item.Key);
                sb.Append("=");
                sb.Append(item.Value); //Todo: Uri.EscapeDataString ?
            }

            return sb.ToString();
        }
    }
}