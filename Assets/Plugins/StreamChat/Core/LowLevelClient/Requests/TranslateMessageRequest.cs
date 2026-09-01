using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public class TranslateMessageRequest : RequestObjectBase, ISavableTo<TranslateMessageRequestInternalDTO>
    {
        /// <summary>
        /// Target language as an ISO language code, e.g. "en"
        /// </summary>
        public string Language { get; set; }

        TranslateMessageRequestInternalDTO ISavableTo<TranslateMessageRequestInternalDTO>.SaveToDto() =>
            new TranslateMessageRequestInternalDTO
            {
                Language = Language,
                AdditionalProperties = AdditionalProperties,
            };
    }
}
