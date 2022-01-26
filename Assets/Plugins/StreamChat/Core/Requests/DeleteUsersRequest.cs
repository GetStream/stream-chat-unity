using StreamChat.Core;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Core.Requests
{
    public partial class DeleteUsersRequest : RequestObjectBase, ISavableTo<DeleteUsersRequestDTO>
    {
        public DeleteUsersRequestConversations? Conversations { get; set; }

        public DeletionType? Messages { get; set; }

        public string NewChannelOwnerId { get; set; }

        public DeletionType? User { get; set; }

        public System.Collections.Generic.ICollection<string> UserIds { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        public DeleteUsersRequestDTO SaveToDto() =>
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