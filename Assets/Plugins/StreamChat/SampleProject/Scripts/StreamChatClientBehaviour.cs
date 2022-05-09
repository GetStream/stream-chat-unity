using System;
using System.Collections;
using System.Collections.Generic;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Libs.Auth;
using StreamChat.SampleProject.Inputs;
using StreamChat.SampleProject.Utils;
using StreamChat.SampleProject.Views;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace StreamChat.SampleProject
{
    /// <summary>
    /// Stream Chat Client MonoBehaviour
    /// </summary>
    public class StreamChatClientBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
            var inputSystemFactory = new InputSystemFactory();
            var defaultInputSystem = inputSystemFactory.CreateDefault();

            var viewFactory = new ViewFactory(_viewFactoryConfig, _popupsContainer);

            try
            {
                _client = StreamChatClient.CreateDefaultClient(_authCredentialsAsset.Credentials);
                _client.Connect();

                var viewContext =
                    new ChatViewContext(_client, new UnityImageWebLoader(), viewFactory, defaultInputSystem);

                viewFactory.Init(viewContext);
                _rootView.Init(viewContext);
            }
            catch (StreamMissingAuthCredentialsException)
            {
                var popup = viewFactory.CreateFullscreenPopup<ErrorPopup>();
                popup.SetData("Invalid Authorization Credentials",
                        $"Please provide valid authorization data into `{_authCredentialsAsset.name}` asset. " +
                        $"Register Stream Account and visit <b>Dashboard</b> to get your `API_KEY` and use <b>chat explorer</b> to create your first user. " +
                        $"You can then click here -> <link=\"TokenGenerator\"><u>Tokens & Authorization</u></link> to visit online auth token generator.",
                        new Dictionary<string, string>()
                        {
                            {
                                "TokenGenerator",
                                "https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#manually-generating-tokens"
                            }
                        });

#if UNITY_EDITOR

                StartCoroutine(BlinkProjectAsset(_authCredentialsAsset, popup));

#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected void Update() => _client?.Update(Time.deltaTime);

        protected void OnDestroy() => _client?.Dispose();

        private IStreamChatClient _client;

        [SerializeField]
        private RootView _rootView;

        [SerializeField]
        private AuthCredentialsAsset _authCredentialsAsset;

        [SerializeField]
        private ViewFactoryConfig _viewFactoryConfig;

        [SerializeField]
        private Transform _popupsContainer;

#if UNITY_EDITOR
        private IEnumerator BlinkProjectAsset(Object target, Object owner)
        {

            EditorUtility.FocusProjectWindow();

            while (owner != null)
            {
                EditorGUIUtility.PingObject(target);

                yield return new WaitForSeconds(1);
            }

        }
#endif

    }
}