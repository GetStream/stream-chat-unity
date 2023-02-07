#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.EditorTools;
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
            InitClient();
        }

        [OneTimeTearDown]
        public async void OneTimeTearDown()
        {
            Debug.Log("------------ TearDown");

            await DeleteTempChannelsAsync();

            await Client.DisconnectUserAsync();

            Client.Dispose();
            Client = null;
        }

        protected StreamChatClient Client { get; private set; }

        // StreamTodo: replace with admin ids fetched from loaded data set
        protected const string TestUserId = TestUtils.TestUserId;
        protected const string TestAdminId = TestUtils.TestAdminId;
        protected const string TestGuestId = TestUtils.TestGuestId;

        protected TestAuthDataSet TestAuthDataSet { get; private set; }

        protected IEnumerable<AuthCredentials> OtherAdminUsersCredentials
            => TestAuthDataSet.TestAdminData.Where(d => d.UserId != Client.LocalUserData.UserId);

        protected enum UserLevel
        {
            Admin,
            User,
            Guest
        }

        protected async Task ConnectUserAsync(UserLevel level = UserLevel.Admin)
        {
            if (Client.IsConnected)
            {
                return;
            }

            var userCredentials = GetUserAuthCredentials(level);
            var connectTask = Client.ConnectUserAsync(userCredentials);
            while (!connectTask.IsCompleted)
            {
#if STREAM_DEBUG_ENABLED
                Debug.Log($"Wait for {nameof(StatefulClient)} to connect");
#endif

                await Task.Delay(1);
            }

            Debug.Log($"Connection made: {Client.ConnectionState}, user ID: {Client.LocalUserData.User.Id}");
        }

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
        protected async Task<IStreamUser> CreateUniqueTempUserAsync(string name)
        {
            var userId = "random-user-22222-" + Guid.NewGuid() + "-" + name;

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
            if (!condition())
            {
                return;
            }

            for (int i = 0; i < maxIterations; i++)
            {
                if (condition())
                {
                    await Task.Delay(1);
                }
            }
        }

        protected static async Task WaitWhileConditionFalse(Func<bool> condition, int maxIterations = 500)
        {
            if (condition())
            {
                return;
            }

            for (int i = 0; i < maxIterations; i++)
            {
                if (!condition())
                {
                    await Task.Delay(1);
                }
            }
        }

        private readonly List<IStreamChannel> _tempChannels = new List<IStreamChannel>();
        
        private async Task ConnectAndExecuteAsync(Func<Task> test)
        {
            await ConnectUserAsync();
            const int maxAttempts = 3;
            int currentAttempt = 0;
            while (maxAttempts > currentAttempt)
            {
                currentAttempt++;
                try
                {
                    await test();
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

        private void InitClient(string forcedAdminId = null)
        {
            Client = (StreamChatClient)StreamChatClient.CreateDefaultClient(new StreamClientConfig
            {
                LogLevel = StreamLogLevel.Debug
            });
            TestAuthDataSet = TestUtils.GetTestAuthCredentials();
        }

        private static AuthCredentials GetUserAuthCredentials(UserLevel level)
        {
            TestUtils.GetTestAuthCredentials(out var guestAuthCredentials, out var userAuthCredentials,
                out var adminAuthCredentials, out var otherUserAuthCredentials);

            Assert.IsFalse(adminAuthCredentials.IsAnyEmpty());
            Assert.IsFalse(userAuthCredentials.IsAnyEmpty());
            Assert.IsFalse(guestAuthCredentials.IsAnyEmpty());

            switch (level)
            {
                case UserLevel.Admin: return adminAuthCredentials;
                case UserLevel.User: return userAuthCredentials;
                case UserLevel.Guest: return guestAuthCredentials;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
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