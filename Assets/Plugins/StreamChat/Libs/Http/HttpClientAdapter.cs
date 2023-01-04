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

        public async Task<HttpResponse> GetAsync(Uri uri)
        {
            var response = await _httpClient.GetAsync(uri);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> PostAsync(Uri uri, HttpContent content)
        {
            var response = await _httpClient.PostAsync(uri, content);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> PutAsync(Uri uri, HttpContent content)
        {
            var response = await _httpClient.PutAsync(uri, content);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> PatchAsync(Uri uri, HttpContent content)
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), uri)
                { Content = content });
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> DeleteAsync(Uri uri)
        {
            var response = await _httpClient.DeleteAsync(uri);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        private readonly HttpClient _httpClient;
    }
}