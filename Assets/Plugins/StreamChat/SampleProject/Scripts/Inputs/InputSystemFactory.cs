using System;

namespace StreamChat.SampleProject.Inputs
{
    /// <summary>
    /// <see cref="IInputSystem"/>
    /// </summary>
    public class InputSystemFactory
    {
        public IInputSystem CreateDefault()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            return new UnityInput();
#elif ENABLE_INPUT_SYSTEM
            return new UnityPreviewInputSystem();
#endif

            throw new Exception("No input system is enabled");
        }
    }
}