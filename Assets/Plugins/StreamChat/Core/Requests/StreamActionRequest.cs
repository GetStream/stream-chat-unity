using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    public class StreamActionRequest : ISavableTo<ActionRequestInternalDTO>
    {
        public string Name { get; set; }

        public string Style { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        ActionRequestInternalDTO ISavableTo<ActionRequestInternalDTO>.SaveToDto() =>
            new ActionRequestInternalDTO
            {
                Name = Name,
                Style = Style,
                Text = Text,
                Type = Type,
                Value = Value,
            };
    }
}