using System.Collections.Generic;
using System.Collections.ObjectModel;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class DeleteUsersRequest : RequestObjectBase, ISavableTo<DeleteUsersRequestDTO>
    {
        public DeleteUsersRequestConversations? Conversations { get; set; }

        public DeletionType? Messages { get; set; }

        public string NewChannelOwnerId { get; set; }

        public DeletionType? User { get; set; }

        public ICollection<string> UserIds { get; set; } = new Collection<string>();

        DeleteUsersRequestDTO ISavableTo<DeleteUsersRequestDTO>.SaveToDto() =>
            new DeleteUsersRequestDTO
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