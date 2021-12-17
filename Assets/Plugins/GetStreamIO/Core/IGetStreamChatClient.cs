using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugins.GetStreamIO.Core.Auth;
using Plugins.GetStreamIO.Core.Events.DTO;
using Plugins.GetStreamIO.Core.Models;

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

        ConnectionState ConnectionState { get; }

        void Update(float deltaTime);

        void Connect();

        bool IsLocalUser(User user);

        bool IsLocalUser(Member member);

        Task SendMessageAsync(Channel channel, string message);

        Task UpdateMessageAsync(Message message);

        Task DeleteMessage(Message message, bool hard);

        Task<IEnumerable<Channel>> GetChannelsAsync(QueryChannelsOptions options = null);

        Task Mute(User user);
    }
}