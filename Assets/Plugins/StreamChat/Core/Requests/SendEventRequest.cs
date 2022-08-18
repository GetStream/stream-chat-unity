using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Events;

namespace StreamChat.Core.Requests
{
    internal partial class SendEventRequest<TEvent, TEventDto>: EventBase, ISavableTo<SendEventRequestDTO>
        where TEvent : ISavableTo<TEventDto>
    {
        public TEvent Event { get; set; }

        SendEventRequestDTO ISavableTo<SendEventRequestDTO>.SaveToDto() =>
            new SendEventRequestDTO
            {
                Event = Event.SaveToDto()
            };
    }
}