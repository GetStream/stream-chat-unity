#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;
using StreamChat.Libs.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace StreamChat.Tests.LowLevelClient.Integration
{
    /// <summary>
    /// Base class for integration tests that operate on API and make real API requests
    /// </summary>
    internal abstract class BaseIntegrationTests
    {
        [OneTimeSetUp]
        public void Up()
        {
            Debug.Log("------------ Up");

            InitClientAndConnect();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Debug.Log("------------ TearDown");

            DeleteTempChannels();
            TryCleanupClient();
        }

        // StreamTodo: replace with admin ids fetched from loaded data set
        protected const string TestUserId = TestUtils.TestUserId;
        protected const string TestAdminId = TestUtils.TestAdminId;
        protected const string TestGuestId = TestUtils.TestGuestId;

        protected IStreamChatLowLevelClient LowLevelClient { get; private set; }
        protected OwnUser InitialLocalUser;

        /// <summary>
        /// Id of other user than currently logged one
        /// </summary>
        protected string OtherUserId { get; private set; }

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected IEnumerator CreateTempUniqueChannel(string channelType,
            ChannelGetOrCreateRequest channelGetOrCreateRequest, Action<ChannelState> onChannelReturned = null)
        {
            var createChannelTask = CreateTempUniqueChannelAsync(channelType, channelGetOrCreateRequest);

            yield return createChannelTask.RunAsIEnumerator(response =>
            {
                _tempChannelsToDelete.Add((response.Channel.Type, response.Channel.Id));
                onChannelReturned?.Invoke(response);
            });
        }

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected async Task<ChannelState> CreateTempUniqueChannelAsync(string channelType,
            ChannelGetOrCreateRequest channelGetOrCreateRequest)
        {
            var channelId = "random-channel-" + Guid.NewGuid();

            var channelState
                = await LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, channelGetOrCreateRequest);
            _tempChannelsToDelete.Add((channelState.Channel.Type, channelState.Channel.Id));
            return channelState;
        }

        protected IEnumerator InternalWaitForSeconds(float seconds)
        {
            if (seconds <= 0)
            {
                yield break;
            }

            var currentTime = EditorApplication.timeSinceStartup;

            while ((EditorApplication.timeSinceStartup - currentTime) < seconds)
            {
                yield return null;
            }
        }

        protected IEnumerator SendTestMessages(ChannelState channelState, int count,
            Action<(int Index, MessageResponse MessageResponse)> onMessageSent, string messagePrefix = "")
        {
            for (int i = 0; i < count; i++)
            {
                var sendMessageTask = LowLevelClient.MessageApi.SendNewMessageAsync(channelState.Channel.Type,
                    channelState.Channel.Id, new SendMessageRequest
                    {
                        Message = new MessageRequest
                        {
                            Text = messagePrefix + " #" + i
                        }
                    });

                yield return sendMessageTask.RunAsIEnumerator(messageResponse =>
                {
                    onMessageSent?.Invoke((i, messageResponse));
                });
            }
        }

        protected string GenerateRandomMessage(int minWords = 2, int maxWords = 10, int minWordLength = 2,
            int maxWordLength = 8)
        {
            var wordsCount = Random.Range(minWords, maxWords);
            while (wordsCount-- > 0)
            {
                var wordLength = Random.Range(minWordLength, maxWordLength);

                while (wordLength-- > 0)
                {
                    _sb.Append(Random.Range(0, WordChars.Length));
                }

                _sb.Append(" ");
            }

            var result = _sb.ToString();
            _sb.Clear();
            return result;
        }

        public static async Task<Texture2D> DownloadTextureAsync(string url)
        {
            using (var www = UnityWebRequestTexture.GetTexture(url))
            {
                var sendWebRequest = www.SendWebRequest();

                while (!sendWebRequest.isDone)
                {
                    await Task.Delay(1);
                }

                if (www.error != null)
                {
                    Debug.LogError($"{www.error}, URL:{www.url}");
                    return null;
                }

                return DownloadHandlerTexture.GetContent(www);
            }
        }

        public static async Task<byte[]> DownloadVideoAsync(string url)
        {
            using (var www = UnityWebRequest.Get(url))
            {
                var sendWebRequest = www.SendWebRequest();

                while (!sendWebRequest.isDone)
                {
                    await Task.Delay(1);
                }

                if (www.error != null)
                {
                    Debug.LogError($"{www.error}, URL:{www.url}");
                    return null;
                }

                return www.downloadHandler.data;
            }
        }

        private const string WordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly List<(string ChannelType, string ChannelId)> _tempChannelsToDelete =
            new List<(string ChannelType, string ChannelId)>();

        private readonly StringBuilder _sb = new StringBuilder();

        private void DeleteTempChannels()
        {
            if (_tempChannelsToDelete.Count == 0)
            {
                return;
            }
            
            var cids = new List<string>();

            foreach (var (channelType, channelId) in _tempChannelsToDelete)
            {
                cids.Add($"{channelType}:{channelId}");
            }

            _tempChannelsToDelete.Clear();

            LowLevelClient.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
            {
                Cids = cids,
                HardDelete = true
            }).LogIfFailed();
        }

        private void InitClientAndConnect(string forcedAdminId = null)
        {
            TestUtils.GetTestAuthCredentials(out var guestAuthCredentials, out var userAuthCredentials,
                out var adminAuthCredentials, out var otherUserAuthCredentials, forcedAdminId);

            OtherUserId = otherUserAuthCredentials.UserId;

            LowLevelClient = StreamChatLowLevelClient.CreateDefaultClient(adminAuthCredentials);
            LowLevelClient.Connected += OnClientConnected;
            LowLevelClient.Connect();
        }

        private void TryCleanupClient()
        {
            if (LowLevelClient == null)
            {
                return;
            }

            LowLevelClient.Connected -= OnClientConnected;
            LowLevelClient.Dispose();
            LowLevelClient = null;
        }

        private void OnClientConnected(OwnUser localUser)
        {
            InitialLocalUser = localUser;
        }

        protected IEnumerator ReconnectClient()
        {
            var userId = LowLevelClient.UserId;
            TryCleanupClient();
            InitClientAndConnect(userId);

            yield return LowLevelClient.WaitForClientToConnect();
        }
    }
}
#endif