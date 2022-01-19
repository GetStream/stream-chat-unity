using System;
using System.Threading.Tasks;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Events;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Requests.V2;
using Plugins.GetStreamIO.Core.Responses;
using Action = System.Action;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// GetStream.io main client
    /// </summary>
    public interface IGetStreamChatClient : IAuthProvider, IConnectionProvider, IDisposable
    {
        event Action Connected;
        event Action<string> EventReceived;
        event Action<EventMessageNew> MessageReceived;
        event Action<EventMessageDeleted> MessageDeleted;
        event Action<EventMessageUpdated> MessageUpdated;

        ConnectionState ConnectionState { get; }

        void Update(float deltaTime);

        void Connect();

        bool IsLocalUser(User user);

        bool IsLocalUser(ChannelMember channelMember);

        Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId, SendMessageRequest sendMessageRequest);

        Task<ChannelsResponse> QueryChannelsAsync(QueryChannelsRequest queryChannelsRequest);

        Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest updateMessageRequest);

        Task<MessageResponse> DeleteMessageAsync(string messageId, bool hard);

        Task<MuteUserResponse> MuteUserAsync(MuteUserRequest muteUserRequest);
    }
}