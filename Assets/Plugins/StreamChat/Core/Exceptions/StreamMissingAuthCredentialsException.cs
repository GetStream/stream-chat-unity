using System;

namespace StreamChat.Core.Exceptions
{
    /// <summary>
    /// Thrown when auth credentials are missing
    /// </summary>
    public class StreamMissingAuthCredentialsException : Exception
    {
        public StreamMissingAuthCredentialsException(string message)
            : base(message)
        {
        }
    }
}