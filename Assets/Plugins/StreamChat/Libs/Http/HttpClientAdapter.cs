using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StreamChat.Libs.Http
{
    /// <summary>
    /// .NET http client adapter
    /// </summary>
    public class HttpClientAdapter : IHttpClient
    {
        public HttpClientAdapter()
        {
            _httpClient = new HttpClient();
        }

        public void SetDefaultAuthenticationHeader(string value)
            => _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(value);

        public void AddDefaultCustomHeader(string key, string value)
            => _httpClient.DefaultRequestHeaders.Add(key, value);

        public Task<HttpResponseMessage> GetAsync(Uri uri) => _httpClient.GetAsync(uri);

        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content) => _httpClient.PostAsync(uri, content);

        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
            => _httpClient.PutAsync(uri, content);

        public Task<HttpResponseMessage> PatchAsync(Uri uri, HttpContent content)
            => _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), uri)
                { Content = content });

        public Task<HttpResponseMessage> DeleteAsync(Uri uri) => _httpClient.DeleteAsync(uri);

        private readonly HttpClient _httpClient;
    }
}