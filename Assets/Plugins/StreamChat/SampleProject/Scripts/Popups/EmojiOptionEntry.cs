using System;
using UnityEngine;

namespace StreamChat.SampleProject.Popups
{
    public readonly struct EmojiOptionEntry
    {
        public readonly string Key;
        public readonly Sprite Sprite;
        public readonly bool IsAdded;
        public readonly Action OnClick;

        public EmojiOptionEntry(string key, Sprite sprite, bool isAdded, Action onClick)
        {
            Key = key;
            Sprite = sprite;
            IsAdded = isAdded;
            OnClick = onClick;
        }
    }
}