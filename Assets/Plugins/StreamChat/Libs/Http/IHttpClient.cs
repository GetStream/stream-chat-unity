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

        Task<IHttpRequestResponse> PostAsync(Uri uri, string content);

        Task<IHttpRequestResponse> DeleteAsync(Uri uri);

        Task<IHttpRequestResponse> GetAsync(Uri uri);

        Task<IHttpRequestResponse> PatchAsync(Uri uri, string content);

        Task<IHttpRequestResponse> PostAsync(Uri uri, HttpContent content);
    }
}