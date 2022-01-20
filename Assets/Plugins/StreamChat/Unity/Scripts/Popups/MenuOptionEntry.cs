using System;

namespace StreamChat.Unity.Scripts.Popups
{
    /// <summary>
    /// Option entry for <see cref="MessageOptionsPopup"/>
    /// </summary>
    public readonly struct MenuOptionEntry
    {
        public readonly string Name;
        public readonly Action OnClick;

        public MenuOptionEntry(string name, Action onClick)
        {
            Name = name;
            OnClick = onClick;
        }
    }
}