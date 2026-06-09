using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.QueryBuilders.Filters;
using StreamChat.Core.QueryBuilders.Filters.Channels;
using StreamChat.Core.QueryBuilders.Filters.Messages;
using StreamChat.Core.QueryBuilders.Sort;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class MessagesCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_message/?language=unity
        /// </summary>
        public async Task Overview()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_message/?language=unity#complex-example
        /// </summary>
        public async Task ComplexExample()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

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

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_message/?language=unity#get-a-message
        /// </summary>
        public async Task GetMessageById()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_message/?language=unity#update-a-message
        /// </summary>
        public async Task UpdateAMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

// Edit message text and some custom data
            await message.UpdateAsync(new StreamUpdateMessageRequest
            {
                Text = "Hi everyone!",
                CustomData = new StreamCustomDataRequest
                {
                    { "tags", new[] { "Funny", "Unique" } }
                }
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_message/?language=unity#partial-update
        /// </summary>
        public async Task PartialUpdate()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_message/?language=unity#delete-a-message
        /// </summary>
        public async Task DeleteAMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

// Soft delete
            await message.SoftDeleteAsync();

// Hard delete
            await message.HardDeleteAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message_format/?language=unity#open-graph-scraper
        /// </summary>
        public async Task OpenGraphScrapper()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/file_uploads/?language=unity#how-to-upload-a-file-or-image
        /// </summary>
        public async Task UploadFileOrImage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

// Get file byte array however you want e.g. Addressables.LoadAsset, Resources.Load, etc.
            var sampleFile = File.ReadAllBytes("path/to/file");
            var fileUploadResponse = await channel.UploadFileAsync(sampleFile, "my-file-name");
            var fileWebUrl = fileUploadResponse.FileUrl;

// Get image byte array however you want e.g. Addressables.LoadAsset, Resources.Load, etc.
            var sampleImage = File.ReadAllBytes("path/to/file");
            var imageUploadResponse = await channel.UploadImageAsync(sampleFile, "my-image-name");
            var imageWebUrl = imageUploadResponse.FileUrl;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/file_uploads/?language=unity#deleting-files-and-images
        /// </summary>
        public async Task DeleteFileOrImage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.DeleteFileOrImageAsync("file-url");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/file_uploads/?language=unity#using-your-own-cdn
        /// </summary>
        public async Task UsingYourOwnCdn()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

//Implement your own CDN upload and obtain the file URL
            var fileUrl = "file-url-to-your-cdn";

            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "Message with file attachment",
                Attachments = new List<StreamAttachmentRequest>
                {
                    new StreamAttachmentRequest
                    {
                        AssetUrl = fileUrl,
                    }
                }
            });

            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_reaction/?language=unity
        /// </summary>
        public async Task ReactionOverview()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

// Send simple reaction with a score of 1
            await message.SendReactionAsync("like");

// Send reaction with a custom score value
            await message.SendReactionAsync("clap", 10);

// Send reaction with a custom score value
            await message.SendReactionAsync("clap", 10);

// Send reaction and replace all previous reactions (if any) from this user
            await message.SendReactionAsync("love", enforceUnique: true);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_reaction/?language=unity#removing-a-reaction
        /// </summary>
        public async Task RemoveReaction()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            await message.DeleteReactionAsync("like");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_reaction/?language=unity#paginating-reactions
        /// </summary>
        public async Task PaginateReactions()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            //StreamTodo: IMPLEMENT reactions paginating
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_reaction/?language=unity#cumulative-(clap)-reactions
        /// </summary>
        public async Task CumulativeReactions()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            await message.SendReactionAsync("clap", score: 3);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity
        /// </summary>
        public async Task ThreadsAndReplies()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Send simple message with text only
            var message3 = await channel.SendNewMessageAsync("Hello");

// Send simple message with text only
            var parentMessage = await channel.SendNewMessageAsync("Let's start a thread!");

            var messageInThread = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                ParentId = parentMessage.Id, // Write in thread
                ShowInChannel = false, // Optionally send to both thread and the main channel like in Slack
                Text = "Hello",
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#thread-pagination
        /// </summary>
        public async Task ThreadPagination()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Send a parent message and a few replies
            var parentMessage = await channel.SendNewMessageAsync("Let's start a thread!");
            for (int i = 0; i < 5; i++)
            {
                await channel.SendNewMessageAsync(new StreamSendMessageRequest
                {
                    ParentId = parentMessage.Id,
                    Text = $"Reply #{i}",
                });
            }

            // Load the most recent page of replies (oldest-first)
            var firstPage = await parentMessage.LoadRepliesAsync(limit: 2);

            // Paginate older replies using the id of the oldest message we have
            var oldest = firstPage[0];
            var olderPage = await parentMessage.LoadRepliesAsync(limit: 2, idLessThan: oldest.Id);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/threads/?language=unity#quote-message
        /// </summary>
        public async Task QuoteMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            // Send simple message with text only
            var message3 = await channel.SendNewMessageAsync("Hello");

