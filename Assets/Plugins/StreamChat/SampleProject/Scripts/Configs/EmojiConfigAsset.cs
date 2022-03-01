using System;
using System.Collections.Generic;
using StreamChat.Core;
using StreamChat.Libs.Utils;
using UnityEngine;

namespace StreamChat.SampleProject.Plugins.StreamChat.SampleProject.Scripts.Configs
{
    [CreateAssetMenu(fileName = "EmojiConfig", menuName = StreamChatClient.MenuPrefix + "Demo/Create emoji config asset", order = 1)]
    public class EmojiConfigAsset : ScriptableObject, IEmojiConfig
    {
        public IReadOnlyDictionary<string, Sprite> Emojis => GetCached();

        private readonly Dictionary<string, Sprite> _emojisCache = new Dictionary<string, Sprite>();

        [Header("Emoji's key to sprite mapping asset")]
        [SerializeField]
        private List<EmojiEntry> _emojis = new List<EmojiEntry>();

        private bool _isCacheLoaded;

        [Serializable]
        private struct EmojiEntry
        {
            public string Key => _key;
            public Sprite Sprite => _sprite;

            [SerializeField]
            private string _key;

            [SerializeField]
            private Sprite _sprite;
        }

        private Dictionary<string, Sprite> GetCached()
        {
            if (_isCacheLoaded)
            {
                return _emojisCache;
            }

            for (var i = 0; i < _emojis.Count; i++)
            {
                var entry = _emojis[i];

                if (entry.Key.IsNullOrEmpty())
                {
                    Debug.LogWarning("Empty key for entry: " + i);
                    continue;
                }

                if (entry.Sprite == null)
                {
                    Debug.LogError("Empty sprite for key: " + entry.Key);
                    continue;
                }

                if (_emojisCache.ContainsKey(entry.Key))
                {
                    Debug.LogWarning("Duplicate key: " + entry.Key);
                    continue;
                }

                _emojisCache[entry.Key] = entry.Sprite;
            }

            return _emojisCache;
        }
    }
}