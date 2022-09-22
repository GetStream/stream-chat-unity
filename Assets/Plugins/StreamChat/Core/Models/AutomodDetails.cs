using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public partial class AutomodDetails : ModelBase, ILoadableFrom<AutomodDetailsInternalDTO, AutomodDetails>
    {
        public string Action { get; set; }

        public System.Collections.Generic.List<string> ImageLabels { get; set; }

        public FlagMessageDetails MessageDetails { get; set; }

        public string OriginalMessageType { get; set; }

        public MessageModerationResult Result { get; set; }

        AutomodDetails ILoadableFrom<AutomodDetailsInternalDTO, AutomodDetails>.LoadFromDto(AutomodDetailsInternalDTO dto)
        {
            Action = dto.Action;
            ImageLabels = dto.ImageLabels;
            MessageDetails = MessageDetails.TryLoadFromDto(dto.MessageDetails);
            OriginalMessageType = dto.OriginalMessageType;
            Result = Result.TryLoadFromDto(dto.Result);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}