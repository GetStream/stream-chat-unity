#if STREAM_TESTS_ENABLED
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace StreamChat.Tests.LowLevelClient.Integration
{
    /// <summary>
    /// Integration tests for Messages
    /// </summary>
    internal class MessagesApiIntegrationTests : BaseIntegrationTests
    {
        [UnityTest]
        public IEnumerator Send_message()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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

            var messageResponseTask = LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            yield return messageResponseTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Message.Text, "message content");
            });
        }

        [UnityTest]
        public IEnumerator Update_message()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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

            var messageResponseTask = LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            var lastMessageId = string.Empty;
            yield return messageResponseTask.RunAsIEnumerator(response =>
            {
                lastMessageId = response.Message.Id;

                Assert.AreEqual(response.Message.Text, "message content");
            });

            var updateMessageTask = LowLevelClient.MessageApi.UpdateMessageAsync(new UpdateMessageRequest
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

        //[UnityTest] //StreamTodo: debug, works when triggered manually but fails in GitHub Actions
        public IEnumerator Send_message_with_url()
        {
            yield return ConnectAndExecute(Send_message_with_url_Async);
        }
        
        private async Task Send_message_with_url_Async()
        {
            const string channelType = "messaging";

            var channelState = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            var channelId = channelState.Channel.Id;

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Check this bear out https://imgur.com/r/bears/4zmGbMN"
                }
            };

            await LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            //Message is not always immediately available due to data propagation
            channelState = await Try(() => LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    State = true,
                    Messages = new MessagePaginationParamsRequest
                    {
                        Limit = 30,
                        Offset = 0,
                    },
                }),channelState => channelState.Messages != null && channelState.Messages.Count > 0);

            Assert.IsNotNull(channelState.Messages);
            Assert.IsNotEmpty(channelState.Messages);
            Assert.AreEqual(channelState.Messages.Last().Attachments.First().AuthorName, "Imgur");
            Assert.AreEqual(channelState.Messages.Last().Attachments.First().TitleLink, "https://imgur.com/4zmGbMN");
        }

        [UnityTest]
        public IEnumerator When_sending_silent_message_expect_message_received_in_channel()
        {
            yield return ConnectAndExecute(When_sending_silent_message_expect_message_received_in_channel_Async);
        }
        
        private async Task When_sending_silent_message_expect_message_received_in_channel_Async()
        {
            const string channelType = "messaging";

            var channelState = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            var channelId = channelState.Channel.Id;

            var sendSilentMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Silent message",
                    Silent = true
                }
            };

            await LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendSilentMessageRequest);

            var channelState2 = await LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
                new ChannelGetOrCreateRequest
                {
                    State = true,
                });

            Assert.AreEqual(channelState2.Messages.Last().Silent, true);
        }

        [UnityTest]
        public IEnumerator UploadFile()
        {
            yield return LowLevelClient.WaitForClientToConnect();

            //var filename = "pexels-rulo-davila-5380467.mp4"; //32MB
            var filename = "SampleVideo_1280x720_1mb.mp4"; //1MB
            var videoFilePath = "Assets/Plugins/StreamChat/Tests/SampleFiles/" + filename;

            var videoClip = AssetDatabase.LoadAssetAtPath<VideoClip>(videoFilePath);
            Assert.NotNull(videoClip);

            var videoFileContent = File.ReadAllBytes(videoFilePath);
            Assert.NotNull(videoFileContent);
            Assert.IsNotEmpty(videoFileContent);

            var channelType = "messaging";

            ChannelState channelState = null;
            yield return CreateTempUniqueChannel(channelType, new ChannelGetOrCreateRequest(),
                state => channelState = state);
            var channelId = channelState.Channel.Id;

            var uploadFileTask =
                LowLevelClient.MessageApi.UploadFileAsync(channelType, channelId, videoFileContent, "sample-file-1");

            var fileUrl = "";
            yield return uploadFileTask.RunAsIEnumerator(response => { fileUrl = response.File; });

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Check out my cool video!",
                    Attachments = new List<AttachmentRequest>
                    {
                        new AttachmentRequest
                        {
                            AssetUrl = fileUrl,
                            Type = "video"
                        }
                    }
                }
            };

            var sendMessageTask = LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

            yield return sendMessageTask.RunAsIEnumerator(response =>
            {
                Assert.IsNotEmpty(response.Message.Attachments);
            });
        }

        [UnityTest]
        public IEnumerator UploadImageWithResize()
        {
            yield return LowLevelClient.WaitForClientToConnect();
            yield return UploadImageWithResizeAsync().RunAsIEnumerator();
        }

        private async Task UploadImageWithResizeAsync()
        {
            var filename = "pexels-markus-spiske-360591.jpg"; // 1920 x 1280 photo
            var imageFilePath = "Assets/Plugins/StreamChat/Tests/SampleFiles/" + filename;

            var imageAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(imageFilePath);
            Assert.NotNull(imageAsset);

            var imageFileContent = File.ReadAllBytes(imageFilePath);
            Assert.NotNull(imageFileContent);
            Assert.IsNotEmpty(imageFileContent);

            const string channelType = "messaging";

            var channelState = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            var channelId = channelState.Channel.Id;

            var uploadImageResponse = await LowLevelClient.MessageApi.UploadImageAsync(channelType, channelId, imageFileContent, filename);
            var fileUrl = uploadImageResponse.File;

            // Resize in scale mode to 500x500 pixels
            fileUrl += "&w=500&h=500&resize=scale";

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "My image description",
                    Attachments = new List<AttachmentRequest>
                    {
                        new AttachmentRequest
                        {
                            AssetUrl = fileUrl,
                            Type = "image"
                        }
                    }
                }
            };

            var sendMessageResponse = await LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);
            Assert.IsNotEmpty(sendMessageResponse.Message.Attachments);

            var imageUrl = sendMessageResponse.Message.Attachments[0].AssetUrl;
            var image = await DownloadTextureAsync(imageUrl);
            Assert.IsNotNull(image);

            Assert.AreEqual(image.width, 500);
            Assert.AreEqual(image.height, 500);
        }

        [UnityTest]
        public IEnumerator DeleteFile()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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
                LowLevelClient.MessageApi.UploadFileAsync(channelType, channelId, videoFileContent, "sample-file-1");

            var fileUrl = "";
            yield return uploadFileTask.RunAsIEnumerator(response =>
            {
                Assert.IsNotEmpty(response.File);
                fileUrl = response.File;
            });

            byte[] result = null;
            yield return DownloadVideoAsync(fileUrl).RunAsIEnumerator(response =>
            {
                result = response;
            });

            bool isEqual = videoFileContent.SequenceEqual(result);
            Debug.Log($"resultsEqual: {isEqual}, videoFileContent: {videoFileContent.Length}, result: {result.Length}");


            var _pathToFile = Path.Combine(Application.persistentDataPath, "video.mp4");
            Debug.Log("m" + _pathToFile);
            File.WriteAllBytes(_pathToFile, result);

            var deleteFileTask = LowLevelClient.MessageApi.DeleteFileAsync(channelType, channelId, fileUrl);

            yield return deleteFileTask.RunAsIEnumerator(response => { });
        }

        [UnityTest]
        public IEnumerator Add_reaction_score_to_existing_message()
        {
            yield return LowLevelClient.WaitForClientToConnect();

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

            var messageResponseTask = LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);

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

            var sendReactionTask = LowLevelClient.MessageApi.SendReactionAsync(messageId, sendReactionRequest);

            yield return sendReactionTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(response.Reaction.MessageId, messageId);
                Assert.AreEqual(response.Reaction.Score, 15);
            });

            var channelGetOrCreateTask = LowLevelClient.ChannelApi.GetOrCreateChannelAsync(channelType, channelId,
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
            yield return LowLevelClient.WaitForClientToConnect();
            yield return SearchMessagesAsync().RunAsIEnumerator(lowLevelClient: LowLevelClient);
        }

        public async Task SearchMessagesAsync()
        {
            var phrasesToInject = new string[]
            {
                "bengal Cat",
                "Bengal's logo",
                "A beNGAl cat is cool"
            };

            var injectedMessageIds = new List<string>();

            const string channelType = "messaging";
            var channelState = await CreateTempUniqueChannelAsync(channelType, new ChannelGetOrCreateRequest());
            var channelId = channelState.Channel.Id;

            await LowLevelClient.ChannelApi.UpdateChannelAsync(channelState.Channel.Type, channelState.Channel.Id,
                new UpdateChannelRequest
                {
                    AddMembers = new List<ChannelMemberRequest>
                    {
                        new ChannelMemberRequest
                        {
                            UserId = LowLevelClient.UserId
                        }
                    }
                });

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

                var messageResponse = await LowLevelClient.MessageApi.SendNewMessageAsync(channelType, channelId, sendMessageRequest);
                if (injectSearchPhrase)
                {
                    injectedMessageIds.Add(messageResponse.Message.Id);
                }
            }

            if (injectedMessageIds.Count != 3)
            {
                Debug.LogError("Failed to inject search phrase into 3 messages");
            }
            
            Assert.AreEqual(3, injectedMessageIds.Count);

            //Search

            // Due to data propagation the results may not be instant
            var attempt = 0;
            const int MaxAttemps = 4;

            SearchResponse response = null;
            while (attempt < MaxAttemps)
            {
                attempt++;
                
                response = await LowLevelClient.MessageApi.SearchMessagesAsync(new SearchRequest
                {
                    //Filter is required for search
                    FilterConditions = new Dictionary<string, object>
                    {
                        {
                            //Get channels that local user is a member of
                            "members", new Dictionary<string, object>
                            {
                                { "$in", new[] { LowLevelClient.UserId } }
                            }
                        }
                    },
                    
                    Sort = new List<SortParamRequest>
                    {
                        new SortParamRequest
                        {
                            Field = "created_at",
                            Direction = -1
                        }
                    },

                    //search phrase
                    Query = "bengal"
                });
            
                // Due to data propagation the results may not be instant
                if (response.Results.Count(r => r.Message.Channel.Cid == channelState.Channel.Cid) != 3)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        LowLevelClient.Update(0.1f);
                        await Task.Delay(1);
                    }
                    continue;
                }

                break;
            }
            
            Assert.NotNull(response);
            Assert.NotNull(response.Results);

            if (response.Results.Count(r => r.Message.Channel.Cid == channelState.Channel.Cid) > 3)
            {
                Debug.Log("Error: Search returned more results than expected. Listing found messages:");
                foreach (var message in response.Results)
                {
                    Debug.Log($"{message.Message.Channel.Cid} - {message.Message.Text}");
                }
            }

            Assert.AreEqual(3, response.Results.Count(r => r.Message.Channel.Cid == channelState.Channel.Cid));

            foreach (var injectedPhrase in phrasesToInject)
            {
                var result = response.Results.FirstOrDefault(_ => _.Message.Text.Contains(injectedPhrase));
                Assert.NotNull(result);
            }
        }
    }
}
#endif