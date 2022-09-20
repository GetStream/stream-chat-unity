using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public partial class FlagDetails : ModelBase, ILoadableFrom<FlagDetailsDTO, FlagDetails>
    {
        public AutomodDetails Automod { get; set; }

        FlagDetails ILoadableFrom<FlagDetailsDTO, FlagDetails>.LoadFromDto(FlagDetailsDTO dto)
        {
            Automod = Automod.TryLoadFromDto(dto.Automod);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}