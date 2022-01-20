namespace StreamChat.Core.Responses
{
    public abstract class ResponseObjectBase
    {
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new System.Collections.Generic.Dictionary<string, object>();
    }
}