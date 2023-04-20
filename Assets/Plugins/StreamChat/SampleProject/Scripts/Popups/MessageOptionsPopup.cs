using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreamChat.SampleProject.Popups
{
    /// <summary>
    /// Context menu for message
    /// </summary>
    public class MessageOptionsPopup : BasePopup<MessageOptionsPopup.Args>
    {
        public readonly struct Args : IPopupArgs
        {
            public bool HideOnPointerExit { get; }
            public bool HideOnButtonClicked { get; }
            public IReadOnlyList<MenuOptionEntry> Options => _options;
            public IReadOnlyList<EmojiOptionEntry> Emojis => _emojis;

            public Args(bool hideOnPointerExit, bool hideOnButtonClicked, IEnumerable<MenuOptionEntry> options, IEnumerable<EmojiOptionEntry> emojis)
            {
                HideOnPointerExit = hideOnPointerExit;
                HideOnButtonClicked = hideOnButtonClicked;
                _options = options.ToList();
                _emojis = emojis.ToList();
            }

            private readonly List<MenuOptionEntry> _options;
            private readonly List<EmojiOptionEntry> _emojis;
        }

        public bool IsPointerOver { get; private set; }
        public RectTransform RectTransform { get; private set; }

        protected override void OnStart()
        {
            base.OnStart();
            RectTransform = GetComponent<RectTransform>();
        }

        protected override void OnShow(Args args)
        {
            base.OnShow(args);

            ClearAllButtons();

            foreach (var option in args.Options)
            {
                var instance = Instantiate(_buttonPrefab, _buttonsContainer);
                _buttons.Add(instance);

                instance.onClick.AddListener(() =>
                {
                    TryHide();
                    option.OnClick();
                });
                instance.GetComponentInChildren<TextMeshProUGUI>().text = option.Name;
            }

            foreach (var emoji in args.Emojis)
            {
                var instance = Instantiate(_emojiButtonPrefab, _emojiButtonsContainer);
                _buttons.Add(instance);

                var image = instance.GetComponent<Image>();
                image.sprite = emoji.Sprite;

                var color = image.color;
                color.a = emoji.IsAdded ? 0.2f : 1.0f;
                image.color = color;

                instance.onClick.AddListener(() =>
                {
                    TryHide();
                    emoji.OnClick();
                });
            }

            IsPointerOver = true;
        }

        private readonly IList<Button> _buttons = new List<Button>();

        [SerializeField]
        private Transform _buttonsContainer;

        [SerializeField]
        private Transform _emojiButtonsContainer;

        [SerializeField]
        private Button _buttonPrefab;

        [SerializeField]
        private Button _emojiButtonPrefab;

        private void ClearAllButtons()
        {
            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

            _buttons.Clear();
        }

        private void TryHide()
        {
            if (SelfArgs.HideOnButtonClicked)
            {
                Hide();
            }
        }
    }
}