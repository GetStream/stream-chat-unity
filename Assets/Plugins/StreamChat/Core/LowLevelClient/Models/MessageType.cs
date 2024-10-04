using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public readonly struct MessageType : System.IEquatable<MessageType>,
        ILoadableFrom<MessageTypeInternalDTO, MessageType>,
        ISavableTo<MessageTypeInternalDTO>
    {
        public static readonly MessageType Regular = new MessageType("regular");
        public static readonly MessageType Ephemeral = new MessageType("ephemeral");
        public static readonly MessageType Error = new MessageType("error");
        public static readonly MessageType Reply = new MessageType("reply");
        public static readonly MessageType System = new MessageType("system");
        public static readonly MessageType Deleted = new MessageType("deleted");

        public MessageType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(MessageType other) => _value == other._value;

        public override bool Equals(object obj) => obj is MessageType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(MessageType left, MessageType right) => left.Equals(right);

        public static bool operator !=(MessageType left, MessageType right) => !left.Equals(right);

        public static implicit operator MessageType(string value) => new MessageType(value);

        public static implicit operator string(MessageType type) => type._value;

        MessageType ILoadableFrom<MessageTypeInternalDTO, MessageType>.
            LoadFromDto(MessageTypeInternalDTO dto)
            => new MessageType(dto.Value);

        MessageTypeInternalDTO ISavableTo<MessageTypeInternalDTO>.SaveToDto() => new MessageTypeInternalDTO(_value);

        private readonly string _value;
    }
}