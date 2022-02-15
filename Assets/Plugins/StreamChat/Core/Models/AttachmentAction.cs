using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.Models
{
    public class AttachmentAction : ModelBase, ILoadableFrom<ActionDTO, AttachmentAction>
    {
        public string Name { get; set; }

        public string Style { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        AttachmentAction ILoadableFrom<ActionDTO, AttachmentAction>.LoadFromDto(ActionDTO dto)
        {
            Name = dto.Name;
            Style = dto.Style;
            Text = dto.Text;
            Type = dto.Type;
            Value = dto.Value;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}