using System.IO;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;

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
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");

            //StreamTodo: test
            await channel.SendNewMessageAsync(new StreamSendMessageRequest
            {
                IsPendingMessage = null,
                PendingMessageMetadata = null,
                SkipEnrichUrl = null,
                SkipPush = null,
                Attachments = null,
                Id = null,
                MentionedUsers = null,
                ParentId = null,
                PinExpires = null,
                Pinned = null,
                PinnedAt = null,
                PinnedBy = null,
                QuotedMessage = null,
                ShowInChannel = null,
                Silent = null,
                Text = null,
                CustomData = null
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
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/send_reaction/?language=unity
        /// </summary>
        public async Task ReactionOverview()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, channelId: "my-channel-id");
            var message = await channel.SendNewMessageAsync("Hello world!");

            await message.SendReactionAsync("like");
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
            await Task.CompletedTask;
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
            await Task.CompletedTask;
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
            await Task.CompletedTask;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/search/?language=unity
        /// </summary>
        public async Task Search()
        {
            await Task.CompletedTask;
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
            await Task.CompletedTask;
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