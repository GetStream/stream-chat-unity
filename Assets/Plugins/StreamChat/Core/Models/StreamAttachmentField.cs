using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamAttachmentField : IStateLoadableFrom<FieldInternalDTO, StreamAttachmentField>
    {
        public bool? Short { get; private set; }

        public string Title { get; private set; }

        public string Value { get; private set; }

        StreamAttachmentField IStateLoadableFrom<FieldInternalDTO, StreamAttachmentField>.LoadFromDto(FieldInternalDTO dto, ICache cache)
        {
            Short = dto.Short;
            Title = dto.Title;
            Value = dto.Value;

            return this;
        }
    }
}