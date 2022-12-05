using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
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