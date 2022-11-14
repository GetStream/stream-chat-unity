﻿#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Configs;
using StreamChat.Core.State;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Libs;
using StreamChat.Libs.Auth;
using StreamChat.Libs.ChatInstanceRunner;
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

        protected StreamChatStateClient StatefulClient { get; private set; }

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

        protected Task ConnectUserAsync(UserLevel level = UserLevel.Admin)
        {
            var userCredentials = GetUserAuthCredentials(level);
            return StatefulClient.ConnectUserAsync(userCredentials);
        }

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected async Task<IStreamChannel> CreateUniqueTempChannelAsync()
        {
            var channelId = "random-channel-" + Guid.NewGuid();

            var channelState = await StatefulClient.GetOrCreateChannelAsync(ChannelType.Messaging, channelId);
            _tempChannels.Add(channelState);
            return channelState;
        }

        /// <summary>
        /// Use only if you've successfully deleted the channel
        /// </summary>
        protected void SkipThisTempChannelDeletionInTearDown(IStreamChannel channel)
        {
            _tempChannels.Remove(channel);
        }

        private readonly List<IStreamChannel> _tempChannels = new List<IStreamChannel>();

        private void InitClient(string forcedAdminId = null)
        {
            StatefulClient = (StreamChatStateClient)StreamChatStateClient.CreateDefaultClient(new StreamClientConfig
            {
                LogLevel = StreamLogLevel.Debug
            });
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
            if (_tempChannels.Count == 0)
            {
                return;
            }

            var unityLogs = LibsFactory.CreateDefaultLogs();

            StatefulClient.DeleteMultipleChannelsAsync(_tempChannels, isHardDelete: true).LogIfFailed(unityLogs);
            _tempChannels.Clear();
        }

        protected IEnumerator ConnectAndExecute(Func<Task> test)
        {
            yield return ConnectUserAsync().RunAsIEnumerator();
            yield return test().RunAsIEnumerator(statefulClient: StatefulClient);
        }
    }
}
#endif