// Send simple message with text only
            var quotedMessage = await channel.SendNewMessageAsync("Let's start a thread!");

            var messageWithQuote = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                QuotedMessage = quotedMessage,
                Text = "Hello",
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/unread-reminders/?language=unity#enabling-unread-reminders
        /// </summary>
        public async Task EnableUnreadReminders()
        {
            // Enable in Dashboard: Open your application -> Channel Types -> Pick Channel Type -> Enable "Message Reminders"
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#enabling-reminders
        /// </summary>
        public async Task EnableMessageReminders()
        {
            // This is a server-side feature, choose any of our server-side SDKs (or the Stream Dashboard) to enable it
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#creating-a-message-reminder
        /// </summary>
        public async Task CreateMessageReminder()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#updating-a-message-reminder
        /// </summary>
        public async Task UpdateMessageReminder()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#deleting-a-message-reminder
        /// </summary>
        public async Task DeleteMessageReminder()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#querying-message-reminders
        /// </summary>
        public async Task QueryMessageReminders()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#filtering-reminders
        /// </summary>
        public async Task FilterMessageReminders()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#pagination
        /// </summary>
        public async Task PaginateMessageReminders()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/message-reminders/?language=unity#events
        /// </summary>
        public async Task MessageReminderEvents()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/silent_messages/?language=unity
        /// </summary>
        public async Task SilentMessages()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

// This message will not trigger events for channel members
            var silentMessage = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "System message",
                Silent = true
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/search/?language=unity
        /// </summary>
        public async Task Search()
        {
            // Search for messages containing text
            var results = await Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                // Channel filter is required - here, channels the local user is a member of
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Members.In("john"),
                },
                Query = "supercalifragilisticexpialidocious",
                Limit = 10,
            });

            foreach (var hit in results.Results)
            {
                Debug.Log(hit.Message.Id); // Stateful IStreamMessage
                Debug.Log(hit.Message.Text);
                Debug.Log(hit.Message.User);
                Debug.Log(hit.Channel.Cid); // Stateful IStreamChannel (auto-watched by default)
            }

            // Search with message filters - mutually exclusive with Query
            var filtered = await Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = new IFieldFilterRule[]
                {
                    ChannelFilter.Members.In("john"),
                },
                MessageFilter = new IFieldFilterRule[]
                {
                    MessageFilter.Text.Autocomplete("super"),
                    MessageFilter.AttachmentType.In("image", "video"),
                },
                Limit = 10,
                // Set to false for one-off search bars where you don't want every result
                // channel to start receiving realtime updates.
                WatchResultChannels = true,
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/search/?language=unity#pagination
        /// </summary>
        public async Task SearchPagination()
        {
            var channelFilters = new IFieldFilterRule[]
            {
                ChannelFilter.Cid.EqualsTo("messaging:my-channel"),
            };
            var messageFilters = new IFieldFilterRule[]
            {
                MessageFilter.Text.Autocomplete("supercali"),
            };

            // First page with custom sorting
            var page1 = await Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = channelFilters,
                MessageFilter = messageFilters,
                Sort = MessagesSort
                    .OrderByDescending(MessageSortFieldName.Relevance)
                    .ThenByAscending(MessageSortFieldName.UpdatedAt),
                Limit = 10,
            });

            // Next page using the cursor returned by the previous response
            var page2 = await Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = channelFilters,
                MessageFilter = messageFilters,
                Limit = 10,
                Next = page1.Next,
            });

            // Previous page
            var page1Again = await Client.SearchMessagesAsync(new StreamSearchMessagesRequest
            {
                ChannelFilter = channelFilters,
                MessageFilter = messageFilters,
                Limit = 10,
                Next = page2.Previous,
            });
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/pinned_messages/?language=unity#pin-and-unpin-a-message
        /// </summary>
        public async Task PinAndUnpinMessage()
        {
            IStreamMessage message = null;

// Pin until unpinned
            await message.PinAsync();

// Pin for 7 days
            await message.PinAsync(new DateTime().AddDays(7));

// Unpin previously pinned message
            await message.UnpinAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/pinned_messages/?language=unity#retrieve-pinned-messages
        /// </summary>
        public async Task RetrievePinnedMessages()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/pinned_messages/?language=unity#paginate-over-all-pinned-messages
        /// </summary>
        public async Task PaginatePinnedMessages()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#message-translation-endpoint
        /// </summary>
        public async Task MessageTranslation()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#enabling-automatic-translation
        /// </summary>
        public async Task EnableAutomaticTranslation()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#set-user-language
        /// </summary>
        public async Task SetUserLanguage()
        {
            await Task.CompletedTask;
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}