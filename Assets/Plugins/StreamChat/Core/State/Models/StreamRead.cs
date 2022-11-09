using System;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State.Models
{
    //StreamTodo: this could contain the last read StreamMessage
    public class StreamRead : IStateLoadableFrom<ReadInternalDTO, StreamRead>
    {
        public DateTimeOffset LastRead { get; private set; }

        public int UnreadMessages { get; private set; }

        public StreamUser User { get; private set; }

        StreamRead IStateLoadableFrom<ReadInternalDTO, StreamRead>.LoadFromDto(ReadInternalDTO dto, ICache cache)
        {
            //Is this always set? What if a user marks empty channel as read? 
            LastRead = dto.LastRead.GetValueOrDefault(); //StreamTodo: GetValueOrThrow? 
            UnreadMessages = dto.UnreadMessages.GetValueOrDefault();
            User = cache.TryCreateOrUpdate(dto.User);

            return this;
        }

        internal void Update(DateTimeOffset lastRead) => LastRead = lastRead;
    }
}