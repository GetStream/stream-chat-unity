using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    /// <summary>
    /// Response containing a poll option
    /// </summary>
    public partial class PollOptionResponse : ResponseObjectBase, ILoadableFrom<PollOptionResponseInternalDTO, PollOptionResponse>
    {
        public PollOption PollOption { get; set; }

        PollOptionResponse ILoadableFrom<PollOptionResponseInternalDTO, PollOptionResponse>.LoadFromDto(PollOptionResponseInternalDTO dto)
        {
            PollOption = PollOption.TryLoadFromDto<PollOptionResponseDataInternalDTO, PollOption>(dto.PollOption);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

