using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public partial class UpdateMessageRequest : RequestObjectBase, ISavableTo<UpdateMessageRequestDTO>
    {
        public MessageRequest Message { get; set; }

        /// <summary>
        /// Do not try to enrich the links within message
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        UpdateMessageRequestDTO ISavableTo<UpdateMessageRequestDTO>.SaveToDto() =>
            new UpdateMessageRequestDTO
            {
                Message = Message.TrySaveToDto(),
                SkipEnrichUrl = SkipEnrichUrl
            };
    }
}