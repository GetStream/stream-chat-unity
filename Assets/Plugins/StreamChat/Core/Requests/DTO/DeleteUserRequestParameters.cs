using StreamChat.Core.Web;

namespace StreamChat.Core.Requests.DTO
{
    public class DeleteUserRequestParameters : IAppendableQueryParameters
    {
        [Newtonsoft.Json.JsonProperty("mark_messages_deleted", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool MarkMessagesDeleted { get; set; }

        [Newtonsoft.Json.JsonProperty("hard_delete", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool HardDelete { get; set; }

        [Newtonsoft.Json.JsonProperty("delete_conversation_channels", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool DeleteConversationChannels { get; set; }

        void IAppendableQueryParameters.AppendQueryParameters(QueryParameters queryParameters)
        {
            queryParameters["mark_messages_deleted"] = MarkMessagesDeleted.ToString();
            queryParameters["hard_delete"] = HardDelete.ToString();
            queryParameters["delete_conversation_channels"] = DeleteConversationChannels.ToString();
        }
    }
}