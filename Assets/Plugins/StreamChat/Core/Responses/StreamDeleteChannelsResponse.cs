using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.Responses
{
    //StreamTodo: replace all ILoadableFrom to IStateLoadableFrom so we can separate the interfaces
    /// <summary>
    /// Response for <see cref="StreamChatClient.DeleteMultipleChannelsAsync"/>
    /// </summary>
    public sealed class
        StreamDeleteChannelsResponse : ILoadableFrom<DeleteChannelsResponseInternalDTO, StreamDeleteChannelsResponse>
    {
        public System.Collections.Generic.Dictionary<string, StreamDeleteChannelsResult> Result { get; private set; }

        /// <summary>
        /// ID of the channels delete request server task. This can be used to check the status of this operation.
        /// </summary>
        public string TaskId { get; private set; }

        StreamDeleteChannelsResponse ILoadableFrom<DeleteChannelsResponseInternalDTO, StreamDeleteChannelsResponse>.
            LoadFromDto(DeleteChannelsResponseInternalDTO dto)
        {
            Result = Result.TryLoadFromDtoDictionary(dto.Result);
            TaskId = dto.TaskId;

            return this;
        }
    }
}