using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.LowLevelClient.Requests;
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
        {"tags", new [] {"Funny", "Unique"}}
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
            await Task.CompletedTask;
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
        /// https://getstream.io/chat/docs/unity/reminders/?language=unity
        /// </summary>
        public async Task Reminders()
        {
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

// Access to low-level client is left for backward compatibility. Soon simplified syntax for searching will be implemented
var searchResponse = await Client.LowLevelClient.MessageApi.SearchMessagesAsync(new SearchRequest
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

        /// <summary>
        /// https://getstream.io/chat/docs/unity/search/?language=unity#pagination
        /// </summary>
        public async Task SearchPagination()
        {
            await Task.CompletedTask;
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