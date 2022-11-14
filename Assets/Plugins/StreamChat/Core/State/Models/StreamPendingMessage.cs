﻿using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.State.Models
{
    public partial class StreamPendingMessage : ModelBase, IStateLoadableFrom<PendingMessageInternalDTO, StreamPendingMessage>
    {
        /// <summary>
        /// The message
        /// </summary>
        public IStreamMessage Message { get; private set; }

        /// <summary>
        /// Additional data attached to the pending message. This data is discarded once the pending message is committed.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> Metadata { get; private set; }

        StreamPendingMessage IStateLoadableFrom<PendingMessageInternalDTO, StreamPendingMessage>.LoadFromDto(PendingMessageInternalDTO dto, ICache cache)
        {
            Message = cache.TryCreateOrUpdate(dto.Message);
            Metadata = dto.Metadata;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}