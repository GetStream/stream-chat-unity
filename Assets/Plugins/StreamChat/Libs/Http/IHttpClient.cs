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

        Task<HttpResponse> GetAsync(Uri uri);

        Task<HttpResponse> PostAsync(Uri uri, HttpContent content);

        Task<HttpResponse> PutAsync(Uri uri, HttpContent content);

        Task<HttpResponse> PatchAsync(Uri uri, HttpContent content);

        Task<HttpResponse> DeleteAsync(Uri uri);
    }
}