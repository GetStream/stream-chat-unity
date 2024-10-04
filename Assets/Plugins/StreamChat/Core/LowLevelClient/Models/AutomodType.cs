using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public readonly struct AutomodType : System.IEquatable<AutomodType>,
        ILoadableFrom<AutomodTypeInternalDTO, AutomodType>, ISavableTo<AutomodTypeInternalDTO>
    {
        public static readonly AutomodType Disabled = new AutomodType("disabled");
        public static readonly AutomodType Simple = new AutomodType("simple");
        public static readonly AutomodType AI = new AutomodType("AI");

        public AutomodType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(AutomodType other) => _value == other._value;

        public override bool Equals(object obj) => obj is AutomodType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(AutomodType left, AutomodType right) => left.Equals(right);

        public static bool operator !=(AutomodType left, AutomodType right) => !left.Equals(right);

        public static implicit operator AutomodType(string value) => new AutomodType(value);

        public static implicit operator string(AutomodType type) => type._value;

        AutomodType ILoadableFrom<AutomodTypeInternalDTO, AutomodType>.LoadFromDto(AutomodTypeInternalDTO dto)
            => new AutomodType(dto.Value);

        AutomodTypeInternalDTO ISavableTo<AutomodTypeInternalDTO>.SaveToDto() => new AutomodTypeInternalDTO(_value);

        private readonly string _value;
    }
}