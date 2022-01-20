using System.IO;
using System.Text;

namespace StreamChat.Libs.Utils
{
    public static class CommunicationUtils
    {
        /// <summary>
        /// Converts memory stream into string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="ms">Memory Stream.</param>
        /// <param name="encoding">Encoding.</param>
        public static string StreamToString(MemoryStream ms, Encoding encoding)
        {
            if (encoding != Encoding.UTF8)
            {
                return "";
            }

            using (var reader = new StreamReader(ms, encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}