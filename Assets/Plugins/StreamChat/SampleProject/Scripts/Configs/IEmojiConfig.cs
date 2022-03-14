using System.Collections.Generic;
using UnityEngine;

namespace StreamChat.SampleProject.Plugins.StreamChat.SampleProject.Scripts.Configs
{
    public interface IEmojiConfig
    {
        IEnumerable<(string Key, Sprite Sprite)> Emojis { get; }
    }
}