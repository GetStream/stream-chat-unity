using System;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Utils;
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
        public event Action<IStreamChannel> Clicked;

        public void Init(IStreamChannel channel, IChatViewContext context)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            
            _context.State.ActiveChanelChanged += OnActiveChanelChanged;
            OnActiveChanelChanged(_context.State.ActiveChannel);

            _isDirectMessage = channel.IsDirectMessage;

            UpdateMessagePreview();
            UpdateAvatarAsync().LogIfFailed();
        }

        protected void Awake()
        {
            _button.onClick.AddListener(OnClicked);
            _buttonImage = _button.GetComponent<Image>();
            _defaultBgSprite = _buttonImage.sprite;
            _defaultAvatarSprite = _avatar.sprite;
        }

        protected void OnDestroy()
        {
            if (_context?.State != null)
            {
                _context.State.ActiveChanelChanged -= OnActiveChanelChanged;
            }
        }

        private const int PreviewMessageLenght = 30;

        private IStreamChannel _channel;
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
        
        [SerializeField]
        private Sprite _channelActiveSprite;

        private IChatViewContext _context;
        private Image _buttonImage;
        private Sprite _defaultBgSprite;
        private Sprite _defaultAvatarSprite;

        private void OnClicked() => Clicked?.Invoke(_channel);

        private void UpdateMessagePreview()
        {
            var channelCreator = _channel.CreatedBy;
            var channelCreatorName = channelCreator.Name.IsNullOrEmpty() ? channelCreator.Id : channelCreator.Name;

            var name = _isDirectMessage ? channelCreatorName : _channel.Name;

            _headerText.text = name;
            _messagePreviewText.text = GetLastMessagePreview();

            var abbreviationSource = name.IsNullOrEmpty()
                ? channelCreatorName
                : name;

            //StreamTodo: this breaks when instead of a regular character we have an emoji
            var abbreviation = abbreviationSource.Length > 0 && char.IsLetterOrDigit(abbreviationSource.First())
                ? abbreviationSource.Substring(0, 1).ToUpper()
                : string.Empty;

             _avatarAbbreviation.text = abbreviation;
             UpdateAbbreviationVisibility();
        }
        
        private void UpdateAbbreviationVisibility() => _avatarAbbreviation.gameObject.SetActive(_avatar.sprite == _defaultAvatarSprite);

        private string GetLastMessagePreview()
        {
            var lastMessage = _channel.Messages.LastOrDefault();

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
        
        private void OnActiveChanelChanged(IStreamChannel channel)
        {
            var isThisChannelActive = channel == _channel;
            _buttonImage.sprite = isThisChannelActive ? _channelActiveSprite : _defaultBgSprite;
        }

        private bool TrySetChannelIcon()
        {
            _channel.CustomData.TryGet<string>("clan_symbol", out var symbol);
            _channel.CustomData.TryGet<string>("clan_color", out var color);

            if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(color))
            {
                return false;
            }

            var spriteName = $"ico_{symbol}_{color}";
            
            var sprites = Resources.LoadAll("ClanIcons", typeof(Sprite));
            var sprite = sprites.FirstOrDefault(s => s.name.ToLower() == spriteName);

            if (sprite == null)
            {
                return false;
            }

            _avatar.sprite = (Sprite)sprite;
            UpdateAbbreviationVisibility();
            return true;
        }

        private async Task UpdateAvatarAsync()
        {
            if (TrySetChannelIcon())
            {
                return;
            }

            _avatar.gameObject.SetActive(false);

            if (!_isDirectMessage)
            {
                return;
            }

            var otherMember = _channel.Members.FirstOrDefault(_ => _.User != _context.Client.LocalUserData.User);
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
                UpdateAbbreviationVisibility();
            }
        }
    }
}