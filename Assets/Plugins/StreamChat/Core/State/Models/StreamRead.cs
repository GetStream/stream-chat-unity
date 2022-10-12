using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State.Models
{
    public class StreamRead : IStateLoadableFrom<ReadInternalDTO, StreamRead>
    {
        public System.DateTimeOffset? LastRead { get; set; }

        public int? UnreadMessages { get; set; }

        public StreamUser User { get; set; }

        StreamRead IStateLoadableFrom<ReadInternalDTO, StreamRead>.LoadFromDto(ReadInternalDTO dto, ICache cache)
        {
            LastRead = dto.LastRead;
            UnreadMessages = dto.UnreadMessages;
            User = cache.TryCreateOrUpdate(dto.User);

            return this;
        }
    }
}