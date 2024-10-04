using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Responses
{
    public sealed class StreamDeleteChannelsResult : ILoadableFrom<DeleteChannelsResultResponseInternalDTO, StreamDeleteChannelsResult>
    {
        public string Error { get; private set; }

        public string Status { get; private set; }

        StreamDeleteChannelsResult ILoadableFrom<DeleteChannelsResultResponseInternalDTO, StreamDeleteChannelsResult>.LoadFromDto(DeleteChannelsResultResponseInternalDTO dto)
        {
            Error = dto.Error;
            Status = dto.Status;

            return this;
        }
    }
}