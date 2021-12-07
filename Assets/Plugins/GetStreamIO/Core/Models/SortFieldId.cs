namespace Plugins.GetStreamIO.Core.Models
{
    /// <summary>
    /// Sorting field id
    /// </summary>
    public enum SortFieldId
    {
        FrozenStatus,
        ChannelType,
        ChannelId,
        ChannelCid,
        Members,
        InviteStatus,
        IsCurrentUserJoined,
        IsCurrentUserMuted,
        //...
        LastMessageAt,
    }

    //Todo: https://getstream.io/chat/docs/other-rest/query_channels/?language=kotlin#channel-queryable-built-in-fields
}