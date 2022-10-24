using System;
using UnityEngine;

namespace StreamChat.EditorTools
{
    internal readonly struct GUIColorScope : IDisposable
    {
        public GUIColorScope(Color color)
        {
            _prev = GUI.color;
            GUI.color = color;
        }

        public void Dispose() => GUI.color = _prev;

        private readonly Color _prev;
    }
}