using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StreamChat.SampleProject.Views
{
    public class TMPInputFieldSteamHelper : MonoBehaviour
    {
        protected void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();

            if (_inputField == null)
            {
                Debug.LogWarning($"Failed to get component: {nameof(TMP_InputField)}", this);
                return;
            }

            _inputField.onSelect.AddListener(OnSelect);
        }

        protected void OnDestroy()
        {
            if (_inputField == null)
            {
                return;
            }

            _inputField.onSelect.RemoveListener(OnSelect);
        }

        [SerializeField]
        private string _textLabel;

        private TMP_InputField _inputField;

        private int _count;

        //StreamTodo: encountered a bug where the keyboard would re-appear after the message was sent. Debug input selection.
        private void OnSelect(string arg0)
        {
            //Debug.LogError("On Select - Show keyboard - " + _count++);
            SteamService.ShowKeyboard(_textLabel, OnTextProvided, _inputField.text);
        }

        private void OnTextProvided(string text)
        {
            //Debug.LogError("OnTextProvided START - focused:  " + _inputField.isFocused);
            if (!_inputField.isFocused)
            {
                return;
            }

            _inputField.text = text;
            EventSystem.current.SetSelectedGameObject(null);
            //Debug.LogError("OnTextProvided END - focused:  " + _inputField.isFocused);
        }
    }
}