using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public class FieldRequest : RequestObjectBase, ISavableTo<FieldRequestDTO>
    {
        public bool Short { get; set; }

        public string Title { get; set; }

        public string Value { get; set; }

        FieldRequestDTO ISavableTo<FieldRequestDTO>.SaveToDto() =>
            new FieldRequestDTO
            {
                Short = Short,
                Title = Title,
                Value = Value,
                AdditionalProperties = AdditionalProperties
            };
    }
}