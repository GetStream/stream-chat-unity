using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Models;

namespace StreamChat.Core.State
{
    internal static class ICacheExt
    {
        public static StreamChannelMember CreateOrUpdate(this ICache cache, ChannelMemberInternalDTO dto)
            => cache.ChannelMembers.CreateOrUpdate<StreamChannelMember, ChannelMemberInternalDTO>(dto);
    }
}