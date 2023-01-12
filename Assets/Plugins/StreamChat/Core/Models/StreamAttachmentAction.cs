using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamAttachmentAction : IStateLoadableFrom<ActionInternalDTO, StreamAttachmentAction>
    {
        public string Name { get; private set; }

        public string Style { get; private set; }

        public string Text { get; private set; }

        public string Type { get; private set; }

        public string Value { get; private set; }

        StreamAttachmentAction IStateLoadableFrom<ActionInternalDTO, StreamAttachmentAction>.LoadFromDto(ActionInternalDTO dto, ICache cache)
        {
            Name = dto.Name;
            Style = dto.Style;
            Text = dto.Text;
            Type = dto.Type;
            Value = dto.Value;

            return this;
        }
    }
}