﻿using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Responses;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public class ReactionRemovalResponse : ResponseObjectBase, ILoadableFrom<ReactionRemovalResponseInternalDTO, ReactionRemovalResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        public Message Message { get; set; }

        public Reaction Reaction { get; set; }

        ReactionRemovalResponse ILoadableFrom<ReactionRemovalResponseInternalDTO, ReactionRemovalResponse>.LoadFromDto(ReactionRemovalResponseInternalDTO dto)
        {
            Duration = Duration;
            Message = Message.TryLoadFromDto<MessageInternalDTO, Message>(dto.Message);
            Reaction = Reaction.TryLoadFromDto(dto.Reaction);
            AdditionalProperties = AdditionalProperties;

            return this;
        }
    }
}