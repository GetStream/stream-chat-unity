using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class FlagResponse : ResponseObjectBase, ILoadableFrom<FlagResponseInternalDTO, FlagResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Flag Flag { get; set; }

        FlagResponse ILoadableFrom<FlagResponseInternalDTO, FlagResponse>.LoadFromDto(FlagResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Flag = Flag.TryLoadFromDto(dto.Flag);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}