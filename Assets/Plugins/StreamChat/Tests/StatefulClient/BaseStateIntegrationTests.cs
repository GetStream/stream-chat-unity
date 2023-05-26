#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Auth;
using Debug = UnityEngine.Debug;

namespace StreamChat.Tests.StatefulClient
{
    internal abstract class BaseStateIntegrationTests
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

        protected static StreamChatClient Client => StreamTestClients.Instance.StateClient;

        // StreamTodo: replace with admin ids fetched from loaded data set
        protected const string TestUserId = TestUtils.TestUserId;
        protected const string TestAdminId = TestUtils.TestAdminId;
        protected const string TestGuestId = TestUtils.TestGuestId;

        protected static string OtherUserId => StreamTestClients.Instance.OtherUserId;

        protected static IEnumerable<AuthCredentials> OtherAdminUsersCredentials
            => StreamTestClients.Instance.OtherUserCredentials;

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected async Task<IStreamChannel> CreateUniqueTempChannelAsync(string name = null)
        {
            var channelId = "random-channel-11111-" + Guid.NewGuid();

            var channelState = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId, name);
            _tempChannels.Add(channelState);
            return channelState;
        }

        /// <summary>
        /// Create temp user with random id
        /// </summary>
        protected async Task<IStreamUser> CreateUniqueTempUserAsync(string name, string prefix = "")
        {
            var userId = prefix + "random-user-22222-" + Guid.NewGuid() + "-" + name;

            var user = await Client.UpsertUsers(new StreamUserUpsertRequest[]
            {
                new StreamUserUpsertRequest
                {
                    Id = userId,
                    Name = name
                }
            });
            return user.First();
        }

        /// <summary>
        /// Use only if you've successfully deleted the channel
        /// </summary>
        protected void SkipThisTempChannelDeletionInTearDown(IStreamChannel channel)
        {
            _tempChannels.Remove(channel);
        }

        protected static IEnumerator ConnectAndExecute(Func<Task> test)
        {
            yield return ConnectAndExecuteAsync(test).RunAsIEnumerator(statefulClient: Client);
        }

        protected Task<IStreamChatClient> GetConnectedOtherClient()
            => StreamTestClients.Instance.ConnectOtherStateClientAsync();

        //StreamTodo: figure out syntax to wrap call in using that will subscribe to observing an event if possible
        /// <summary>
        /// Use this if state update depends on receiving WS event that might come after the REST call was completed
        /// </summary>
        protected static async Task WaitWhileTrueAsync(Func<bool> condition, int maxIterations = 500)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            for (int i = 0; i < maxIterations; i++)
            {
                if (!condition())
                {
                    return;
                }

                if (sw.Elapsed.Seconds > 60)
                {
                    return;
                }

                var delay = (int)Math.Max(1, Math.Min(400, Math.Pow(2, i)));
                await Task.Delay(delay);
            }
        }

        protected static async Task WaitWhileFalseAsync(Func<bool> condition, int maxIterations = 500)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            for (int i = 0; i < maxIterations; i++)
            {
                if (condition())
                {
                    return;
                }
                
                if (sw.Elapsed.Seconds > 60)
                {
                    return;
                }

                var delay = (int)Math.Max(1, Math.Min(400, Math.Pow(2, i)));
                await Task.Delay(delay);
            }
        }

        protected static async Task WaitWithTimeoutAsync(Task task, int maxSeconds, string exceptionMsg)
        {
            if (await Task.WhenAny(task, Task.Delay(maxSeconds * 1000)) != task)
            {
                throw new TimeoutException(exceptionMsg);
            }
        }

        /// <summary>
        /// Timeout will be doubled on each subsequent attempt. So max timeout = <see cref="initTimeoutMs"/> * 2^<see cref="maxAttempts"/>
        /// </summary>
        protected static async Task<T> TryAsync<T>(Func<Task<T>> task, Predicate<T> successCondition, int maxAttempts = 20,
            int initTimeoutMs = 150)
        {
            var response = default(T);

            var attemptsLeft = maxAttempts;
            while (attemptsLeft > 0)
            {
                response = await task();

                if (successCondition(response))
                {
                    return response;
                }

                var delay = initTimeoutMs * Math.Pow(2, (maxAttempts - attemptsLeft));
                await Task.Delay((int)delay);
                attemptsLeft--;
            }

            return response;
        }

        private readonly List<IStreamChannel> _tempChannels = new List<IStreamChannel>();

        private static async Task ConnectAndExecuteAsync(Func<Task> test)
        {
            await StreamTestClients.Instance.ConnectStateClientAsync();
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
                throw new AggregateException($"Failed all attempts. Last Exception: {exceptions.Last().Message} ", exceptions);
            }
        }

        private async Task DeleteTempChannelsAsync()
        {
            if (_tempChannels.Count == 0)
            {
                return;
            }

            try
            {
                await Client.DeleteMultipleChannelsAsync(_tempChannels, isHardDelete: true);
            }
            catch (StreamApiException streamApiException)
            {
                if (streamApiException.Code == StreamApiException.RateLimitErrorHttpStatusCode)
                {
                    await Task.Delay(500);
                }

                Debug.Log($"Try {nameof(DeleteTempChannelsAsync)} again due to exception:  " + streamApiException);

                await Client.DeleteMultipleChannelsAsync(_tempChannels, isHardDelete: true);
            }

            _tempChannels.Clear();
        }
    }
}
#endif