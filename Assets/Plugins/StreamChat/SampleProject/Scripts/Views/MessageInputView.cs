using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
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
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (InputSystem.WasEnteredPressedThisFrame)
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

        [SerializeField]
        private Button _attachmentButton;

        private Mode _mode;
        private Message _currentEditMessage;
        private string _lastAttachmentUrl;

        private async void OnSendButtonClicked()
        {
            if (_messageInput.text.Length == 0)
            {
                return;
            }

            var channelState = ViewContext.State.ActiveChannel;

            var uploadedFileUrl = string.Empty;
            var uploadedFileType = "";

            if (!_lastAttachmentUrl.IsNullOrEmpty())
            {
                try
                {
                    uploadedFileType = Path.GetExtension(_lastAttachmentUrl);

                    Debug.Log("Start uploading attachment: " + _lastAttachmentUrl + ". This may take a while.");
                    _messageInput.text = "Uploading attachment. This may take a while. Operation is asynchronous so you can continue using chat without being blocked.";

                    var fileContent = await File.ReadAllBytesAsync(_lastAttachmentUrl);
                    var uploadFileResponse = await Client.MessageApi.UploadFileAsync(channelState.Channel.Type, channelState.Channel.Id, fileContent, "attachment-1");
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

                    var sendMessageResponse = await Client.MessageApi.SendNewMessageAsync(channelState.Channel.Type, channelState.Channel.Id,
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
    }
}