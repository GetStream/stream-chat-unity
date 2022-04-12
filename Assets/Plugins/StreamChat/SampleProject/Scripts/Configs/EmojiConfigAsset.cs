using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core;
using UnityEngine;

namespace StreamChat.SampleProject.Configs
{
    [CreateAssetMenu(fileName = "EmojiConfig", menuName = StreamChatClient.MenuPrefix + "Demo/Create emoji config asset", order = 1)]
    public class EmojiConfigAsset : ScriptableObject, IEmojiConfig
    {
        public IEnumerable<(string Key, Sprite Sprite)> Emojis => _emojis.Select(_ => (_.Key, _.Sprite));

        [Header("Emoji's key to sprite mapping asset")]
        [SerializeField]
        private List<EmojiEntry> _emojis = new List<EmojiEntry>();

        [Serializable]
        public struct EmojiEntry
        {
            public string Key => _key;
            public Sprite Sprite => _sprite;

            [SerializeField]
            private string _key;

            [SerializeField]
            private Sprite _sprite;
        }
    }
}