﻿using StreamChat.Core;
using StreamChat.SampleProject.Plugins.StreamChat.SampleProject.Scripts.Configs;
using StreamChat.SampleProject.Popups;
using UnityEngine;

namespace StreamChat.SampleProject.Views
{
    /// <summary>
    /// Asset to keep <see cref="IViewFactory"/> config
    /// </summary>
    [CreateAssetMenu(fileName = "ViewFactoryConfig", menuName = StreamChatClient.MenuPrefix + "View/Create view factory config asset", order = 1)]
    public class ViewFactoryConfig : ScriptableObject, IViewFactoryConfig
    {
        public MessageOptionsPopup MessageOptionsPopupPrefab => _messageOptionsPopupPrefab;
        public IEmojiConfig EmojiConfig => _emojiConfig;

        [SerializeField]
        private MessageOptionsPopup _messageOptionsPopupPrefab;

        [SerializeField]
        private EmojiConfigAsset _emojiConfig;
    }
}