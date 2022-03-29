﻿using System.Collections;
using System.Linq;
using NUnit.Framework;
using StreamChat.Core.Requests;
using UnityEngine.TestTools;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Messages
    /// </summary>
    public class MessagesApiIntegrationTests : BaseIntegrationTests
    {
        //[UnityTest]
        public IEnumerator Send_message()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator();

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content"
                }
            };

            var messageResponseTask = Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            yield return messageResponseTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Message.Text, "message content");
            });
        }

        //[UnityTest]
        public IEnumerator Update_message()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator();

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content"
                }
            };

            var messageResponseTask = Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            var lastMessageId = string.Empty;
            yield return messageResponseTask.RunAsIEnumerator(response =>
            {
                lastMessageId = response.Message.Id;

                Assert.AreEqual(response.Message.Text, "message content");
            });

            var updateMessageTask = Client.MessageApi.UpdateMessageAsync(new UpdateMessageRequest
            {
                Message = new MessageRequest
                {
                    Id = lastMessageId,
                    Text = "updated message content"
                }
            });

            yield return updateMessageTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Message.Id, lastMessageId);
                Assert.AreEqual(response.Message.Text, "updated message content");
            });
        }

        //[UnityTest]
        public IEnumerator Send_message_with_url()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var deleteChannelTask = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            yield return deleteChannelTask.RunAsIEnumerator(onFaulted: exception =>
            {
                //ignore if deletion failed
            });

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator();

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Check this bear out https://imgur.com/r/bears/4zmGbMN"
                }
            };

            var messageResponseTask = Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            yield return messageResponseTask.RunAsIEnumerator(response => { });

            var createChannelTask2 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    State = true,
                    Messages = new MessagePaginationParamsRequest()
                    {
                        Limit = 30,
                        Offset = 0
                    }
                });

            yield return createChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Messages.Last().Attachments.First().AuthorName, "Imgur");
                Assert.AreEqual(response.Messages.Last().Attachments.First().TitleLink, "https://imgur.com/4zmGbMN");
            });
        }

        //[UnityTest]
        public IEnumerator Send_silent_message()
        {
            yield return Client.WaitForClientToConnect();

            var channelType = "messaging";
            var channelId = "new-channel-id-1";

            var deleteChannelTask = Client.ChannelApi.DeleteChannelAsync(channelType, channelId);

            yield return deleteChannelTask.RunAsIEnumerator(onFaulted: exception =>
            {
                //ignore if deletion failed
            });

            var createChannelTask =
                Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, new ChannelGetOrCreateRequest());

            yield return createChannelTask.RunAsIEnumerator();

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Regular message"
                }
            };

            var messageResponseTask = Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            yield return messageResponseTask.RunAsIEnumerator();

            var createChannelTask2 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    Watch = true,
                    State = true,
                });

            yield return createChannelTask2.RunAsIEnumerator();

            var sendSilentMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Silent message",
                    Silent = true
                }
            };

            var silentMessageResponseTask =
                Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendSilentMessageRequest);

            yield return silentMessageResponseTask.RunAsIEnumerator();

            var createChannelTask3 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    State = true,
                });

            yield return createChannelTask3.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Messages.Last().Silent, true);
            });
        }
    }
}