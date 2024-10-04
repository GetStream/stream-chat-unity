using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public readonly struct AutomodBehaviourType : System.IEquatable<AutomodBehaviourType>,
        ILoadableFrom<AutomodBehaviourTypeInternalDTO, AutomodBehaviourType>,
        ISavableTo<AutomodBehaviourTypeInternalDTO>
    {
        public static readonly AutomodBehaviourType Flag = new AutomodBehaviourType("flag");
        public static readonly AutomodBehaviourType Block = new AutomodBehaviourType("block");
        public static readonly AutomodBehaviourType ShadowBlock = new AutomodBehaviourType("shadow_block");

        public AutomodBehaviourType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(AutomodBehaviourType other) => _value == other._value;

        AutomodBehaviourType ILoadableFrom<AutomodBehaviourTypeInternalDTO, AutomodBehaviourType>.
            LoadFromDto(AutomodBehaviourTypeInternalDTO dto)
            => new AutomodBehaviourType(dto.Value);

        AutomodBehaviourTypeInternalDTO ISavableTo<AutomodBehaviourTypeInternalDTO>.SaveToDto()
            => new AutomodBehaviourTypeInternalDTO(_value);

        public override bool Equals(object obj) => obj is AutomodBehaviourType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(AutomodBehaviourType left, AutomodBehaviourType right) => left.Equals(right);

        public static bool operator !=(AutomodBehaviourType left, AutomodBehaviourType right) => !left.Equals(right);

        public static implicit operator AutomodBehaviourType(string value) => new AutomodBehaviourType(value);

        public static implicit operator string(AutomodBehaviourType type) => type._value;

        private readonly string _value;
    }
}