using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.Requests;
using StreamChat.Core.State;
using StreamChat.Core.State.TrackedObjects;
using UnityEngine;
using UnityEngine.TestTools;

namespace StreamChat.Tests.StatefulClient
{
    /// <summary>
    /// Tests operations executed on <see cref="StreamChannel"/>
    /// </summary>
    public class ChannelsTests : BaseStateIntegrationTests
    {
        [UnityTest]
        public IEnumerator When_GetOrCreateChannel_expect_no_errors()
            => ConnectAndExecute(When_GetOrCreateChannel_expect_no_errors_Async);

        private async Task When_GetOrCreateChannel_expect_no_errors_Async()
        {
            var channel = await CreateUniqueTempChannelAsync();
        }

        [UnityTest]
        public IEnumerator When_channel_send_message_expect_no_errors()
            => ConnectAndExecute(When_channel_send_message_expect_no_errors_Async);

        private async Task When_channel_send_message_expect_no_errors_Async()
        {
            //await ConnectUserAsync();
            var channel = await CreateUniqueTempChannelAsync();
            var channel2 = await CreateUniqueTempChannelAsync();

            var messageApi = ((StreamChatStateClient)StatefulClient).LowLevelClient.MessageApi;


            // var response1 = await messageApi.SendNewMessageAsync(channel.Type, channel.Id, new SendMessageRequest
            // {
            //     Message = new MessageRequest
            //     {
            //         Text = "Hello",
            //         ReactionScores = new Dictionary<string, int>
            //         {
            //             { "like", 5 },
            //             { "heart", 10 }
            //         },
            //     }
            // });
            //
            // var response2 = await messageApi.SendNewMessageAsync(channel.Type, channel.Id, new SendMessageRequest
            // {
            //     Message = new MessageRequest
            //     {
            //         Id = "MyAwesomeID-444",
            //         Text = "Hello",
            //         ReactionScores = new Dictionary<string, int>
            //         {
            //             { "like", 5 },
            //             { "heart", 10 }
            //         },
            //         Pinned = true,
            //         PinExpires = DateTime.Now.AddDays(1),
            //         ShowInChannel = false,
            //         Cid = channel2.Cid
            //
            //     }
            // });

            //StreamTodo: do not test low leve stuff here
            var response = await messageApi.SendNewMessageAsync(channel.Type, channel.Id, new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    //Id = "MyAwesomeID-222",
                    Text = "Hello",
                    // Pinned = true,
                    // PinExpires = DateTime.Now.AddDays(1),
                    // ShowInChannel = false,
                    // ParentId = response1.Message.Id,
                    // Cid = channel2.Cid,
                    // MentionedUsers = new List<string>
                    // {
                    //     TestGuestId, TestUserId
                    // },
                    // QuotedMessageId = response2.Message.Id

                }
            });

            var sentMessageId = response.Message.Id;

            StreamMessage receivedMessage = null;
            int waited = 0;
            for (int i = 0; i < 1000; i++) //Wait for WS event for max 1s
            {
                receivedMessage = channel.Messages.FirstOrDefault(_ => _.Id == sentMessageId);
                if (receivedMessage != null)
                {
                    waited = i;
                    break;
                }

                await Task.Delay(1);
            }

            Debug.Log("Waited: " + waited);

            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(receivedMessage.Text, "Hello");
        }
    }
}