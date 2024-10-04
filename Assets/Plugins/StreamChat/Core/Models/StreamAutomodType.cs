using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Models
{
    public readonly struct StreamAutomodType : System.IEquatable<StreamAutomodType>,
        ILoadableFrom<AutomodTypeInternalDTO, StreamAutomodType>, ISavableTo<AutomodTypeInternalDTO>
    {
        public static readonly StreamAutomodType Disabled = new StreamAutomodType("disabled");
        public static readonly StreamAutomodType Simple = new StreamAutomodType("simple");
        public static readonly StreamAutomodType AI = new StreamAutomodType("AI");
        
        public StreamAutomodType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(StreamAutomodType other) => _value == other._value;

        public override bool Equals(object obj) => obj is StreamAutomodType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(StreamAutomodType left, StreamAutomodType right) => left.Equals(right);

        public static bool operator !=(StreamAutomodType left, StreamAutomodType right) => !left.Equals(right);

        public static implicit operator StreamAutomodType(string value) => new StreamAutomodType(value);

        public static implicit operator string(StreamAutomodType type) => type._value;

        StreamAutomodType ILoadableFrom<AutomodTypeInternalDTO, StreamAutomodType>.
            LoadFromDto(AutomodTypeInternalDTO dto) => new StreamAutomodType(dto.Value);

        AutomodTypeInternalDTO ISavableTo<AutomodTypeInternalDTO>.SaveToDto() => new AutomodTypeInternalDTO(_value);

        private readonly string _value;
    }
}