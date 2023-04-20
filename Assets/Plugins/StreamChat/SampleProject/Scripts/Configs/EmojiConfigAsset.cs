using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Core.LowLevelClient;
using TMPro;
using UnityEngine;

namespace StreamChat.SampleProject.Configs
{
    [CreateAssetMenu(fileName = "EmojiConfig",
        menuName = StreamChatLowLevelClient.MenuPrefix + "Demo/Create emoji config asset", order = 1)]
    public class EmojiConfigAsset : ScriptableObject, IEmojiConfig
    {
        public TMP_SpriteAsset TMPSpriteAsset => _tmpSpriteAsset;

        public Texture2D EmojisAtlasTexture => _sourceSpritesAtlas;
        public IEnumerable<Sprite> ReactionSprites => _reactions;
        public IEnumerable<Sprite> AllSprites => _emojis;

        public void LoadEmojisSprites()
        {
            var sprites = Resources.LoadAll<Sprite>(_sourceSpritesAtlas.name);
            _emojis.AddRange(sprites);

            var reactionNames = _reactionSpritesNames.Split(',').Select(n => n.Trim()).ToList();
            if (reactionNames.Count == 0)
            {
                Debug.LogError("No reaction names in the configuration file");
                return;
            }

            foreach (var s in sprites)
            {
                if (reactionNames.Contains(s.name))
                {
                    _reactions.Add(s);
                }
            }
        }
        
        private readonly List<Sprite> _emojis = new List<Sprite>();
        private readonly List<Sprite> _reactions = new List<Sprite>();

        [SerializeField]
        private TMP_SpriteAsset _tmpSpriteAsset;

        [SerializeField]
        private Texture2D _sourceSpritesAtlas;

        [Header("Reaction names to be found in source atlas. Separated by comma")]
        [SerializeField]
        private string _reactionSpritesNames;

        private IChatViewContext _viewContext;
    }
}