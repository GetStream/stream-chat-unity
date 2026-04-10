using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Request to create a poll option
    /// </summary>
    public class StreamPollOptionRequest : ISavableTo<PollOptionInputInternalDTO>, ISavableTo2<PollOptionRequestInternalDTO>
    {
        /// <summary>
        /// Custom data for the option
        /// </summary>
        public Dictionary<string, object> Custom { get; set; }

        /// <summary>
        /// Option text
        /// </summary>
        public string Text { get; set; }

        PollOptionRequestInternalDTO ISavableTo2<PollOptionRequestInternalDTO>.SaveToDto()
        {
            return new PollOptionRequestInternalDTO
            {
                Custom = Custom,
                Text = Text
            };
        }

        PollOptionInputInternalDTO ISavableTo<PollOptionInputInternalDTO>.SaveToDto()
        {
            return new PollOptionInputInternalDTO
            {
                Custom = Custom,
                Text = Text
            };
        }
    }
}