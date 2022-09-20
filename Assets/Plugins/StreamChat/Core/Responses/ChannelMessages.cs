using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;
using StreamChat.Core.Models;

namespace StreamChat.Core.Responses
{
    public partial class ChannelMessages : ResponseObjectBase, ILoadableFrom<ChannelMessagesDTO, ChannelMessages>
    {
        public Channel Channel { get; set; }

        public System.Collections.Generic.List<Message> Messages { get; set; }

        ChannelMessages ILoadableFrom<ChannelMessagesDTO, ChannelMessages>.LoadFromDto(ChannelMessagesDTO dto)
        {
            Channel = Channel.TryLoadFromDto(dto.Channel);
            Messages = Messages.TryLoadFromDtoCollection(dto.Messages);

            return this;
        }
    }
}