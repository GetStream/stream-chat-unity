using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Represents a poll option
    /// </summary>
    public partial class PollOption : ILoadableFrom<PollOptionInternalDTO, PollOption>, ILoadableFrom<PollOptionResponseDataInternalDTO, PollOption>
    {
        public Dictionary<string, object> Custom { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public Dictionary<string, object> AdditionalProperties { get; set; }

        PollOption ILoadableFrom<PollOptionInternalDTO, PollOption>.LoadFromDto(PollOptionInternalDTO dto)
        {
            Custom = dto.Custom;
            Id = dto.Id;
            Text = dto.Text;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }

        PollOption ILoadableFrom<PollOptionResponseDataInternalDTO, PollOption>.LoadFromDto(PollOptionResponseDataInternalDTO dto)
        {
            Custom = dto.Custom;
            Id = dto.Id;
            Text = dto.Text;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}

