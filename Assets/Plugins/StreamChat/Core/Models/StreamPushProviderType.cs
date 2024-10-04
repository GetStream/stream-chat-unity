using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Models
{
    public readonly struct StreamPushProviderType : System.IEquatable<StreamPushProviderType>,
        ILoadableFrom<PushProviderTypeInternalDTO, StreamPushProviderType>,
        ISavableTo<PushProviderTypeInternalDTO>
    {
        public static readonly StreamPushProviderType Firebase = new StreamPushProviderType("firebase");
        public static readonly StreamPushProviderType Apn = new StreamPushProviderType("apn");
        public static readonly StreamPushProviderType Huawei = new StreamPushProviderType("huawei");
        public static readonly StreamPushProviderType Xiaomi = new StreamPushProviderType("xiaomi");
        
        public StreamPushProviderType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(StreamPushProviderType other) => _value == other._value;

        public override bool Equals(object obj) => obj is StreamPushProviderType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(StreamPushProviderType left, StreamPushProviderType right) => left.Equals(right);

        public static bool operator !=(StreamPushProviderType left, StreamPushProviderType right)
            => !left.Equals(right);

        public static implicit operator StreamPushProviderType(string value) => new StreamPushProviderType(value);

        public static implicit operator string(StreamPushProviderType type) => type._value;

        StreamPushProviderType ILoadableFrom<PushProviderTypeInternalDTO, StreamPushProviderType>.
            LoadFromDto(PushProviderTypeInternalDTO dto)
            => new StreamPushProviderType(dto.Value);

        PushProviderTypeInternalDTO ISavableTo<PushProviderTypeInternalDTO>.SaveToDto()
            => new PushProviderTypeInternalDTO(_value);

        private readonly string _value;
    }
}