using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public class ActionRequest : RequestObjectBase, ISavableTo<ActionRequestDTO>
    {
        public string Name { get; set; }

        public string Style { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        ActionRequestDTO ISavableTo<ActionRequestDTO>.SaveToDto() =>
            new ActionRequestDTO
            {
                Name = Name,
                Style = Style,
                Text = Text,
                Type = Type,
                Value = Value,
                AdditionalProperties = AdditionalProperties
            };
    }
}