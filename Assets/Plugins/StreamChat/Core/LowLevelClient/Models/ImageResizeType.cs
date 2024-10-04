using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public readonly struct ImageResizeType : System.IEquatable<ImageResizeType>,
        ILoadableFrom<ImageResizeTypeInternalDTO, ImageResizeType>,
        ISavableTo<ImageResizeTypeInternalDTO>
    {
        public static readonly ImageResizeType Clip = new ImageResizeType("clip");
        public static readonly ImageResizeType Crop = new ImageResizeType("crop");
        public static readonly ImageResizeType Scale = new ImageResizeType("scale");
        public static readonly ImageResizeType Fill = new ImageResizeType("fill");

        public ImageResizeType(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(ImageResizeType other) => _value == other._value;

        public override bool Equals(object obj) => obj is ImageResizeType other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(ImageResizeType left, ImageResizeType right) => left.Equals(right);

        public static bool operator !=(ImageResizeType left, ImageResizeType right) => !left.Equals(right);

        public static implicit operator ImageResizeType(string value) => new ImageResizeType(value);

        public static implicit operator string(ImageResizeType type) => type._value;

        ImageResizeType ILoadableFrom<ImageResizeTypeInternalDTO, ImageResizeType>.
            LoadFromDto(ImageResizeTypeInternalDTO dto)
            => new ImageResizeType(dto.Value);

        ImageResizeTypeInternalDTO ISavableTo<ImageResizeTypeInternalDTO>.SaveToDto()
            => new ImageResizeTypeInternalDTO(_value);

        private readonly string _value;
    }
}