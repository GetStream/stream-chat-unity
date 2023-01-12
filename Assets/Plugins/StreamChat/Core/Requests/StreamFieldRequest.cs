using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
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