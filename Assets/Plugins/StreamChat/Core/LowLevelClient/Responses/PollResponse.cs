using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Responses
{
    /// <summary>
    /// Response containing a poll
    /// </summary>
    public partial class PollResponse : ResponseObjectBase, ILoadableFrom<PollResponseInternalDTO, PollResponse>
    {
        public Poll Poll { get; set; }

        PollResponse ILoadableFrom<PollResponseInternalDTO, PollResponse>.LoadFromDto(PollResponseInternalDTO dto)
        {
            Poll = Poll.TryLoadFromDto<PollResponseDataInternalDTO, Poll>(dto.Poll);
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

