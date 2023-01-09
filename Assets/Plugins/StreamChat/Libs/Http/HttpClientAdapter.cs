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

        public async Task<HttpResponse> SendHttpRequestAsync(HttpMethodType methodType, Uri uri,
            object optionalRequestContent)
        {
            var httpContent = TryGetHttpContent(optionalRequestContent);

            Task<HttpResponseMessage> ExecuteAsync()
            {
                switch (methodType)
                {
                    case HttpMethodType.Get: return _httpClient.GetAsync(uri);
                    case HttpMethodType.Post: return _httpClient.PostAsync(uri, httpContent);
                    case HttpMethodType.Put: return _httpClient.PutAsync(uri, httpContent);
                    case HttpMethodType.Patch:
                        return _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), uri)
                            { Content = httpContent });
                    case HttpMethodType.Delete: return _httpClient.DeleteAsync(uri);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null);
                }
            }

            var httpResponseMessage = await ExecuteAsync();
            return await HttpResponse.CreateFromHttpResponseMessageAsync(httpResponseMessage);
        }

        public async Task<HttpResponse> GetAsync(Uri uri)
        {
            var response = await _httpClient.GetAsync(uri);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> PostAsync(Uri uri, object content)
        {
            var httpContent = TryGetHttpContent(content);
            var response = await _httpClient.PostAsync(uri, httpContent);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> PutAsync(Uri uri, object content)
        {
            var httpContent = TryGetHttpContent(content);
            var response = await _httpClient.PutAsync(uri, httpContent);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> PatchAsync(Uri uri, object content)
        {
            var httpContent = TryGetHttpContent(content);
            var response = await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), uri)
                { Content = httpContent });
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        public async Task<HttpResponse> DeleteAsync(Uri uri)
        {
            var response = await _httpClient.DeleteAsync(uri);
            return await HttpResponse.CreateFromHttpResponseMessageAsync(response);
        }

        private readonly HttpClient _httpClient;

        private static HttpContent TryGetHttpContent(object content)
        {
            if (content == null)
            {
                return null;
            }

            if (content is string stringContent)
            {
                return new StringContent(stringContent);
            }

            if (content is FileWrapper fileWrapper)
            {
                var body = new MultipartFormDataContent();
                body.Add(new ByteArrayContent(fileWrapper.FileContent), "file", fileWrapper.FileName);
                return body;
            }

            throw new NotImplementedException(content.GetType().ToString());
        }
    }
}