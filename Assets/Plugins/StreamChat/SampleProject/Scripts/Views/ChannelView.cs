using System;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.Models;
using StreamChat.Libs.Utils;
using StreamChat.SampleProject.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Single channel entry view
    /// </summary>
    public class ChannelView : MonoBehaviour
    {
        public event Action<ChannelState> Clicked;

        public void Init(ChannelState channel, IChatViewContext context)
        {
            _channelState = channel ?? throw new ArgumentNullException(nameof(channel));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _isDirectMessage = channel.IsDirectMessage;

            UpdateMessagePreview();
            UpdateImageAsync().LogIfFailed();
        }

        protected void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private const int PreviewMessageLenght = 30;

        private ChannelState _channelState;
        private bool _isDirectMessage;

        [SerializeField]
        private TMP_Text _headerText;

        [SerializeField]
        private TMP_Text _messagePreviewText;

        [SerializeField]
        private Image _avatar;

        [SerializeField]
        private TMP_Text _avatarAbbreviation;

        [SerializeField]
        private Button _button;

        private IChatViewContext _context;

        private void OnClicked() => Clicked?.Invoke(_channelState);

        private void UpdateMessagePreview()
        {
            var channelCreator = _channelState.Channel.CreatedBy;
            var channelCreatorName = channelCreator.Name.IsNullOrEmpty() ? channelCreator.Id : channelCreator.Name;

            var name = _isDirectMessage ? channelCreatorName : _channelState.Channel.Name;

            _headerText.text = name;
            _messagePreviewText.text = GetLastMessagePreview();

            var abbreviationSource = name.IsNullOrEmpty()
                ? channelCreatorName
                : name;

            //Todo: this breaks when instead of a regular character we have an emoji
            var abbreviation = abbreviationSource.Length > 0 && char.IsLetterOrDigit(abbreviationSource.First())
                ? abbreviationSource.Substring(0, 1).ToUpper()
                : string.Empty;

             _avatarAbbreviation.text = abbreviation;
        }

        private string GetLastMessagePreview()
        {
            var lastMessage = _channelState.Messages.LastOrDefault();

            if (lastMessage == null)
            {
                return string.Empty;
            }

            if (lastMessage.Text.Length <= PreviewMessageLenght)
            {
                return lastMessage.Text;
            }

            return lastMessage.Text.Substring(0, PreviewMessageLenght) + " ...";
        }

        private async Task UpdateImageAsync()
        {
            _avatar.gameObject.SetActive(false);

            if (!_isDirectMessage)
            {
                return;
            }

            var otherMember = _channelState.Members.FirstOrDefault(_ => !_context.Client.IsLocalUser(_));

            if (otherMember == null || otherMember.User.Image.IsNullOrEmpty())
            {
                return;
            }

            var sprite = await _context.ImageLoader.LoadImageAsync(otherMember.User.Image);

            if (sprite != null)
            {
                _avatar.gameObject.SetActive(true);
                _avatar.sprite = sprite;
                _avatarAbbreviation.text = string.Empty;
            }
        }
    }
}