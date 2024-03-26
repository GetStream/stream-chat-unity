using System.Collections.Generic;
using System.Text;

namespace StreamChat.Libs.Utils
{
    /// <summary>
    /// Uri utility methods
    /// </summary>
    public static class UriUtils
    {
        public static string ToQueryParameters(this IReadOnlyDictionary<string, string> dict)
        {
            _sb.Length = 0;

            foreach(var item in dict)
            {
                if (_sb.Length > 0)
                {
                    _sb.Append("&");
                }

                _sb.Append(item.Key);
                _sb.Append("=");
                _sb.Append(item.Value); //StreamTodo: Uri.EscapeDataString ?
            }

            return _sb.ToString();
        }

        private static readonly StringBuilder _sb = new StringBuilder();
    }
}