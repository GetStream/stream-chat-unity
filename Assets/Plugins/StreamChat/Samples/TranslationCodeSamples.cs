using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    /// <summary>
    /// Code examples for the features/translation/ docs page
    /// </summary>
    internal sealed class TranslationCodeSamples
    {
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
