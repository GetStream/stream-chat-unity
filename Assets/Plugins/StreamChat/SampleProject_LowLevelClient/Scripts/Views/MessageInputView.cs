using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Utils;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Message input view
    /// </summary>
    public class MessageInputView : BaseView
    {
        protected void Awake()
        {
            _sendButton.onClick.AddListener(OnSendButtonClicked);
            _attachmentButton.onClick.AddListener(OnAttachmentButtonClicked);
            _messageInput.onValueChanged.AddListener(OnMessageInputValueChanged);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            _typingMonitor.Update();

            if (InputSystem.WasEnteredPressedThisFrame)
            {
                OnSendButtonClicked();
            }
        }

        protected override void OnInited()
        {
            base.OnInited();

            _typingMonitor = new TypingMonitor(_messageInput, Client, State, isActive: () => _mode == Mode.Create);

            ViewContext.State.MessageEditRequested += OnMessageEditRequested;

            var sb = new StringBuilder();
            foreach (var sprite in ViewContext.AppConfig.Emojis.AllSprites)
            {
                if (sprite == null)
                {
                    continue;
                }

                sb.Append(":");
                sb.Append(sprite.name);
                sb.Append(":");

                var shortcode = sb.ToString();
                _emojisShortcodes.Add(shortcode);
                _emojiShortcodeToSpriteName[shortcode] = sprite.name;

                sb.Length = 0;
            }
        }

        protected override void OnDisposing()
        {
            _sendButton.onClick.RemoveListener(OnSendButtonClicked);
            _attachmentButton.onClick.RemoveListener(OnAttachmentButtonClicked);
            _messageInput.onValueChanged.RemoveListener(OnMessageInputValueChanged);

            ViewContext.State.MessageEditRequested -= OnMessageEditRequested;

            _typingMonitor?.Dispose();
            _typingMonitor = null;

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

        private readonly List<string> _emojisShortcodes = new List<string>();
        private readonly Dictionary<string, string> _emojiShortcodeToSpriteName = new Dictionary<string, string>();

        [SerializeField]
        private TMP_InputField _messageInput;

        [SerializeField]
        private Button _sendButton;

        [SerializeField]
        private Button _attachmentButton;

        private Mode _mode;
        private Message _currentEditMessage;
        private string _lastAttachmentUrl;
        private TypingMonitor _typingMonitor;

        private async void OnSendButtonClicked()
        {
            if (_messageInput.text.Length == 0)
            {
                return;
            }

            var channelState = ViewContext.State.ActiveChannel;

            if (channelState == null)
            {
                Debug.LogError("Failed to send message because the active channel is null");
                return;
            }

            var uploadedFileUrl = string.Empty;
            var uploadedFileType = "";

            if (!_lastAttachmentUrl.IsNullOrEmpty())
            {
                try
                {
                    uploadedFileType = Path.GetExtension(_lastAttachmentUrl);

                    Debug.Log("Start uploading attachment: " + _lastAttachmentUrl + ". This may take a while.");
                    _messageInput.text =
                        "Uploading attachment. This may take a while. Operation is asynchronous so you can continue using chat without being blocked.";

                    var fileContent = File.ReadAllBytes(_lastAttachmentUrl);

                    var uploadFileResponse = await Client.MessageApi.UploadFileAsync(channelState.Channel.Type,
                        channelState.Channel.Id, fileContent, "attachment-1");
                    uploadedFileUrl = uploadFileResponse.File;
                    _lastAttachmentUrl = string.Empty;

                    Debug.Log("Upload successful, CDN url: " + uploadedFileUrl);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
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

                    if (!uploadedFileUrl.IsNullOrEmpty())
                    {
                        sendMessageRequest.Message.Attachments = new List<AttachmentRequest>
                        {
                            new AttachmentRequest
                            {
                                AssetUrl = uploadedFileUrl,
                                Type = uploadedFileType
                            }
                        };
                    }

                    var sendMessageResponse = await Client.MessageApi.SendNewMessageAsync(channelState.Channel.Type,
                        channelState.Channel.Id,
                        sendMessageRequest);

                    if (!uploadedFileUrl.IsNullOrEmpty())
                    {
                        Debug.Log(sendMessageResponse.Message.Attachments.First().AssetUrl);
                    }

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

            _lastAttachmentUrl = string.Empty;
            _messageInput.text = "";

            _messageInput.Select();
            _messageInput.ActivateInputField();

            _currentEditMessage = null;
            _mode = Mode.Create;

            _typingMonitor.NotifyChannelStoppedTyping(State.ActiveChannel.Channel);
        }

        private void OnAttachmentButtonClicked()
        {
#if UNITY_EDITOR
            var filters = new string[2];
            filters[0] = "Video files";
            filters[1] = string.Join(", ", AllowedVideoFormats);

            _lastAttachmentUrl = EditorUtility.OpenFilePanelWithFilters("Pick attachment", "", filters);
            _messageInput.text = "Attachment ready: " + _lastAttachmentUrl;
#else
            Debug.LogError("Please implement file picker for this platform. File picker in demo only works in editor.");
#endif
        }

        private void OnMessageInputValueChanged(string value) => ReplaceEmojisWithSpriteMarkdown();

        private void ReplaceEmojisWithSpriteMarkdown()
        {
            var source = _messageInput.text;
            foreach (var shortcode in _emojisShortcodes)
            {
                var spriteName = _emojiShortcodeToSpriteName[shortcode];
                source = source.Replace(shortcode, $"<sprite name=\"{spriteName}\">");
            }

            if (source != _messageInput.text)
            {
                _messageInput.SetTextWithoutNotify(source);
                _messageInput.caretPosition = _messageInput.text.Length;
            }
        }
    }
}