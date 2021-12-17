namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// Message <see cref="Message"/>
    /// </summary>
    public enum MessageType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"regular")]
        Regular = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"ephemeral")]
        Ephemeral = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"error")]
        Error = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"reply")]
        Reply = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"system")]
        System = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"deleted")]
        Deleted = 5,
    }
}