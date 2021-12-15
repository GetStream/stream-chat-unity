using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Plugins.GetStreamIO.Libs.Http
{
    /// <summary>
    /// Http client
    /// </summary>
    public interface IHttpClient
    {
        void SetDefaultAuthenticationHeader(string value);

        void AddDefaultCustomHeader(string key, string value);

        Task<HttpResponseMessage> PostAsync(Uri uri, ByteArrayContent content);

        Task<HttpResponseMessage> PostAsync(Uri uri, string content);

        Task<HttpResponseMessage> DeleteAsync(Uri uri);
    }
}