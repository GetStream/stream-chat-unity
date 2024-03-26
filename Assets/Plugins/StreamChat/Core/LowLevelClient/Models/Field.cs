using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class Field : ModelBase, ILoadableFrom<FieldInternalDTO, Field>
    {
        public bool? Short { get; set; }

        public string Title { get; set; }

        public string Value { get; set; }

        Field ILoadableFrom<FieldInternalDTO, Field>.LoadFromDto(FieldInternalDTO dto)
        {
            Short = dto.Short;
            Title = dto.Title;
            Value = dto.Value;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}