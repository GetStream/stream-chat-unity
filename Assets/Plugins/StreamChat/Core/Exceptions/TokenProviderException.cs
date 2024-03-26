using System;
using StreamChat.Libs.Auth;

namespace StreamChat.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when the injected <see cref="ITokenProvider"/> fails to return a token
    /// </summary>
    public class TokenProviderException : Exception
    {
        public TokenProviderException(string message)
            : base(message)
        {
        }
        
        public TokenProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}