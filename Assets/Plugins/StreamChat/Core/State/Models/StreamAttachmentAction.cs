﻿using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamAttachmentAction : IStateLoadableFrom<ActionInternalDTO, StreamAttachmentAction>
    {
        public string Name { get; set; }

        public string Style { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        StreamAttachmentAction IStateLoadableFrom<ActionInternalDTO, StreamAttachmentAction>.LoadFromDto(ActionInternalDTO dto, ICache cache)
        {
            Name = dto.Name;
            Style = dto.Style;
            Text = dto.Text;
            Type = dto.Type;
            Value = dto.Value;

            return this;
        }
    }
}