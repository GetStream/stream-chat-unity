using System.Threading.Tasks;
using UnityEngine;

namespace StreamChat.Libs.Auth
{
    /// <summary>
    /// Providers JWT authorization token for Stream Chat
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Get JWT token for the provided user id
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#token-providers</remarks>
        Task<string> GetTokenAsync(string userId);
    }
}