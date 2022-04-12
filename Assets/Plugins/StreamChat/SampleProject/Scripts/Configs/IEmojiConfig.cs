using System.Collections.Generic;
using UnityEngine;

namespace StreamChat.SampleProject.Configs
{
    public interface IEmojiConfig
    {
        IEnumerable<(string Key, Sprite Sprite)> Emojis { get; }
    }
}