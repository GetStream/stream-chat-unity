using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public class ActionRequest : RequestObjectBase, ISavableTo<ActionRequestInternalDTO>
    {
        public string Name { get; set; }

        public string Style { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        ActionRequestInternalDTO ISavableTo<ActionRequestInternalDTO>.SaveToDto()
            => new ActionRequestInternalDTO
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