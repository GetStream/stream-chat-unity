using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Represents poll option input
    /// </summary>
    public partial class PollOptionInput : ISavableTo<PollOptionInputInternalDTO>
    {
        public Dictionary<string, object> Custom { get; set; }

        public string Text { get; set; }

        PollOptionInputInternalDTO ISavableTo<PollOptionInputInternalDTO>.SaveToDto()
            => new PollOptionInputInternalDTO
            {
                Custom = Custom,
                Text = Text,
            };
    }
}

