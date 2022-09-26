using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StreamChat.Libs.Http
{
    /// <summary>
    /// .NET <see cref="HttpClient"/> adapter
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

        public async Task<IHttpRequestResponse> GetAsync(Uri uri)
        {
            var httpResponse = await _httpClient.GetAsync(uri);
            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            return new NetHttpRequestResponse(responseBody, httpResponse);
        }

        public async Task<IHttpRequestResponse> PostAsync(Uri uri, string content)
        {
            var httpResponse = await _httpClient.PostAsync(uri, new StringContent(content));
            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            return new NetHttpRequestResponse(responseBody, httpResponse);
        }

        public async Task<IHttpRequestResponse> PostAsync(Uri uri, HttpContent content)
        {
            var httpResponse = await _httpClient.PostAsync(uri, content);
            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            return new NetHttpRequestResponse(responseBody, httpResponse);
        }


        public async Task<IHttpRequestResponse> PatchAsync(Uri uri, string content)
        {
            var httpResponse = await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), uri)
                { Content = new StringContent(content) });
            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            return new NetHttpRequestResponse(responseBody, httpResponse);
        }

        public async Task<IHttpRequestResponse> DeleteAsync(Uri uri)
        {
            var httpResponse = await _httpClient.DeleteAsync(uri);
            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            return new NetHttpRequestResponse(responseBody, httpResponse);
        }

        private readonly HttpClient _httpClient;
    }
}