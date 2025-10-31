using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to create a poll option
    /// </summary>
    public partial class CreatePollOptionRequest : RequestObjectBase, ISavableTo<CreatePollOptionRequestInternalDTO>
    {
        /// <summary>
        /// Custom data for the poll option
        /// </summary>
        public Dictionary<string, object> Custom { get; set; }
        
        /// <summary>
        /// Option text
        /// </summary>
        public string Text { get; set; }

        CreatePollOptionRequestInternalDTO ISavableTo<CreatePollOptionRequestInternalDTO>.SaveToDto()
            => new CreatePollOptionRequestInternalDTO
            {
                Custom = Custom,
                Text = Text,
            };
    }
}

