using System;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Models
{
    //StreamTodo: this could contain the last read StreamMessage
    public class StreamRead : IStateLoadableFrom<ReadInternalDTO, StreamRead>,
        IStateLoadableFrom<ReadStateResponseInternalDTO, StreamRead>
    {
        public DateTimeOffset LastRead { get; private set; }

        public int UnreadMessages { get; private set; }

        public IStreamUser User { get; private set; }

        StreamRead IStateLoadableFrom<ReadInternalDTO, StreamRead>.LoadFromDto(ReadInternalDTO dto, ICache cache)
        {
            //Is this always set? What if a user marks empty channel as read? 
            LastRead = dto.LastRead; //StreamTodo: GetValueOrThrow? 
            UnreadMessages = dto.UnreadMessages;
            User = cache.TryCreateOrUpdate(dto.User);

            return this;
        }
        
        StreamRead IStateLoadableFrom<ReadStateResponseInternalDTO, StreamRead>.LoadFromDto(ReadStateResponseInternalDTO dto, ICache cache)
        {
            //Is this always set? What if a user marks empty channel as read? 
            LastRead = dto.LastRead; //StreamTodo: GetValueOrThrow? 
            UnreadMessages = dto.UnreadMessages;
            User = cache.TryCreateOrUpdate(dto.User);

            return this;
        }

        internal void Update(DateTimeOffset lastRead) => LastRead = lastRead;
    }
}