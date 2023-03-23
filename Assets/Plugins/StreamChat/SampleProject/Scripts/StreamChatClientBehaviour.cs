#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.Exceptions;
using StreamChat.Libs.Auth;
using StreamChat.SampleProject.Configs;
using StreamChat.SampleProject.Inputs;
using StreamChat.SampleProject.Utils;
using StreamChat.SampleProject.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

            var viewFactory = new ViewFactory(_appConfig, _popupsContainer);

            TrySetEmojisSpriteAtlas();

            try
            {
                var config = new StreamClientConfig
                {
#if STREAM_DEBUG_ENABLED
                    LogLevel = StreamLogLevel.Debug
#endif
                };
                _client = StreamChatClient.CreateDefaultClient(config);
                _client.ConnectUserAsync(_authCredentialsAsset.Credentials);

                var viewContext = new ChatViewContext(_client, new UnityImageWebLoader(), viewFactory,
                    defaultInputSystem, _appConfig);

                viewFactory.Init(viewContext);
                _rootView.Init(viewContext);
            }
            catch (StreamMissingAuthCredentialsException e)
            {
                Debug.LogError(e.Message);
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

                _missingCredentials = true;

#if UNITY_EDITOR

                StartCoroutine(BlinkProjectAsset(_authCredentialsAsset, popup));

#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected void Update()
        {
            if (_client == null || _missingCredentials)
            {
                return;
            }

            var isClientConnectedOrConnecting = _client.ConnectionState == ConnectionState.Connected ||
                                                _client.ConnectionState == ConnectionState.Connecting;

            var isNetworkReachable =
                Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;

            if (!isClientConnectedOrConnecting && isNetworkReachable)
            {
                Debug.LogWarning("Client is not connected, but network is reachable. Force reconnect.");
                _client.ConnectUserAsync(_authCredentialsAsset.Credentials);
            }
        }

        private IStreamChatClient _client;
        private bool _missingCredentials;

        [SerializeField]
        private RootView _rootView;

        [SerializeField]
        private AuthCredentialsAsset _authCredentialsAsset;

        [FormerlySerializedAs("appConfig")]
        [SerializeField]
        private AppConfig _appConfig;

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

        private void TrySetEmojisSpriteAtlas()
        {
            var spriteAsset = _appConfig?.Emojis?.TMPSpriteAsset;
            if (spriteAsset != null)
            {
                if (TMP_Settings.defaultSpriteAsset == spriteAsset)
                {
                }
                else if (TMP_Settings.defaultSpriteAsset != null)
                {
                    var fallbackSpriteAssets = TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets;

                    if (!fallbackSpriteAssets.Contains(spriteAsset))
                    {
                        fallbackSpriteAssets.Add(spriteAsset);
                    }

                    Debug.LogWarning(
                        $"`{spriteAsset.name}` sprite asset was added as a fallback to the default `{TMP_Settings.defaultSpriteAsset}`");
                }
                else
                {
                    Debug.LogError(
                        $"TMP_Settings Default sprite is not set. Emojis sprite will not be properly replaced. " +
                        $"Please either set the `{spriteAsset.name}` as a default sprite asset or set any default asset so that `{spriteAsset.name}` gets appended as a fallback");
                }
            }
        }
    }
}