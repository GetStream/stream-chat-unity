using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public readonly struct ImageCropType : System.IEquatable<ImageCropType>,
        ILoadableFrom<ImageCropTypeInternalDTO, ImageCropType>, ISavableTo<ImageCropTypeInternalDTO>
    {
        public static readonly ImageCropType Top = new ImageCropType("top");
        public static readonly ImageCropType Bottom = new ImageCropType("bottom");
        public static readonly ImageCropType Left = new ImageCropType("left");
        public static readonly ImageCropType Right = new ImageCropType("right");
        public static readonly ImageCropType Center = new ImageCropType("center");

        public ImageCropType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(ImageCropType other) => _value == other._value;

        public override bool Equals(object obj) => obj is ImageCropType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(ImageCropType left, ImageCropType right) => left.Equals(right);

        public static bool operator !=(ImageCropType left, ImageCropType right) => !left.Equals(right);

        public static implicit operator ImageCropType(string value) => new ImageCropType(value);

        public static implicit operator string(ImageCropType type) => type._value;

        ImageCropType ILoadableFrom<ImageCropTypeInternalDTO, ImageCropType>.LoadFromDto(ImageCropTypeInternalDTO dto)
            => new ImageCropType(dto.Value);

        ImageCropTypeInternalDTO ISavableTo<ImageCropTypeInternalDTO>.SaveToDto()
            => new ImageCropTypeInternalDTO(_value);

        private readonly string _value;
    }
}