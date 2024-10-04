using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Responses
{
    public readonly struct StreamImageCropType : System.IEquatable<StreamImageCropType>,
        ILoadableFrom<ImageCropTypeInternalDTO, StreamImageCropType>, ISavableTo<ImageCropTypeInternalDTO>
    {
        public static readonly StreamImageCropType Top = new StreamImageCropType("top");
        public static readonly StreamImageCropType Bottom = new StreamImageCropType("bottom");
        public static readonly StreamImageCropType Left = new StreamImageCropType("left");
        public static readonly StreamImageCropType Right = new StreamImageCropType("right");
        public static readonly StreamImageCropType Center = new StreamImageCropType("center");

        public StreamImageCropType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(StreamImageCropType other) => _value == other._value;

        public override bool Equals(object obj) => obj is StreamImageCropType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(StreamImageCropType left, StreamImageCropType right) => left.Equals(right);

        public static bool operator !=(StreamImageCropType left, StreamImageCropType right) => !left.Equals(right);

        public static implicit operator StreamImageCropType(string value) => new StreamImageCropType(value);

        public static implicit operator string(StreamImageCropType type) => type._value;

        StreamImageCropType ILoadableFrom<ImageCropTypeInternalDTO, StreamImageCropType>.
            LoadFromDto(ImageCropTypeInternalDTO dto)
            => new StreamImageCropType(dto.Value);

        ImageCropTypeInternalDTO ISavableTo<ImageCropTypeInternalDTO>.SaveToDto()
            => new ImageCropTypeInternalDTO(_value);

        private readonly string _value;
    }
}