using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.GetStreamIO.Unity.Scripts.Popups
{
    /// <summary>
    /// Option entry for <see cref="MessageOptionsPopup"/>
    /// </summary>
    public readonly struct OptionEntry
    {
        public readonly string Name;
        public readonly Action OnClick;
    }

    /// <summary>
    /// Context menu for message
    /// </summary>
    public class MessageOptionsPopup : BasePopup<MessageOptionsPopup.Args>
    {
        public readonly struct Args : IPopupArgs
        {
            public bool HideOnMouseExit { get; }
            public IReadOnlyList<OptionEntry> Options => _options;

            public Args(bool hideOnMouseExit, IEnumerable<OptionEntry> options)
            {
                HideOnMouseExit = hideOnMouseExit;
                _options = options.ToList();
            }

            private readonly List<OptionEntry> _options;
        }

        protected override void OnShow(Args args)
        {
            base.OnShow(args);

            ClearAllButtons();

            foreach (var option in args.Options)
            {
                var instance = Instantiate(_buttonPrefab, _buttonsContainer);
                _buttons.Add(instance);

                instance.onClick.AddListener(() => option.OnClick());
            }
        }

        private readonly IList<Button> _buttons = new List<Button>();

        [SerializeField]
        private Transform _buttonsContainer;

        [SerializeField]
        private Button _buttonPrefab;

        private void ClearAllButtons()
        {
            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

            _buttons.Clear();
        }
    }
}