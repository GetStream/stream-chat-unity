using System;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.QueryBuilders.Filters.Channels
{
    /// <summary>
    /// Filter by <see cref="IStreamChannel.Invite"/>
    /// </summary>
    public sealed class ChannelFieldInvite : BaseFieldToFilter
    {
        public enum Status
        {
            Pending,
            Accepted,
            Rejected
        }
        
        public override string FieldName => "invite";
        
        /// <summary>
        /// Return only channels where Invite status is EQUAL to the provided value
        /// </summary>
        public FieldFilterRule EqualsTo(Status inviteStatus) => InternalEqualsTo(StatusToString(inviteStatus));

        private static string StatusToString(Status status)
        {
            switch (status)
            {
                case Status.Pending: return "pending";
                case Status.Accepted: return "accepted";
                case Status.Rejected: return "rejected";
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}