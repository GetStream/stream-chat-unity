using System;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public enum StreamMessageType
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

    internal static class StreamMessageTypeExt
    {
        public static StreamMessageType? TryConvertToStreamMessageType(this MessageType? messageType)
        {
            if (!messageType.HasValue)
            {
                return default;
            }

            switch (messageType)
            {
                case MessageType.Regular: return StreamMessageType.Regular;
                case MessageType.Ephemeral: return StreamMessageType.Ephemeral;
                case MessageType.Error: return StreamMessageType.Error;
                case MessageType.Reply: return StreamMessageType.Reply;
                case MessageType.System: return StreamMessageType.System;
                case MessageType.Deleted: return StreamMessageType.Deleted;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }
    }
}