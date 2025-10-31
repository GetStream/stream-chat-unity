using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to partially update a poll
    /// </summary>
    public partial class UpdatePollPartialRequest : RequestObjectBase, ISavableTo<UpdatePollPartialRequestInternalDTO>
    {
        public Dictionary<string, object> Set { get; set; }

        public List<string> Unset { get; set; }

        UpdatePollPartialRequestInternalDTO ISavableTo<UpdatePollPartialRequestInternalDTO>.SaveToDto()
            => new UpdatePollPartialRequestInternalDTO
            {
                Set = Set,
                Unset = Unset,
            };
    }
}

