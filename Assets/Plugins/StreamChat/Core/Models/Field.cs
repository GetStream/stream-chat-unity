using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.Models
{
    public class Field : ModelBase, ILoadableFrom<FieldDTO, Field>
    {
        public bool? Short { get; set; }

        public string Title { get; set; }

        public string Value { get; set; }

        Field ILoadableFrom<FieldDTO, Field>.LoadFromDto(FieldDTO dto)
        {
            Short = dto.Short;
            Title = dto.Title;
            Value = dto.Value;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}