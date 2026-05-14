using System.Collections.Generic;

namespace StreamChat.Core.StatefulModels
{
    /// <summary>
    /// Orders messages chronologically by <see cref="IStreamMessage.CreatedAt"/> (oldest first).
    /// </summary>
    internal sealed class MessageCreatedAtComparer : IComparer<IStreamMessage>
    {
        public static readonly MessageCreatedAtComparer Instance = new MessageCreatedAtComparer();

        public int Compare(IStreamMessage x, IStreamMessage y) => x.CreatedAt.CompareTo(y.CreatedAt);

        private MessageCreatedAtComparer()
        {
        }
    }
}
