using System;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using UnityEngine;

namespace StreamChat.SampleProjects.UIToolkit
{
    public class ChatWriter : IChatWriter
    {
        public ChatWriter(IStreamChatClient streamChatClient, IChatState chatState)
        {
            _streamChatClient = streamChatClient ?? throw new ArgumentNullException(nameof(streamChatClient));
            _chatState = chatState ?? throw new ArgumentNullException(nameof(chatState));
        }

        public async Task<bool> SendNewMessageAsync(string message)
        {
            var activeChannel = _chatState.ActiveChannel;

            if (activeChannel == null)
            {
                Debug.LogError("can't send message if there's no active channel");
                return false;
            }

            if (_sendNewMessageTask != null && !_sendNewMessageTask.IsCompleted)
            {
                Debug.LogWarning("Tried to send message, but previous task is still being processed");
                return false;
            }

            var request = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = message
                }
            };

            try
            {
                _sendNewMessageTask = _streamChatClient.MessageApi.SendNewMessageAsync(activeChannel.Channel.Type,
                    activeChannel.Channel.Id,
                    request);
                await _sendNewMessageTask;
                return true;
            }
            catch (StreamApiException e)
            {
                Debug.LogException(e.GetStreamApiUnwrappedException());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;
        }

        private readonly IStreamChatClient _streamChatClient;
        private readonly IChatState _chatState;

        private Task<MessageResponse> _sendNewMessageTask;
    }
}