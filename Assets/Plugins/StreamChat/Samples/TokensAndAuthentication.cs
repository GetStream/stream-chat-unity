using System;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Libs.Auth;

namespace StreamChat.Samples
{
    internal sealed class TokensAndAuthentication
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#how-to-refresh-expired-tokens
        /// </summary>
        public async Task ConnectWithTokenProvider()
        {
            // If your backend exposes a simple endpoint to get token from a url you can use our predefined token provider and provide delegate for URL construction 
            var tokenProvider = StreamChatClient.CreateDefaultTokenProvider(userId
                => new Uri($"https:your-awesome-page.com/api/get_token?userId={userId}"));
            await Client.ConnectUserAsync("api-key", "local-user-id", tokenProvider);


            // For more advanced cases you can implement the ITokenProvider
            var yourTokenProvider = new YourTokenProvider();
            await Client.ConnectUserAsync("api-key", "local-user-id", yourTokenProvider);
        }

// You can write your own implementation of the ITokenProvider
        public class YourTokenProvider : ITokenProvider
        {
            public async Task<string> GetTokenAsync(string userId)
            {
                // Your logic to get the auth token from your backend and generated with Stream backend SDK
                var token = await Task.FromResult("some-token");
                return token;
            }
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}