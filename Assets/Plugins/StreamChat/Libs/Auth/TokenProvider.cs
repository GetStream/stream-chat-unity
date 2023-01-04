using System;
using System.Threading.Tasks;
using StreamChat.Libs.Http;

namespace StreamChat.Libs.Auth
{
    /// <summary>
    /// Simple implementation of <see cref="ITokenProvider"/>
    /// You can create instance of this object using <see cref="StreamDependenciesFactory.CreateTokenProvider"/>
    /// </summary>
    public class TokenProvider : ITokenProvider
    {
        public delegate Uri TokenUriHandler(string userId);

        public TokenProvider(IHttpClient httpClient, TokenUriHandler urlFactory)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _urlFactory = urlFactory ?? throw new ArgumentNullException(nameof(urlFactory));
        }

        public async Task<string> GetTokenAsync(string userId)
        {
            var uri = _urlFactory(userId);
            var response = await _httpClient.GetAsync(uri);
            var responseContent = response.Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Token provider failed with status code: " + response.StatusCode +
                                    " and response body: " + responseContent);
            }

            return responseContent;
        }

        private readonly IHttpClient _httpClient;
        private readonly TokenUriHandler _urlFactory;
    }
}