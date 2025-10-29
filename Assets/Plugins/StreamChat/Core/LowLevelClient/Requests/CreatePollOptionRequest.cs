using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient.Models;

namespace StreamChat.Core.LowLevelClient.Requests
{
    /// <summary>
    /// Request to create a poll option
    /// </summary>
    public partial class CreatePollOptionRequest : RequestObjectBase, ISavableTo<CreatePollOptionRequestInternalDTO>
    {
        //public PollOptionInput PollOption { get; set; }
        
        public string Text { get; set; }

        CreatePollOptionRequestInternalDTO ISavableTo<CreatePollOptionRequestInternalDTO>.SaveToDto()
            => new CreatePollOptionRequestInternalDTO
            {
                Text = Text,
            };
    }
}

