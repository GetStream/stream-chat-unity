#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

        protected int MainThreadId { get; } = Thread.CurrentThread.ManagedThreadId;
        
        protected AuthCredentials AdminPrimaryCredentials => StreamTestClients.Instance.AdminPrimaryCredentials;
        protected AuthCredentials AdminSecondaryCredentials => StreamTestClients.Instance.AdminSecondaryCredentials;
        
        protected AuthCredentials UserPrimaryCredentials => StreamTestClients.Instance.UserPrimaryCredentials;
        protected AuthCredentials UserSecondaryCredentials => StreamTestClients.Instance.UserSecondaryCredentials;

        protected int GetCurrentThreadId() => Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected async Task<IStreamChannel> CreateUniqueTempChannelAsync(string name = null, bool watch = true, StreamChatClient overrideClient = null)
        {
            var channelId = "random-channel-11111-" + Guid.NewGuid();
            var client = overrideClient ?? Client;

            var channelState = await client.InternalGetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId, name, watch: watch);
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
            yield return ConnectAndExecuteAsync(test).RunAsIEnumerator();
        }

        protected Task<StreamChatClient> GetConnectedOtherClientAsync()
            => StreamTestClients.Instance.ConnectOtherStateClientAsync();

        //StreamTodo: figure out syntax to wrap call in using that will subscribe to observing an event if possible
        /// <summary>
        /// Use this if state update depends on receiving WS event that might come after the REST call was completed
        /// </summary>
        protected static async Task WaitWhileTrueAsync(Func<bool> condition, int maxSeconds = 1000)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!condition())
                {
                    return;
                }

                if (sw.Elapsed.TotalSeconds > maxSeconds)
                {
                    throw new TimeoutException("Timeout while waiting for condition");
                }

                var delay = (int)Math.Min(100 * 1000, Math.Pow(2, i + 9));
                await Task.Delay(delay);
            }

            throw new TimeoutException("Timeout while waiting for condition");
        }

        protected static async Task WaitWhileFalseAsync(Func<bool> condition, int maxSeconds = 1000)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (condition())
                {
                    return;
                }
                
                if (sw.Elapsed.TotalSeconds > maxSeconds)
                {
                    throw new TimeoutException("Timeout while waiting for condition");
                }

                var delay = (int)Math.Min(100 * 1000, Math.Pow(2, i + 9));
                await Task.Delay(delay);
            }
            
            throw new TimeoutException("Timeout while waiting for condition");
        }

        protected static async Task WaitWithTimeoutAsync(Task task, string exceptionMsg, int maxSeconds = 300)
        {
            if (await Task.WhenAny(task, Task.Delay(maxSeconds * 1000)) != task)
            {
                throw new TimeoutException(exceptionMsg);
            }
        }

        /// <summary>
        /// Timeout will be doubled on each subsequent attempt. So max timeout = <see cref="initTimeoutMs"/> * 2^<see cref="maxSeconds"/>
        /// </summary>
        protected static async Task<T> TryAsync<T>(Func<Task<T>> task, Predicate<T> successCondition,
            int maxSeconds = 1000)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < int.MaxValue; i++)
            {
                var response = await task();

                if (successCondition(response))
                {
                    return response;
                }
                
                if (sw.Elapsed.TotalSeconds > maxSeconds)
                {
                    throw new TimeoutException("Timeout while waiting for condition");
                }

                var delay = (int)Math.Min(100 * 1000, Math.Pow(2, i + 9));
                await Task.Delay(delay);
            }

            throw new TimeoutException("Timeout while waiting for condition");
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

            await Client.DeleteMultipleChannelsAsync(_tempChannels, isHardDelete: true);

            _tempChannels.Clear();
        }
    }
}
#endif