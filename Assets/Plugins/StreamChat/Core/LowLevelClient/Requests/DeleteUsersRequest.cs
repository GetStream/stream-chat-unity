using System.Collections.Generic;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Requests
{
    public partial class DeleteUsersRequest : RequestObjectBase, ISavableTo<DeleteUsersRequestInternalDTO>
    {
        public DeleteUsersRequestConversations? Conversations { get; set; }

        public DeletionType? Messages { get; set; }

        public string NewChannelOwnerId { get; set; }

        public DeletionType? User { get; set; }

        public List<string> UserIds { get; set; } = new List<string>();

        DeleteUsersRequestInternalDTO ISavableTo<DeleteUsersRequestInternalDTO>.SaveToDto() =>
            new DeleteUsersRequestInternalDTO
            {
                Conversations = Conversations,
                Messages = Messages,
                NewChannelOwnerId = NewChannelOwnerId,
                User = User,
                UserIds = UserIds,
                AdditionalProperties = AdditionalProperties,
            };
    }
}