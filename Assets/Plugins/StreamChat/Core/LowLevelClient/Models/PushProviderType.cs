using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public readonly struct PushProviderType : System.IEquatable<PushProviderType>,
        ILoadableFrom<PushProviderTypeInternalDTO, PushProviderType>,
        ISavableTo<PushProviderTypeInternalDTO>
    {
        public static readonly PushProviderType Firebase = new PushProviderType("firebase");
        public static readonly PushProviderType Apn = new PushProviderType("apn");
        public static readonly PushProviderType Huawei = new PushProviderType("huawei");
        public static readonly PushProviderType Xiaomi = new PushProviderType("xiaomi");

        public PushProviderType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(PushProviderType other) => _value == other._value;

        public override bool Equals(object obj) => obj is PushProviderType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(PushProviderType left, PushProviderType right) => left.Equals(right);

        public static bool operator !=(PushProviderType left, PushProviderType right) => !left.Equals(right);

        public static implicit operator PushProviderType(string value) => new PushProviderType(value);

        public static implicit operator string(PushProviderType type) => type._value;

        PushProviderType ILoadableFrom<PushProviderTypeInternalDTO, PushProviderType>.
            LoadFromDto(PushProviderTypeInternalDTO dto)
            => new PushProviderType(dto.Value);

        PushProviderTypeInternalDTO ISavableTo<PushProviderTypeInternalDTO>.SaveToDto()
            => new PushProviderTypeInternalDTO(_value);

        private readonly string _value;
    }
}