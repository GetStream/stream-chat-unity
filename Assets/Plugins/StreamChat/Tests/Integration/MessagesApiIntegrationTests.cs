using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using StreamChat.Core.Requests;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.Video;

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

            //wait for message to propagate
            yield return UnityTestUtils.WaitForSeconds(0.5f);

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

        //[UnityTest]
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
            var channelId = "new-channel-id-1";

            var getOrCreateChannelTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, request);

            yield return getOrCreateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            var uploadFileTask = Client.MessageApi.UploadFileAsync(channelType, channelId, videoFileContent, "sample-file-1");

            var fileUrl = "";
            yield return uploadFileTask.RunAsIEnumerator(response =>
            {
                fileUrl = response.File;
            });

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

        //[UnityTest]
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
            var channelId = "new-channel-id-1";

            var getOrCreateChannelTask = Client.ChannelApi.GetOrCreateChannelAsync(channelType, channelId, request);

            yield return getOrCreateChannelTask.RunAsIEnumerator(response =>
            {
                Assert.AreEqual(channelId, response.Channel.Id);
                Assert.AreEqual(channelType, response.Channel.Type);
            });

            var uploadFileTask = Client.MessageApi.UploadFileAsync(channelType, channelId, videoFileContent, "sample-file-1");

            var fileUrl = "";
            yield return uploadFileTask.RunAsIEnumerator(response =>
            {
                Assert.IsNotEmpty(response.File);
                fileUrl = response.File;
            });

            var deleteFileTask = Client.MessageApi.DeleteFileAsync(channelType, channelId, fileUrl);

            yield return deleteFileTask.RunAsIEnumerator(response =>
            {

            });
        }
    }
}