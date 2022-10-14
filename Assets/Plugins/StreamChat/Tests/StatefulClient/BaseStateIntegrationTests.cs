using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.State;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Utils;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    public abstract class BaseStateIntegrationTests
    {
        [SetUp]
        public void Up() => InitClient();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            DeleteTempChannels();
            yield return StatefulClient.DisconnectUserAsync().RunAsIEnumerator();
            StatefulClient.Dispose();
            StatefulClient = null;
        }

        protected IStreamChatStateClient StatefulClient { get; private set; }

        // StreamTodo: replace with admin ids fetched from loaded data set
        protected const string TestUserId = TestUtils.TestUserId;
        protected const string TestAdminId = TestUtils.TestAdminId;
        protected const string TestGuestId = TestUtils.TestGuestId;

        protected enum UserLevel
        {
            Admin,
            User,
            Guest
        }

        protected async Task ConnectUserAsync(UserLevel level = UserLevel.Admin)
        {
            var userCredentials = GetUserAuthCredentials(level);

            var connectTask = StatefulClient.ConnectUserAsync(userCredentials);
            while (!connectTask.IsCompleted)
            {
                StatefulClient.Update();
                await Task.Delay(1);
            }
        }

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected async Task<StreamChannel> CreateUniqueTempChannelAsync()
        {
            var channelId = "random-channel-" + Guid.NewGuid();

            var channelState = await StatefulClient.GetOrCreateChannelAsync(ChannelType.Messaging, channelId);
            _tempChannelCids.Add(channelState.Cid);
            return channelState;
        }

        private readonly List<string> _tempChannelCids = new List<string>();

        private void InitClient(string forcedAdminId = null)
        {
            StatefulClient = StreamChatStateClient.CreateDefaultClient();
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

        private void DeleteTempChannels()
        {
            if (_tempChannelCids.Count == 0)
            {
                return;
            }

            var unityLogs = LibsFactory.CreateDefaultLogs();

            StatefulClient.DeleteMultipleChannelsAsync(_tempChannelCids, isHardDelete: true).LogIfFailed(unityLogs);
            _tempChannelCids.Clear();
        }

        protected IEnumerator ConnectAndExecute(Func<Task> test)
        {
            yield return ConnectUserAsync().RunAsIEnumerator();
            yield return test().RunAsIEnumerator(statefulClient: StatefulClient);
        }
    }
}