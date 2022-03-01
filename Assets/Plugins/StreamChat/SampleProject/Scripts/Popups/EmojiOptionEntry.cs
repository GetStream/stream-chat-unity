using System;
using UnityEngine;

namespace StreamChat.SampleProject.Plugins.StreamChat.SampleProject.Scripts.Popups
{
    public readonly struct EmojiOptionEntry
    {
        public readonly string Key;
        public readonly Sprite Sprite;
        public readonly Action OnClick;

        public EmojiOptionEntry(string key, Sprite sprite, Action onClick)
        {
            Key = key;
            Sprite = sprite;
            OnClick = onClick;
        }
    }
}