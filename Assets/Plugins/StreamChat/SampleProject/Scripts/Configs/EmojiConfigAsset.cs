using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.LowLevelClient;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
#endif

namespace StreamChat.SampleProject.Configs
{
    [CreateAssetMenu(fileName = "EmojiConfig",
        menuName = StreamChatLowLevelClient.MenuPrefix + "Demo/Create emoji config asset", order = 1)]
    public class EmojiConfigAsset : ScriptableObject, IEmojiConfig
    {
        public IEnumerable<Sprite> AllSprites => _allSprites;
        public IEnumerable<Sprite> ReactionSprites => _reactionSprites;

        public TMP_SpriteAsset TMPSpriteAsset => _tmpSpriteAsset;

        [FormerlySerializedAs("_sprites")]
        [SerializeField]
        private Sprite[] _allSprites;

        [SerializeField]
        private Sprite[] _reactionSprites;

        [SerializeField]
        private TMP_SpriteAsset _tmpSpriteAsset;

#if UNITY_EDITOR
        [SerializeField]
        private Texture2D _sourceSpritesAtlas;

        [Header("Reaction names to be found in source atlas. Separated by comma")]
        [SerializeField]
        private string _reactionSpritesNames;

        protected void OnValidate()
        {
            if (_sourceSpritesAtlas == null)
            {
                return;
            }

            var path = AssetDatabase.GetAssetPath(_sourceSpritesAtlas);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>().ToDictionary(_ => _.name, _ => _);

            var found = new List<Sprite>();

            var keys = _reactionSpritesNames.Split(',').Select(_ => _.TrimStart().TrimEnd())
                .Where(_ => _ != string.Empty);

            foreach (var key in keys)
            {
                if (!sprites.ContainsKey(key))
                {
                    Debug.LogWarning("Failed to find sprite with key: " + key);
                    continue;
                }

                found.Add(sprites[key]);
            }

            if (found.Any())
            {
                _reactionSprites = found.ToArray();
            }
        }
#endif
    }
}