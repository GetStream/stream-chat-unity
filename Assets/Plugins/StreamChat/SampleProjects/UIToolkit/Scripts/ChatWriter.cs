using System;
using System.Threading.Tasks;
using StreamChat.Core;

namespace StreamChat.SampleProjects.UIToolkit
{
    public class ChatWriter : IChatWriter
    {
        public ChatWriter(IStreamChatClient streamChatClient, IChatState chatState)
        {
            _streamChatClient = streamChatClient ?? throw new ArgumentNullException(nameof(streamChatClient));
            _chatState = chatState ?? throw new ArgumentNullException(nameof(chatState));
        }

        public async Task SendNewMessageAsync(string message)
        {
            
        }
        
        private readonly IStreamChatClient _streamChatClient;
        private readonly IChatState _chatState;
    }
}