using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.State.Requests
{
    public class StreamFieldRequest : ISavableTo<FieldRequestInternalDTO>
    {
        public bool Short { get; set; }

        public string Title { get; set; }

        public string Value { get; set; }

        FieldRequestInternalDTO ISavableTo<FieldRequestInternalDTO>.SaveToDto() =>
            new FieldRequestInternalDTO
            {
                Short = Short,
                Title = Title,
                Value = Value,
            };
    }
}