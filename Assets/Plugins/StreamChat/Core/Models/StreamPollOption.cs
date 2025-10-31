using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    /// <summary>
    /// Represents an option in a poll
    /// </summary>
    public class StreamPollOption : IStateLoadableFrom<PollOptionInternalDTO, StreamPollOption>,
        IStateLoadableFrom<PollOptionResponseDataInternalDTO, StreamPollOption>
    {
        /// <summary>
        /// Custom data associated with this option
        /// </summary>
        public IReadOnlyDictionary<string, object> Custom { get; private set; }

        /// <summary>
        /// Unique option ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Text displayed for this option
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Additional custom properties
        /// </summary>
        public IReadOnlyDictionary<string, object> AdditionalProperties { get; private set; }

        StreamPollOption IStateLoadableFrom<PollOptionInternalDTO, StreamPollOption>.LoadFromDto(PollOptionInternalDTO dto, ICache cache)
        {
            Custom = dto.Custom;
            Id = dto.Id;
            Text = dto.Text;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        StreamPollOption IStateLoadableFrom<PollOptionResponseDataInternalDTO, StreamPollOption>.LoadFromDto(PollOptionResponseDataInternalDTO dto, ICache cache)
        {
            Custom = dto.Custom;
            Id = dto.Id;
            Text = dto.Text;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

