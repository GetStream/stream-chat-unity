using System;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
//This requires StreamChat.Unity assembly definition to have a reference to UnityEngine.InputSystem package
using UnityEngine.InputSystem;
#endif

namespace StreamChat.Unity.Scripts
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
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey("return"))
#elif ENABLE_INPUT_SYSTEM
            //This requires StreamChat.Unity assembly definition to have a reference to UnityEngine.InputSystem package
            if (Keyboard.current.enterKey.wasPressedThisFrame)
#endif
            {
#if ENABLE_LEGACY_INPUT_MANAGER || ENABLE_INPUT_SYSTEM
                OnSendButtonClicked();
#endif
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

                    var sendMessageRequest = new SendMessageRequest
                    {
                        Message = new MessageRequest
                        {
                            Text = _messageInput.text
                        }
                    };
                    var channel = ViewContext.State.ActiveChannel;

                    Client.MessageApi.SendNewMessageAsync(channel.Channel.Type, channel.Channel.Id, sendMessageRequest)
                        .LogStreamExceptionIfFailed();
                    break;

                case Mode.Edit:

                    _currentEditMessage.Text = _messageInput.text;

                    var updateMessageRequest = new UpdateMessageRequest
                    {
                        Message = new MessageRequest
                        {
                            Id = _currentEditMessage.Id,
                            Text = _messageInput.text
                        }
                    };

                    Client.MessageApi.UpdateMessageAsync(updateMessageRequest).LogStreamExceptionIfFailed();
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