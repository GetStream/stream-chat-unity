using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StreamChat.Core.Helpers;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
#endif

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

            _typingMonitor?.Update();

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

        private void OnMessageEditRequested(IStreamMessage message)
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
        private IStreamMessage _currentEditMessage;
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
                    
                    
                    var uploadFileResponse = await State.ActiveChannel.UploadFileAsync(fileContent, "attachment-1");
                    uploadedFileUrl = uploadFileResponse.FileUrl;
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

                    var sendMessageRequest = new StreamSendMessageRequest
                    {
                        Text = _messageInput.text
                    };

                    if (!uploadedFileUrl.IsNullOrEmpty())
                    {
                        sendMessageRequest.Attachments = new List<StreamAttachmentRequest>
                        {
                            new StreamAttachmentRequest
                            {
                                AssetUrl = uploadedFileUrl,
                                Type = uploadedFileType
                            }
                        };
                    }

                    var sentMessage = await State.ActiveChannel.SendNewMessageAsync(sendMessageRequest);

                    if (!uploadedFileUrl.IsNullOrEmpty())
                    {
                        Debug.Log(sentMessage.Attachments.First().AssetUrl);
                    }

                    break;

                case Mode.Edit:

                    _currentEditMessage.UpdateAsync(new StreamUpdateMessageRequest
                    {
                        Text = _messageInput.text
                    }).LogExceptionsOnFailed();

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

            _typingMonitor.NotifyChannelStoppedTyping(State.ActiveChannel);
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