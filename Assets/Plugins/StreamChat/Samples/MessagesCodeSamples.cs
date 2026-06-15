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
        /// https://getstream.io/chat/docs/unity/send-message/?language=unity#sending-a-message
        /// </summary>
        public async Task Overview()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello, world!");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send-message/?language=unity#sending-messages-with-attachments
        /// </summary>
        public async Task SendingMessagesWithAttachments()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            IStreamUser josh = null;

            var message = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                Text = "@Josh Check out this image!",

                // All fields below are optional
                Attachments = new List<StreamAttachmentRequest>
                {
                    new StreamAttachmentRequest
                    {
                        Type = "image",
                        AssetUrl = "https://bit.ly/2K74TaG",
                        ThumbUrl = "https://bit.ly/2Uumxti",
                    }
                },
                MentionedUsers = new List<IStreamUser> { josh },
                Pinned = true,
                PinExpires = new DateTimeOffset(DateTime.Now).AddDays(7),
                CustomData = new StreamCustomDataRequest
                {
                    { "priority", "high" }
                }
            });
        }

        // Code-only reference (kitchen-sink demo of every send-message option the
        // stateful Unity SDK exposes — threading, quoting, pinning, mentions,
        // and custom data in a single call). The published docs do not host an
        // equivalent "complex example" group on /send-message/ since the page
        // restructure on 2025-12-11 (commit 3ccdc786e), so per the orphan rule
        // there is no `///` URL to point at — the page now splits these
        // features across /threads/ (parent_id, show_in_channel),
        // /pinned-messages/ (pinned, pin_expires) and the section-specific
        // tabs above. Kept as a single grep-friendly reference for Unity
        // readers who want to see every option in one spot.
        public async Task ComplexExample()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

            IStreamUser someUser = null;

            // Send a quoted message
            var message3 = await channel.SendNewMessageAsync("Hello");

            // Send a parent message we can reply to in a thread
            var message2 = await channel.SendNewMessageAsync("Let's start a thread!");

            var message = await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                MentionedUsers = new List<IStreamUser> { someUser }, // Mention a user
                ParentId = message2.Id, // Write in a thread
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
        /// https://getstream.io/chat/docs/unity/send-message/?language=unity#updating-a-message
        /// </summary>
        public async Task UpdateAMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello, world!");

            await message.UpdateAsync(new StreamUpdateMessageRequest
            {
                Text = "Updated message text",
                CustomData = new StreamCustomDataRequest
                {
                    { "tags", new[] { "edited" } }
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
        /// https://getstream.io/chat/docs/unity/send-message/?language=unity#deleting-a-message
        /// </summary>
        public async Task DeleteAMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello, world!");

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
        /// https://getstream.io/chat/docs/unity/file-uploads/?language=unity#uploading-files-to-a-channel
        /// </summary>
        public async Task UploadFileOrImage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            var imageBytes = File.ReadAllBytes("path/to/image.jpg");
            var imageUploadResponse = await channel.UploadImageAsync(imageBytes, "image.jpg");
            var imageUrl = imageUploadResponse.FileUrl;

            var fileBytes = File.ReadAllBytes("path/to/document.pdf");
            var fileUploadResponse = await channel.UploadFileAsync(fileBytes, "document.pdf");
            var fileUrl = fileUploadResponse.FileUrl;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/file-uploads/?language=unity#uploading-standalone-files
        /// </summary>
        public Task UploadStandaloneFiles()
        {
            // Uploading standalone files via a client-level endpoint is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            // As a workaround, upload via channel.UploadImageAsync / channel.UploadFileAsync
            // and reuse the returned URL when upserting a user with Client.UpsertUsersAsync.
            return Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/file-uploads/?language=unity#deleting-files
        /// </summary>
        public async Task DeleteFileOrImage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            await channel.DeleteFileOrImageAsync("file-url");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/file-uploads/?language=unity#using-your-own-cdn
        /// </summary>
        public async Task UsingYourOwnCdn()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            // Upload to your CDN and obtain the file URL
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
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send-reaction/?language=unity#sending-a-reaction
        /// </summary>
        public async Task ReactionOverview()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            // Send a reaction with default score of 1
            await message.SendReactionAsync("like");

            // Send a reaction with custom score
            await message.SendReactionAsync("clap", 10);

            // Replace all previous reactions from this user
            await message.SendReactionAsync("love", enforceUnique: true);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send-reaction/?language=unity#removing-a-reaction
        /// </summary>
        public async Task RemoveReaction()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            await message.DeleteReactionAsync("like");
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send-reaction/?language=unity#paginating-reactions
        /// </summary>
        public async Task PaginateReactions()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            // Paginating reactions via API is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            // Each message exposes a snapshot of the reaction summary instead:

            // The 10 most recent reactions on the message
            foreach (var reaction in message.LatestReactions)
            {
                Debug.Log($"{reaction.Type} from {reaction.User?.Id}");
            }

            // The 10 most recent reactions left by the local user
            foreach (var reaction in message.OwnReactions)
            {
                Debug.Log($"My reaction: {reaction.Type}");
            }

            // Number of reactions per type, e.g. {"love": 3, "fire": 2}
            foreach (var entry in message.ReactionCounts)
            {
                Debug.Log($"{entry.Key}: {entry.Value} reactions");
            }

            // Sum of reaction scores per type (matches counts unless cumulative reactions are used)
            foreach (var entry in message.ReactionScores)
            {
                Debug.Log($"{entry.Key}: total score {entry.Value}");
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send-reaction/?language=unity#querying-reactions
        /// </summary>
        public async Task QueryReactions()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send-reaction/?language=unity#cumulative-reactions
        /// </summary>
        public async Task CumulativeReactions()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            await message.SendReactionAsync("clap", score: 5);
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
        /// https://getstream.io/chat/docs/unity/pinned-messages/?language=unity#pinning-and-unpinning-messages
        /// </summary>
        public async Task PinAndUnpinMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Important announcement");

            // Pin until unpinned
            await message.PinAsync();

            // Pin for 7 days
            await message.PinAsync(DateTime.UtcNow.AddDays(7));

            // Unpin previously pinned message
            await message.UnpinAsync();
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/pinned-messages/?language=unity#retrieving-pinned-messages
        /// </summary>
        public async Task RetrievePinnedMessages()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            // channel.PinnedMessages exposes the 10 most recent pinned messages loaded with the channel state.
            foreach (var pinnedMessage in channel.PinnedMessages)
            {
                Debug.Log($"{pinnedMessage.Id}: {pinnedMessage.Text}");
            }
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/pinned-messages/?language=unity#paginating-pinned-messages
        /// </summary>
        public async Task PaginatePinnedMessages()
        {
            // Paginating pinned messages via the dedicated pinned-messages endpoint is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            // The channel.PinnedMessages snapshot loaded with the channel state (max 10) is the available alternative.
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#message-translation-endpoint
        /// On-demand message translation is not yet exposed by the Unity SDK; the docs
        /// surface the canonical "raise an issue" placeholder. Tracked in B5 of the docs
        /// update plan.
        /// </summary>
        public Task MessageTranslation()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            return Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#enabling-automatic-translation
        /// Enabling automatic translation (per-channel or per-app) is server-side / not
        /// yet exposed by the Unity stateful client; the docs surface the canonical
        /// "raise an issue" placeholder. Tracked in B5 of the docs update plan.
        /// </summary>
        public Task EnableAutomaticTranslation()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            return Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#set-user-language
        /// Setting the user language at connect time is not yet exposed by the Unity
        /// stateful client; the docs surface the canonical "raise an issue" placeholder.
        /// Tracked in B5 of the docs update plan.
        /// </summary>
        public Task SetUserLanguage()
        {
            // This feature is not yet available in the Unity SDK.
            // Please let us know if you'd like this feature implemented: https://github.com/GetStream/stream-chat-unity/issues
            return Task.CompletedTask;
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}