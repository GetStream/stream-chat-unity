using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Requests;
using UnityEngine;

namespace StreamChat.Samples.LowLevelClient.ClientDocs
{
    /// <summary>
    /// Code samples for Channels sections: https://getstream.io/chat/docs/unity/send_message/?language=unity
    /// </summary>
    public class MessagesApiCodeSamples
    {
        public async Task SendMessage()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content"
                }
            };

            var messageResponse = await _lowLevelClient.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task SendMessageInThread()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content",
                    ParentId = "parent-message-id", //Thread root
                    ShowInChannel = true //If true the message will be visible both in the thread and the main channel
                }
            };

            var messageResponse = await _lowLevelClient.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task QuoteOtherMessage()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content",
                    QuotedMessageId = "quoted-message-id"
                }
            };

            var messageResponse = await _lowLevelClient.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task UpdateMessage()
        {
            await _lowLevelClient.MessageApi.UpdateMessageAsync(new UpdateMessageRequest
            {
                Message = new MessageRequest
                {
                    Id = "message-id-1",
                    Text = "updated message content"
                }
            });
        }

        public async Task DeleteMessage()
        {
            var messageResponse = await _lowLevelClient.MessageApi.DeleteMessageAsync(messageId: "message-id-1", hard: false);
        }

        public async Task OpenGraphScraper()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "Check this bear out https://imgur.com/r/bears/4zmGbMN"
                }
            };

            var messageResponse = await _lowLevelClient.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task SendReaction()
        {
            var reactionResponse = await _lowLevelClient.MessageApi.SendReactionAsync(messageId: "message-id-1",
                new SendReactionRequest
                {
                    Reaction = new ReactionRequest
                    {
                        Type = "like",
                    }
                });
        }

        public async Task RemoveReaction()
        {
            var reactionRemovalResponse =
                await _lowLevelClient.MessageApi.DeleteReactionAsync(messageId: "message-id-1", reactionType: "like");
        }

        public async Task SendSilentMessage()
        {
            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message content",
                    Silent = true
                },
            };

            var messageResponse = await _lowLevelClient.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task UploadFile()
        {
            var filePath = "path/to/Cool Video.mp4";
            var fileName = "Cool Video.mp4";

            var fileContent = File.ReadAllBytes(filePath);

            //Upload file and get file url from CDN
            var uploadFileResponse = await _lowLevelClient.MessageApi.UploadFileAsync(channelType: "messaging",
                channelId: "channel-id-1", fileContent, fileName);

            //Url to file in CDN
            var remoteFileUrl = uploadFileResponse.File;

            var sendMessageRequest = new SendMessageRequest
            {
                Message = new MessageRequest
                {
                    Text = "message with file attachment",
                    Attachments = new List<AttachmentRequest>
                    {
                        new AttachmentRequest
                        {
                            //Pass this file url as an attachment
                            AssetUrl = remoteFileUrl,
                            Type = "video"
                        }
                    }
                },
            };

            var messageResponse = await _lowLevelClient.MessageApi.SendNewMessageAsync(channelType: "messaging",
                channelId: "channel-id-1", sendMessageRequest);
        }

        public async Task DeleteFile()
        {
            //File url that got returned by Client.MessageApi.UploadFileAsync endpoint
            var remoteFileUrl = "url/to/file/in/stream/cdn";

            var deleteFileResponse = await _lowLevelClient.MessageApi.DeleteFileAsync(channelType: "messaging",
                channelId: "channel-id-1", remoteFileUrl);
        }

        public async Task SearchMessagesWithPaginationUsingLimitAndOffset()
        {
            var searchResponse = await _lowLevelClient.MessageApi.SearchMessagesAsync(new SearchRequest
            {
                //Filter is required for search
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        //Get channels that local user is a member of
                        "members", new Dictionary<string, object>
                        {
                            { "$in", new[] { "John" } }
                        }
                    }
                },

                // Optional sorting, default is "by relevance"
                Sort = new List<SortParamRequest>
                {
                    new SortParamRequest
                    {
                        Field = "updated_at",
                        Direction = -1 // Descending
                    }
                },

                Limit = 100,
                Offset = 0,

                //search phrase
                Query = "supercalifragilisticexpialidocious"
            });

            foreach (var searchResult in searchResponse.Results)
            {
                Debug.Log(searchResult.Message.Id); //Message ID
                Debug.Log(searchResult.Message.Text); //Message text
                Debug.Log(searchResult.Message.User); //Message author info
                Debug.Log(searchResult.Message.Channel); //Channel info
            }
        }

        public async Task SearchMessagesWithPaginationUsingNextKey()
        {
            // Get first page without setting the Next parameter
            var resultsPage1 = await _lowLevelClient.MessageApi.SearchMessagesAsync(new SearchRequest
            {
                //Filter is required for search
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        //Get channels that local user is a member of
                        "members", new Dictionary<string, object>
                        {
                            { "$in", new[] { "John" } }
                        }
                    }
                },
                Query = "supercalifragilisticexpialidocious", // Search phrase
                Limit = 30 // Per page results
            });

            // First page results
            foreach (var searchResult in resultsPage1.Results)
            {
                Debug.Log(searchResult.Message.Id); // Message ID
                Debug.Log(searchResult.Message.Text); // Message text
                Debug.Log(searchResult.Message.User); // Message author info
                Debug.Log(searchResult.Message.Channel); // Channel info
            }

            // Get second page of results by setting: Next = resultsPage1.Next
            var resultsPage2 = await _lowLevelClient.MessageApi.SearchMessagesAsync(new SearchRequest
            {
                // Filter is required for search
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        //Get channels that local user is a member of
                        "members", new Dictionary<string, object>
                        {
                            { "$in", new[] { "John" } }
                        }
                    }
                },

                Next = resultsPage1.Next, // Put Next key from the previous page request
                Query = "supercalifragilisticexpialidocious", // Search phrase
                Limit = 30 // Per page results
            });

            // Second page results
            foreach (var searchResult in resultsPage1.Results)
            {
                Debug.Log(searchResult.Message.Id); // Message ID
                Debug.Log(searchResult.Message.Text); // Message text
                Debug.Log(searchResult.Message.User); // Message author info
                Debug.Log(searchResult.Message.Channel); // Channel info
            }
        }

        private IStreamChatLowLevelClient _lowLevelClient;
    }
}