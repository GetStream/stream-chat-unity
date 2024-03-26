using System;
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

        Task<HttpResponse> PostAsync(Uri uri, object content);

        Task<HttpResponse> PutAsync(Uri uri, object content);

        Task<HttpResponse> PatchAsync(Uri uri, object content);

        Task<HttpResponse> DeleteAsync(Uri uri);

        Task<HttpResponse> SendHttpRequestAsync(HttpMethodType methodType, Uri uri, object optionalRequestContent);
    }
}