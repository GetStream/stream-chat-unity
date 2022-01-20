namespace StreamChat.Core.Models
{
    public abstract class ModelBase
    {
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new System.Collections.Generic.Dictionary<string, object>();
    }
}