using GetStreamIO.Core.DTO.Requests;
using Plugins.GetStreamIO.Core.Helpers;

namespace Plugins.GetStreamIO.Core.Requests.V2
{
    public partial class UpdateMessageRequest : RequestObjectBase, ISavableTo<UpdateMessageRequestDTO>
    {
        public MessageRequest Message { get; set; }

        /// <summary>
        /// Do not try to enrich the links within message
        /// </summary>
        public bool? SkipEnrichUrl { get; set; }

        public UpdateMessageRequestDTO SaveToDto() =>
            new UpdateMessageRequestDTO
            {
                Message = Message.TrySaveToDto(),
                SkipEnrichUrl = SkipEnrichUrl
            };
    }
}