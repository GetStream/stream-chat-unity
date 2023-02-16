#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Auth;
using UnityEngine;

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
        protected async Task<IStreamChannel> CreateUniqueTempChannelAsync()
        {
            var channelId = "random-channel-11111-" + Guid.NewGuid();

            var channelState = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId);
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

        protected IEnumerator ConnectAndExecute(Func<Task> test)
        {
            yield return ConnectAndExecuteAsync(test).RunAsIEnumerator(statefulClient: Client);
        }

        //StreamTodo: figure out syntax to wrap call in using that will subscribe to observing an event if possible
        /// <summary>
        /// Use this if state update depends on receiving WS event that might come after the REST call was completed
        /// </summary>
        protected static async Task WaitWhileConditionTrue(Func<bool> condition, int maxIterations = 500)
        {
            for (int i = 0; i < maxIterations; i++)
            {
                if (!condition())
                {
                    return;
                }

                await Task.Delay(2);
            }
        }

        protected static async Task WaitWhileConditionFalse(Func<bool> condition, int maxIterations = 500)
        {
            for (int i = 0; i < maxIterations; i++)
            {
                if (condition())
                {
                    return;
                }

                await Task.Delay(2);
            }
        }
        
        /// <summary>
        /// Timeout will be doubled on each subsequent attempt. So max timeout = <see cref="initTimeoutMs"/> * 2^<see cref="maxAttempts"/>
        /// </summary>
        protected static async Task<T> Try<T>(Func<Task<T>> task, Predicate<T> successCondition, int maxAttempts = 20,
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
            await StreamTestClients.Instance.TryConnectStateClientAsync();
            const int maxAttempts = 3;
            int currentAttempt = 0;
            while (maxAttempts > currentAttempt)
            {
                currentAttempt++;
                try
                {
                    await test();
                    break;
                }
                catch (StreamApiException e)
                {
                    if (e.IsRateLimitExceededError())
                    {
                        await Task.Delay(1000);
                        continue;
                    }

                    throw;
                }
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