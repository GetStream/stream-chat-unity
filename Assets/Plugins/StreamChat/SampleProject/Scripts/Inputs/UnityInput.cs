using System;
using UnityEngine;

namespace StreamChat.SampleProject.Inputs
{
    /// <summary>
    /// Legacy input system https://docs.unity3d.com/ScriptReference/Input.html
    /// </summary>
    public class UnityInput : IInputSystem
    {
        public Vector2 MousePosition => Input.mousePosition;

        public bool WasEnteredPressedThisFrame => Input.GetKeyDown(KeyCode.Return);

        public bool GetMouseButton(int index)
        {
            switch (index)
            {
                case 0: return Input.GetMouseButton(0);
                case 1: return Input.GetMouseButton(1);
                case 2: return Input.GetMouseButton(2);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}