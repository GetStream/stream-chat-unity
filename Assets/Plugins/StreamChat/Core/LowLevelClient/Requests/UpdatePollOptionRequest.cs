using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to update a poll option
    /// </summary>
    public partial class UpdatePollOptionRequest : RequestObjectBase, ISavableTo<UpdatePollOptionRequestInternalDTO>
    {
        public Dictionary<string, object> Custom { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        UpdatePollOptionRequestInternalDTO ISavableTo<UpdatePollOptionRequestInternalDTO>.SaveToDto()
            => new UpdatePollOptionRequestInternalDTO
            {
                Custom = Custom,
                Id = Id,
                Text = Text,
            };
    }
}

