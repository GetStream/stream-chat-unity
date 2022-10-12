﻿using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamImageData : IStateLoadableFrom<ImageDataInternalDTO, StreamImageData>
    {
        public string Frames { get; set; }

        public string Height { get; set; }

        public string Size { get; set; }

        public string Url { get; set; }

        public string Width { get; set; }

        StreamImageData IStateLoadableFrom<ImageDataInternalDTO, StreamImageData>.LoadFromDto(ImageDataInternalDTO dto, ICache cache)
        {
            Frames = dto.Frames;
            Height = dto.Height;
            Size = dto.Size;
            Url = dto.Url;
            Width = dto.Width;

            return this;
        }
    }
}