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
            => index switch
            {
                0 => Input.GetMouseButton(0),
                1 => Input.GetMouseButton(1),
                2 => Input.GetMouseButton(2),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}