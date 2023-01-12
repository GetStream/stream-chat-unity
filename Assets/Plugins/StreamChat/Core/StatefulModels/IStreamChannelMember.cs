using System;

namespace StreamChat.Core.StatefulModels
{
    /// <summary>
    /// <see cref="IStreamUser"/> that became a member of the <see cref="IStreamChannel"/>. Check <see cref="IStreamChannel.Members"/>
    /// </summary>
    public interface IStreamChannelMember : IStreamStatefulModel
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        DateTimeOffset? BanExpires { get; }

        /// <summary>
        /// Whether member is banned this channel or not
        /// </summary>
        bool Banned { get; }

        /// <summary>
        /// Role of the member in the channel
        /// </summary>
        string ChannelRole { get; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        DateTimeOffset? DeletedAt { get; }

        /// <summary>
        /// Date when invite was accepted
        /// </summary>
        DateTimeOffset? InviteAcceptedAt { get; }

        /// <summary>
        /// Date when invite was rejected
        /// </summary>
        DateTimeOffset? InviteRejectedAt { get; }

        /// <summary>
        /// Whether member was invited or not
        /// </summary>
        bool Invited { get; }

        /// <summary>
        /// Whether member is channel moderator or not
        /// </summary>
        bool IsModerator { get; }

        /// <summary>
        /// Whether member is shadow banned in this channel or not
        /// </summary>
        bool ShadowBanned { get; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        DateTimeOffset? UpdatedAt { get; }

        /// <summary>
        /// This member's <see cref="IStreamUser"/> reference
        /// </summary>
        IStreamUser User { get; }
    }
}