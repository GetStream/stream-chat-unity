using System;
using Steamworks;
using UnityEngine;

namespace StreamChat.SampleProject
{
    public class SteamService : MonoBehaviour
    {
        public static bool ShowKeyboard(string description, Action<string> oneTimeCallback, string currentText = "",
            bool multiline = false, uint charLimit = 10000)
        {
            _oneTimeCallback = oneTimeCallback;
            
            var lineMode = multiline
                ? EGamepadTextInputLineMode.k_EGamepadTextInputLineModeMultipleLines
                : EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine;

            return SteamUtils.ShowGamepadTextInput(
                EGamepadTextInputMode.k_EGamepadTextInputModeNormal,
                lineMode, description,
                charLimit, currentText);
        }

        protected void Start()
        {
            if (SteamManager.Initialized)
            {
                var name = SteamFriends.GetPersonaName();
                Debug.Log(name);
            }
            
            _keyboardDismissedCallback = Callback<GamepadTextInputDismissed_t>.Create(OnKeyboardDismissed);
        }

        private Callback<GamepadTextInputDismissed_t> _keyboardDismissedCallback;
        private static Action<string> _oneTimeCallback;

        private void OnKeyboardDismissed(GamepadTextInputDismissed_t data)
        {
            if (!data.m_bSubmitted)
            {
                return;
            }
            
            var length = SteamUtils.GetEnteredGamepadTextLength();
            //according to steam return should only ever happen if length is > MaxInputLength
            if (!SteamUtils.GetEnteredGamepadTextInput(out var enteredText, length))
            {
                return;
            }

            _oneTimeCallback(enteredText);
            _oneTimeCallback = null;
        }
    }
}