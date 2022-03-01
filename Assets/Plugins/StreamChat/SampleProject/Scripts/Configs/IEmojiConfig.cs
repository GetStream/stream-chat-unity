using System.Collections.Generic;
using UnityEngine;

namespace StreamChat.SampleProject.Plugins.StreamChat.SampleProject.Scripts.Configs
{
    public interface IEmojiConfig
    {
        IReadOnlyDictionary<string, Sprite> Emojis { get; }
    }
}