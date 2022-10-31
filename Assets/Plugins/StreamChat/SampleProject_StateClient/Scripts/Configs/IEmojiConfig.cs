﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StreamChat.SampleProject_StateClient.Configs
{
    public interface IEmojiConfig
    {
        IEnumerable<Sprite> AllSprites { get; }
        IEnumerable<Sprite> ReactionSprites { get; }
        TMP_SpriteAsset TMPSpriteAsset { get; }
    }
}