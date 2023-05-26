#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.Exceptions;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;
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
        public void OneTimeUp()
        {
            Debug.Log("------------ Up");

            StreamTestClients.Instance.AddLock(this);
        }

        [OneTimeTearDown]
        public async void OneTimeTearDown()
        {
            Debug.Log("------------ TearDown");

            await DeleteTempChannelsAsync();
            await StreamTestClients.Instance.RemoveLockAsync(this);
        }

        protected static IStreamChatLowLevelClient LowLevelClient => StreamTestClients.Instance.LowLevelClient;

        /// <summary>
        /// Id of other user than currently logged one
        /// </summary>
        protected static string OtherUserId => StreamTestClients.Instance.OtherUserId;

        protected static OwnUser LowLevelClientOwnUser => StreamTestClients.Instance.LowLevelClientOwnUser;

        protected static IEnumerator ReconnectClient() => StreamTestClients.Instance.ReconnectLowLevelClientClient();

        protected static IEnumerator ConnectAndExecute(Func<Task> task)
        {
            yield return LowLevelClient.WaitForClientToConnect();
            yield return ExecuteAsync(task).RunAsIEnumerator();
        }

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected IEnumerator CreateTempUniqueChannel(string channelType,
            ChannelGetOrCreateRequest channelGetOrCreateRequest, Action<ChannelState> onChannelReturned = null)
        {
            var createChannelTask = CreateTempUniqueChannelAsync(channelType, channelGetOrCreateRequest);

            yield return createChannelTask.RunAsIEnumerator(response => { onChannelReturned?.Invoke(response); });
        }

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected async Task<ChannelState> CreateTempUniqueChannelAsync(string channelType,
            ChannelGetOrCreateRequest channelGetOrCreateRequest)
        {
            var channelId = "random-channel-" + Guid.NewGuid();

            var channelsResponse = await LowLevelClient.ChannelApi.QueryChannelsAsync(new QueryChannelsRequest()
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "id", new Dictionary<string, object>
                        {
                            { "$in", new[] { channelId } }
                        }
                    }
                }
            });

            if (channelsResponse.Channels != null && channelsResponse.Channels.Count > 0)
            {
                Debug.LogError($"Channel with id {channelId} already exists!");
            }

            var channelState = await Try(() => LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                channelGetOrCreateRequest), channelState => channelState != null);

            _tempChannelsCidsToDelete.Add(channelState.Channel.Cid);
            return channelState;
        }

        protected void RemoveTempChannelFromDeleteList(string channelCid)
            => _tempChannelsCidsToDelete.Remove(channelCid);

        /// <summary>
        /// Timeout will be doubled on each subsequent attempt. So max timeout = <see cref="initTimeoutMs"/> * 2^<see cref="maxAttempts"/>
        /// </summary>
        protected static async Task<T> Try<T>(Func<Task<T>> task, Predicate<T> successCondition, int maxAttempts = 20,
            int initTimeoutMs = 150)
        {
            var response = default(T);

            var attempt = 1;
            while (attempt <= maxAttempts)
            {
                attempt++;

                response = await task();

                if (successCondition(response))
                {
                    return response;
                }

                var delay = Math.Min(1000, initTimeoutMs + Math.Pow(2, attempt));
                await Task.Delay((int)delay);
            }

            return response;
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

        protected static async Task<Texture2D> DownloadTextureAsync(string url)
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

        protected static async Task<byte[]> DownloadVideoAsync(string url)
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

        private readonly List<string> _tempChannelsCidsToDelete =
            new List<string>();

        private readonly StringBuilder _sb = new StringBuilder();

        private async Task DeleteTempChannelsAsync()
        {
            if (_tempChannelsCidsToDelete.Count == 0)
            {
                return;
            }

            try
            {
                await LowLevelClient.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
                {
                    Cids = _tempChannelsCidsToDelete,
                    HardDelete = true
                });
            }
            catch (StreamApiException streamApiException)
            {
                if (streamApiException.Code == StreamApiException.RateLimitErrorHttpStatusCode)
                {
                    await Task.Delay(500);
                }

                await LowLevelClient.ChannelApi.DeleteChannelsAsync(new DeleteChannelsRequest
                {
                    Cids = _tempChannelsCidsToDelete,
                    HardDelete = true
                });
            }

            _tempChannelsCidsToDelete.Clear();
        }

        private static async Task ExecuteAsync(Func<Task> test)
        {
            const int maxAttempts = 7;
            var currentAttempt = 0;
            var completed = false;
            var exceptions = new List<Exception>();
            while (maxAttempts > currentAttempt)
            {
                currentAttempt++;
                try
                {
                    await test();
                    completed = true;
                    break;
                }
                catch (StreamApiException e)
                {
                    exceptions.Add(e);
                    if (e.IsRateLimitExceededError())
                    {
                        var seconds = (int)Math.Max(1, Math.Min(60, Math.Pow(2, currentAttempt)));
                        await Task.Delay(1000 * seconds);
                        continue;
                    }

                    throw;
                }
            }

            if (!completed)
            {
                throw new AggregateException($"Failed all attempts. Last Exception: {exceptions.Last().Message} ",
                    exceptions);
            }
        }
    }
}
#endif