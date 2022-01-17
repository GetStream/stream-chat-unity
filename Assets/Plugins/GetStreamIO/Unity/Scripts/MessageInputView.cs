using System;
using Plugins.GetStreamIO.Core;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Requests.V2;
using Plugins.GetStreamIO.Libs.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts
{
    /// <summary>
    /// Message input view
    /// </summary>
    public class MessageInputView : BaseView
    {
        protected void Awake()
        {
            _sendButton.onClick.AddListener(OnSendButtonClicked);
        }

        protected void Update()
        {
            if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey("return"))
            {
                OnSendButtonClicked();
            }
        }

        protected override void OnInited()
        {
            base.OnInited();

            ViewContext.State.MessageEditRequested += OnMessageEditRequested;
        }

        protected override void OnDisposing()
        {
            ViewContext.State.MessageEditRequested -= OnMessageEditRequested;
            base.OnDisposing();
        }

        private void OnMessageEditRequested(Message message)
        {
            _currentEditMessage = message;
            _mode = Mode.Edit;
            _messageInput.text = message.Text;

            _messageInput.Select();
            _messageInput.ActivateInputField();
        }

        private enum Mode
        {
            Create,
            Edit,
        }

        [SerializeField]
        private TMP_InputField _messageInput;

        [SerializeField]
        private Button _sendButton;

        private Mode _mode;
        private Message _currentEditMessage;

        private void OnSendButtonClicked()
        {
            if (_messageInput.text.Length == 0)
            {
                return;
            }

            switch (_mode)
            {
                case Mode.Create:

                    var request = new SendMessageRequest
                    {
                        Message = new MessageRequest
                        {
                            Text = _messageInput.text
                        }
                    };
                    var channel = ViewContext.State.ActiveChannelDeprecated;

                    Client.SendNewMessageAsync(channel.Channel.Type, channel.Channel.Id, request)
                        .LogStreamExceptionIfFailed();
                    break;

                case Mode.Edit:

                    _currentEditMessage.Text = _messageInput.text;
                    Client.UpdateMessageAsync(_currentEditMessage).LogStreamExceptionIfFailed();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _messageInput.text = "";

            _messageInput.Select();
            _messageInput.ActivateInputField();

            _currentEditMessage = null;
            _mode = Mode.Create;
        }
    }
}