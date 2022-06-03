using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class FlagResponse : ResponseObjectBase, ILoadableFrom<FlagResponseDTO, FlagResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Flag Flag { get; set; }

        FlagResponse ILoadableFrom<FlagResponseDTO, FlagResponse>.LoadFromDto(FlagResponseDTO dto)
        {
            Duration = dto.Duration;
            Flag = Flag.TryLoadFromDto(dto.Flag);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}