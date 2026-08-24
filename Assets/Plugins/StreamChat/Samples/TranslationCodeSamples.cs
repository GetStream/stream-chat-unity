using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class TranslationCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#i18n-data
        /// </summary>
        public void ReadMessageTranslations()
        {
            IStreamMessage message = null;

// Translations are exposed on the message as the I18n dictionary
            foreach (var pair in message.I18n)
            {
                Debug.Log($"{pair.Key} = {pair.Value}"); // e.g. "fr_text = Bonjour, ..."
            }

// The original language is under the "language" key
            if (message.I18n.TryGetValue("language", out var originalLanguage))
            {
                Debug.Log(originalLanguage); // "en"
            }

// Read a specific translation, falling back to the original text
            var text = message.I18n.TryGetValue("fr_text", out var french) ? french : message.Text;
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#enabling-automatic-translation
        /// </summary>
        public async Task EnableChannelAutoTranslation()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "channel-id");

// Enable auto-translation for a single channel
            await channel.UpdatePartialAsync(new Dictionary<string, object>
            {
                { "auto_translation_enabled", true },
                { "auto_translation_language", "en" }
            });

// Read the current settings back from the channel
            Debug.Log(channel.AutoTranslationEnabled);
            Debug.Log(channel.AutoTranslationLanguage);

// Enabling auto-translation for the whole app is a server-side operation.
// Use one of our server-side SDKs or the Stream Dashboard for that.
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity
        /// </summary>
        public async Task TranslateAMessage()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "channel-id");
            var message = await channel.SendNewMessageAsync("Hello, world!");

// Translate into French (ISO language code). The translation is stored on the
// message, and every client watching the channel receives a message.updated event
            await message.TranslateAsync("fr");

// Translations are available from I18n under the "{language}_text" key, alongside
// a "language" key naming the detected source language
            var frenchText = message.I18n["fr_text"];
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/translation/?language=unity#set-user-language
        /// </summary>
        public async Task SetUserLanguage()
        {
// Set the language used to translate messages for a user
            await Client.UpsertUsersAsync(new[]
            {
                new StreamUserUpsertRequest
                {
                    Id = "user-id",
                    Language = "fr"
                }
            });
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}
