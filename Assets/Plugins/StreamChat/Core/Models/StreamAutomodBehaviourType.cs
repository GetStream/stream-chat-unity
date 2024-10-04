using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Models
{
    public readonly struct StreamAutomodBehaviourType : System.IEquatable<StreamAutomodBehaviourType>,
        ILoadableFrom<AutomodBehaviourTypeInternalDTO, StreamAutomodBehaviourType>,
        ISavableTo<AutomodBehaviourTypeInternalDTO>
    {
        public static readonly StreamAutomodBehaviourType Flag = new StreamAutomodBehaviourType("flag");
        public static readonly StreamAutomodBehaviourType Block = new StreamAutomodBehaviourType("block");
        public static readonly StreamAutomodBehaviourType ShadowBlock = new StreamAutomodBehaviourType("shadow_block");

        public StreamAutomodBehaviourType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(StreamAutomodBehaviourType other) => _value == other._value;

        public override bool Equals(object obj) => obj is StreamAutomodBehaviourType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(StreamAutomodBehaviourType left, StreamAutomodBehaviourType right)
            => left.Equals(right);

        public static bool operator !=(StreamAutomodBehaviourType left, StreamAutomodBehaviourType right)
            => !left.Equals(right);

        public static implicit operator StreamAutomodBehaviourType(string value)
            => new StreamAutomodBehaviourType(value);

        public static implicit operator string(StreamAutomodBehaviourType type) => type._value;

        StreamAutomodBehaviourType ILoadableFrom<AutomodBehaviourTypeInternalDTO, StreamAutomodBehaviourType>.
            LoadFromDto(AutomodBehaviourTypeInternalDTO dto)
            => new StreamAutomodBehaviourType(dto.Value);

        AutomodBehaviourTypeInternalDTO ISavableTo<AutomodBehaviourTypeInternalDTO>.SaveToDto()
            => new AutomodBehaviourTypeInternalDTO(_value);

        private readonly string _value;
    }
}