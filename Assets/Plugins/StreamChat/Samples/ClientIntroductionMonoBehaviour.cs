using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Filters.Users;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal class ChatManager : MonoBehaviour
    {
        private IStreamChatClient _chatClient;

        private void Start()
        {
            _chatClient = StreamChatClient.CreateDefaultClient();
        }

        public async Task ConnectAsync()
        {
            var localUserData = await _chatClient.ConnectUserAsync("api-key", "user-id", "userToken");
            Debug.Log($"User {localUserData.UserId} is connected!");
        }

        public async Task DisconnectAsync()
        {
            await _chatClient.DisconnectUserAsync();
            Debug.Log($"User disconnected");
        }

        public async Task CreateChannelAsync()
        {
            var channel = await _chatClient.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");
        }

        public async Task QueryChannelsAsync()
        {
            var filters = new List<IFieldFilterRule>()
            {
                ChannelFilter.Members.In("user-id-to-search")
            };

            var channels = await _chatClient.QueryChannelsAsync(filters);

            foreach (var channel in channels)
            {
                Debug.Log(channel.Id);
                Debug.Log(channel.Name);
                Debug.Log(channel.Messages.Count); // Messages
                Debug.Log(channel.Members.Count); // Members
            }
        }

        public async Task CreateChannelWithIdAsync()
        {
            var channel = await _chatClient.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");
        }

        public async Task CreateChannelForAGroupOfUsersAsync()
        {
            var filters = new IFieldFilterRule[]
            {
                UserFilter.Id.In("friend-user-id-1", "friend-user-id-2")
            };

            var friends = await _chatClient.QueryUsersAsync(filters);

            var groupToChat = new List<IStreamUser>();
            groupToChat.AddRange(friends); // Add friends
            groupToChat.Add(_chatClient.LocalUserData.User); // Add local user

// Create unique channel
            var channel = await _chatClient.GetOrCreateChannelWithMembersAsync(ChannelType.Messaging, groupToChat);
        }

        public async Task SendMessage()
        {
            var channel = await _chatClient.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Send simple message with text only
            var message = await channel.SendNewMessageAsync("Hello");
        }

        public async Task SendMessageAdvanced()
        {
            var channel = await _chatClient.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            IStreamUser someUser = null;

            // Send simple message with text only
            var message3 = await channel.SendNewMessageAsync("Hello");

            // Send simple message with text only
            var message2 = await channel.SendNewMessageAsync("Let's start a thread!");

            var message = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                MentionedUsers = new List<IStreamUser> { someUser }, // Mention a user
                ParentId = message2.Id, // Write in thread
                PinExpires = new DateTimeOffset(DateTime.Now).AddDays(7), // Pin for 7 days
                Pinned = true,
                QuotedMessage = message3,
                ShowInChannel = true,
                Text = "Hello",
                CustomData = new StreamCustomDataRequest
                {
                    { "my_lucky_numbers", new List<int> { 7, 13, 81 } }
                }
            });
        }

        public async Task ReadMessages()
        {
            var channel = await _chatClient.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            foreach (var message in channel.Messages)
            {
                Debug.Log(message.Text); // Message text
                Debug.Log(message.User); // Message author
                Debug.Log(message.ReactionCounts); // Message reactions
                Debug.Log(message.Attachments); // Message attachments
            }
        }

        public async Task SendReaction()
        {
            var channel = await _chatClient.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");
            var message = channel.Messages.First();

// Send simple reaction with a score of 1
            await message.SendReactionAsync("like");

// Send reaction with a custom score value
            await message.SendReactionAsync("clap", 10);

// Send reaction with a custom score value
            await message.SendReactionAsync("clap", 10);

// Send reaction and replace all previous reactions (if any) from this user
            await message.SendReactionAsync("love", enforceUnique: true);
        }
    }
}