using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Events;

namespace StreamChat.Core.LowLevelClient.Requests
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