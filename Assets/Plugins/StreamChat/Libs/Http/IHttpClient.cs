using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamChat.Libs.Http
{
    /// <summary>
    /// Http client
    /// </summary>
    public interface IHttpClient
    {
        void SetDefaultAuthenticationHeader(string value);

        void AddDefaultCustomHeader(string key, string value);

        Task<HttpResponseMessage> GetAsync(Uri uri);

        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);

        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content);

        Task<HttpResponseMessage> PatchAsync(Uri uri, HttpContent content);

        Task<HttpResponseMessage> DeleteAsync(Uri uri);
    }
}