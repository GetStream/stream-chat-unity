#if ENABLE_INPUT_SYSTEM
using System;
using UnityEngine.InputSystem;
#endif

namespace StreamChat.SampleProject.Inputs
{
    /// <summary>
    /// New input system https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html
    /// </summary>
#if ENABLE_INPUT_SYSTEM
    public class UnityPreviewInputSystem : IInputSystem
    {
        public Vector2 MousePosition => Mouse.current.position.ReadValue();

        public bool WasEnteredPressedThisFrame => Keyboard.current.enterKey.wasPressedThisFrame;

        public bool GetMouseButton(int index)
            => index switch
            {
                0 => Mouse.current.leftButton.wasPressedThisFrame,
                1 => Mouse.current.rightButton.wasPressedThisFrame,
                2 => Mouse.current.middleButton.wasPressedThisFrame,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
#endif
}