using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamAttachmentField : ILoadableFrom<FieldInternalDTO, StreamAttachmentField>
    {
        public bool? Short { get; set; }

        public string Title { get; set; }

        public string Value { get; set; }

        StreamAttachmentField ILoadableFrom<FieldInternalDTO, StreamAttachmentField>.LoadFromDto(FieldInternalDTO dto)
        {
            Short = dto.Short;
            Title = dto.Title;
            Value = dto.Value;

            return this;
        }
    }
}