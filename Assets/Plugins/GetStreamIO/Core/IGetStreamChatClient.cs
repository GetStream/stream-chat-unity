using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Events.DTO;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Models.V2;
using Plugins.GetStreamIO.Core.Requests.V2;
using Plugins.GetStreamIO.Core.Responses;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// GetStream.io main client
    /// </summary>
    public interface IGetStreamChatClient : IAuthProvider, IConnectionProvider, IDisposable
    {
        event Action Connected;
        event Action<string> EventReceived;
        event Action<MessageNewEvent> MessageReceived;
        event Action<MessageDeletedEvent> MessageDeleted;
        event Action<MessageUpdated> MessageUpdated;

        ConnectionState ConnectionState { get; }

        void Update(float deltaTime);

        void Connect();

        bool IsLocalUser(User user);

        bool IsLocalUser(ChannelMember channelMember);

        Task UpdateMessageAsync(Message message);

        Task DeleteMessage(Message message, bool hard);

        Task Mute(User user);

        Task<MessageResponse> SendNewMessageAsync(string channelType, string channelId, SendMessageRequest sendMessageRequest);

        Task<ChannelsResponse> QueryChannelsAsync(QueryChannelsRequest queryChannelsRequest);
    }
}