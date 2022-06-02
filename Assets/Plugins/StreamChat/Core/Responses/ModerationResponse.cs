using StreamChat.Core.DTO.Responses;

namespace StreamChat.Core.Responses
{
    public partial class ModerationResponse : ResponseObjectBase, ILoadableFrom<ModerationResponseDTO, ModerationResponse>
    {
        public float? Explicit { get; set; }

        public float? Spam { get; set; }

        public float? Toxic { get; set; }

        ModerationResponse ILoadableFrom<ModerationResponseDTO, ModerationResponse>.LoadFromDto(ModerationResponseDTO dto)
        {
            Explicit = dto.Explicit;
            Spam = dto.Spam;
            Toxic = dto.Toxic;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}