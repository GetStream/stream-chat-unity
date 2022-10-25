﻿using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.State.Responses
{
    /// <summary>
    /// Response for <see cref="StreamChatStateClient.DeleteMultipleChannelsAsync"/>
    /// </summary>
    public sealed class StreamDeleteChannelsResponse : ILoadableFrom<DeleteChannelsResponseInternalDTO, StreamDeleteChannelsResponse>
    {
        public System.Collections.Generic.Dictionary<string, StreamDeleteChannelsResult> Result { get; set; }

        /// <summary>
        /// ID of the channels delete request server task. This can be used to check the status of this operation.
        /// </summary>
        public string TaskId { get; set; }

        StreamDeleteChannelsResponse ILoadableFrom<DeleteChannelsResponseInternalDTO, StreamDeleteChannelsResponse>.LoadFromDto(DeleteChannelsResponseInternalDTO dto)
        {
            Result = Result.TryLoadFromDtoDictionary(dto.Result);
            TaskId = dto.TaskId;

            return this;
        }
    }
}