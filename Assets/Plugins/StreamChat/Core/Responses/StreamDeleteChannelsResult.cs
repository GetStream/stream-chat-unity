using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.Responses
{
    public sealed class StreamDeleteChannelsResult : ILoadableFrom<DeleteChannelsResultInternalDTO, StreamDeleteChannelsResult>
    {
        public string Error { get; private set; }

        public string Status { get; private set; }

        StreamDeleteChannelsResult ILoadableFrom<DeleteChannelsResultInternalDTO, StreamDeleteChannelsResult>.LoadFromDto(DeleteChannelsResultInternalDTO dto)
        {
            Error = dto.Error;
            Status = dto.Status;

            return this;
        }
    }
}