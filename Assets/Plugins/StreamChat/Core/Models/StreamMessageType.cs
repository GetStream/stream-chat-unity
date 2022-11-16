namespace StreamChat.Core.Models
{
    public enum StreamMessageType
    {
        Regular = 0,
        Ephemeral = 1,
        Error = 2,
        Reply = 3,
        System = 4,
        Deleted = 5,
    }
}