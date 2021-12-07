using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// Api global fields mapper
    /// </summary>
    public static class ApiMapper
    {
        public static FieldMapper<SortFieldId> SortFields { get; } = new FieldMapper<SortFieldId>(map =>
        {
            map.Add(SortFieldId.FrozenStatus, "frozen");
            map.Add(SortFieldId.ChannelType, "type");
            map.Add(SortFieldId.ChannelId, "id");
            map.Add(SortFieldId.ChannelCid, "cid");
            map.Add(SortFieldId.Members, "members");
            map.Add(SortFieldId.InviteStatus, "invite");
            map.Add(SortFieldId.IsCurrentUserJoined, "joined");
            map.Add(SortFieldId.IsCurrentUserMuted, "muted");
            map.Add(SortFieldId.LastMessageAt, "last_message_at");
        });
    }

    //Todo: figure out whether we can automatically check if: (1) mapped field names are correct; (2) have we missed any mappings?
}