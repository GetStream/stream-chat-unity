﻿#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Video;

namespace StreamChat.Tests.Integration
{
    /// <summary>
    /// Integration tests for Messages
    /// </summary>
    public class MessagesApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Send_message()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

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

        [UnityTest]
        public IEnumerator Update_message()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

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

        [UnityTest]
        public IEnumerator Send_message_with_url()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel("messaging", new ChannelGetOrCreateRequest(),
                state => channelState = state);

            var channelId = channelState.Channel.Id;

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Check this bear out https://imgur.com/r/bears/4zmGbMN"
                }
            };

            var messageResponseTask = Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            yield return messageResponseTask.RunAsIEnumerator(response => { });

            //Message is not always immediately available due to data propagation
            yield return InternalWaitForSeconds(0.2f);

            var createChannelTask2 = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    State = true,
                    Messages = new MessagePaginationParamsRequest()
                    {
                        Limit = 30,
                        Offset = 0,
                    },
                });

            yield return createChannelTask2.RunAsIEnumerator(response =>
            {
                Assert.IsNotNull(response.Messages);
                Assert.IsNotEmpty(response.Messages);
                Assert.AreEqual(response.Messages.Last().Attachments.First().AuthorName, "Imgur");
                Assert.AreEqual(response.Messages.Last().Attachments.First().TitleLink, "https://imgur.com/4zmGbMN");
            });
        }

        [UnityTest]
        public IEnumerator Send_silent_message()
        {
            yield return Client.WaitForClientToConnect();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

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

        [UnityTest]
        public IEnumerator UploadFile()
        {
            yield return Client.WaitForClientToConnect();

            //var filename = "pexels-rulo-davila-5380467.mp4"; //32MB
            var filename = "SampleVideo_1280x720_1mb.mp4"; //1MB
            var videoFilePath = "Assets/Plugins/StreamChat/Tests/SampleFiles/" + filename;

            var videoClip = AssetDatabase.LoadAssetAtPath<VideoClip>(videoFilePath);
            Assert.NotNull(videoClip);

            var videoFileContent = File.ReadAllBytes(videoFilePath);
            Assert.NotNull(videoFileContent);
            Assert.IsNotEmpty(videoFileContent);

            var request = new ChannelGetOrCreateRequest();

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var uploadFileTask =
                Client.MessageApi.UploadFileAsync(channelType, channelId, videoFileContent, "sample-file-1");

            var fileUrl = "";
            yield return uploadFileTask.RunAsIEnumerator(response => { fileUrl = response.File; });

            var sendMessageRequest = new SendMessageRequest()
            {
                Message = new MessageRequest
                {
                    Text = "Check out my cool video!",
                    Attachments = new List<AttachmentRequest>()
                    {
                        new AttachmentRequest
                        {
                            AssetUrl = fileUrl,
                            Type = "video"
                        }
                    }
                }
            };

            var sendMessageTask = Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            yield return sendMessageTask.RunAsIEnumerator(response =>
            {
                Assert.IsNotEmpty(response.Message.Attachments);
            });
        }

        [UnityTest]
        public IEnumerator DeleteFile()
        {
            yield return Client.WaitForClientToConnect();

            //var filename = "pexels-rulo-davila-5380467.mp4"; //32MB
            var filename = "SampleVideo_1280x720_1mb.mp4"; //1MB
            var videoFilePath = "Assets/Plugins/StreamChat/Tests/SampleFiles/" + filename;

            var videoClip = AssetDatabase.LoadAssetAtPath<VideoClip>(videoFilePath);
            Assert.NotNull(videoClip);

            var videoFileContent = File.ReadAllBytes(videoFilePath);
            Assert.NotNull(videoFileContent);
            Assert.IsNotEmpty(videoFileContent);

            var request = new ChannelGetOrCreateRequest();

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var uploadFileTask =
                Client.MessageApi.UploadFileAsync(channelType, channelId, videoFileContent, "sample-file-1");

            var fileUrl = "";
            yield return uploadFileTask.RunAsIEnumerator(response =>
            {
                Assert.IsNotEmpty(response.File);
                fileUrl = response.File;
            });

            var deleteFileTask = Client.MessageApi.DeleteFileAsync(channelType, channelId, fileUrl);

            yield return deleteFileTask.RunAsIEnumerator(response => { });
        }

        [UnityTest]
        public IEnumerator Add_reaction_score_to_existing_message()
        {
            yield return Client.WaitForClientToConnect();

            #region Send Message

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content"
                }
            };

            var messageResponseTask = Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            var messageId = string.Empty;
            yield return messageResponseTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Message.Text, "message content");
                messageId = response.Message.Id;
            });

            #endregion

            var sendReactionRequest = new SendReactionRequest
            {
                Reaction = new ReactionRequest
                {
                    Type = "like",
                    Score = 15
                }
            };

            var sendReactionTask = Client.MessageApi.SendReactionAsync(messageId, sendReactionRequest);

            yield return sendReactionTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Reaction.MessageId, messageId);
                Assert.AreEqual(response.Reaction.Score, 15);
            });

            var channelGetOrCreateTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest()
                {
                    State = true
                });

            yield return channelGetOrCreateTask.RunAsIEnumerator(response =>
            {
                var message = response.Messages.FirstOrDefault(_ => _.Id == messageId);
                Assert.IsNotNull(message);

                Assert.IsTrue(message.ReactionCounts.ContainsKey("like"));
                Assert.AreEqual(message.ReactionCounts["like"], 1);

                Assert.IsTrue(message.ReactionScores.ContainsKey("like"));
                Assert.AreEqual(message.ReactionScores["like"], 15);
            });
        }

        [UnityTest]
        public IEnumerator SearchMessages()
        {
            yield return Client.WaitForClientToConnect();

            var phrasesToInject = new string[]
            {
                "bengal Cat",
                "Bengal's logo",
                "A beNGAl cat is cool"
            };

            var injectedMessageIds = new List<string>();

            const string channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var joinTask = Client.ChannelApi.UpdateChannelAsync(channelState.Channel.Type, channelState.Channel.Id,
                new UpdateChannelRequest
                {
                    AddMembers = new List<ChannelMemberRequest>
                    {
                        new ChannelMemberRequest
                        {
                            UserId = Client.UserId
                        }
                    }
                });

            yield return joinTask.RunAsIEnumerator();

            //Generate 6 messages and inject search phrases to 3 of them

            var insertedPhrases = 0;
            for (int i = 0; i < 6; i++)
            {
                var injectSearchPhrase = (i % 2 == 0) && (insertedPhrases < phrasesToInject.Length);
                var text = GenerateRandomMessage();
                if (injectSearchPhrase)
                {
                    var insertIndex = Random.Range(0, text.Length);
                    var phrase = phrasesToInject[insertedPhrases];
                    text = text.Insert(insertIndex, $" {phrase} ");
                    insertedPhrases++;
                }

                var sendMessageRequest = new SendMessageRequest
                {
                    Message = new MessageRequest
                    {
                        Text = text
                    }
                };

                var messageResponseTask =
                    Client.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

                yield return messageResponseTask.RunAsIEnumerator(response =>
                {
                    if (injectSearchPhrase)
                    {
                        injectedMessageIds.Add(response.Message.Id);
                    }
                });
            }

            //Search

            var searchTask = Client.MessageApi.SearchMessagesAsync(new SearchRequest
            {
                //Filter is required for search
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        //Get channels that local user is a member of
                        "members", new Dictionary<string, object>
                        {
                            { "$in", new[] { Client.UserId } }
                        }
                    }
                },

                //search phrase
                Query = "bengal"
            });

            yield return searchTask.RunAsIEnumerator(response =>
            {
                Assert.NotNull(response.Results);
                Assert.AreEqual(3, response.Results.Count);

                foreach (var injectedPhrase in phrasesToInject)
                {
                    var result = response.Results.FirstOrDefault(_ => _.Message.Text.Contains(injectedPhrase));
                    Assert.NotNull(result);
                }
            });
        }
    }
}
#endif