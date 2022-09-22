using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Events;

namespace StreamChat.Core.Requests
{
    internal partial class SendEventRequest<TEvent, TEventDto>: EventBase, ISavableTo<SendEventRequestInternalDTO>
        where TEvent : ISavableTo<TEventDto>
    {
        public TEvent Event { get; set; }

        SendEventRequestInternalDTO ISavableTo<SendEventRequestInternalDTO>.SaveToDto() =>
            new SendEventRequestInternalDTO
            {
                Event = Event.SaveToDto()
            };
    }
}