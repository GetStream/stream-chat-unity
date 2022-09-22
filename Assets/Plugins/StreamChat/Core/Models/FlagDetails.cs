using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public partial class FlagDetails : ModelBase, ILoadableFrom<FlagDetailsInternalDTO, FlagDetails>
    {
        public AutomodDetails Automod { get; set; }

        FlagDetails ILoadableFrom<FlagDetailsInternalDTO, FlagDetails>.LoadFromDto(FlagDetailsInternalDTO dto)
        {
            Automod = Automod.TryLoadFromDto(dto.Automod);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}