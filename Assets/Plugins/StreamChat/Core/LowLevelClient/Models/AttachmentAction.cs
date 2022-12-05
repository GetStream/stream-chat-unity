using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class AttachmentAction : ModelBase, ILoadableFrom<ActionInternalDTO, AttachmentAction>
    {
        public string Name { get; set; }

        public string Style { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        AttachmentAction ILoadableFrom<ActionInternalDTO, AttachmentAction>.LoadFromDto(ActionInternalDTO dto)
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