using System;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.Models;
using StreamChat.Core.Responses;

namespace StreamChat.Core.State
{
    internal static class StreamDtoExtensions
    {
        public static StreamImageCropType ToStreamImageCropType(this ImageCropType value)
        {
            switch (value)
            {
                case ImageCropType.Top: return StreamImageCropType.Top;
                case ImageCropType.Bottom: return StreamImageCropType.Bottom;
                case ImageCropType.Left: return StreamImageCropType.Left;
                case ImageCropType.Right: return StreamImageCropType.Right;
                case ImageCropType.Center: return StreamImageCropType.Center;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
        
        public static StreamImageResizeType ToStreamImageResizeType(this ImageResizeType value)
        {
            switch (value)
            {
                case ImageResizeType.Clip: return StreamImageResizeType.Clip;
                case ImageResizeType.Crop: return StreamImageResizeType.Crop;
                case ImageResizeType.Scale: return StreamImageResizeType.Scale;
                case ImageResizeType.Fill: return StreamImageResizeType.Fill;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
        
        public static StreamMessageType? TryConvertToStreamMessageType(this MessageType? messageType)
        {
            if (!messageType.HasValue)
            {
                return default;
            }

            switch (messageType)
            {
                case MessageType.Regular: return StreamMessageType.Regular;
                case MessageType.Ephemeral: return StreamMessageType.Ephemeral;
                case MessageType.Error: return StreamMessageType.Error;
                case MessageType.Reply: return StreamMessageType.Reply;
                case MessageType.System: return StreamMessageType.System;
                case MessageType.Deleted: return StreamMessageType.Deleted;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }
    }
}